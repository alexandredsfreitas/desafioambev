using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the system.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets the unique sale number that identifies this transaction.
    /// </summary>
    public string SaleNumber { get; private set; }

    /// <summary>
    /// Gets the date and time when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; private set; }

    /// <summary>
    /// Gets or sets the customer who made the purchase.
    /// </summary>
    public Guid CustomerId { get; private set; }
    
    /// <summary>
    /// Gets the customer's name (denormalized for reference).
    /// </summary>
    public string CustomerName { get; private set; }

    /// <summary>
    /// Gets the branch where the sale was made.
    /// </summary>
    public Guid BranchId { get; private set; }
    
    /// <summary>
    /// Gets the branch name (denormalized for reference).
    /// </summary>
    public string BranchName { get; private set; }

    /// <summary>
    /// Gets the total amount of the sale after all discounts.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets whether the sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Gets the collection of items in this sale.
    /// </summary>
    public ICollection<SaleItem> Items { get; private set; }

    /// <summary>
    /// Gets the date and time when the sale was created in the system.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale information.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// </summary>
    public Sale()
    {
        Items = new List<SaleItem>();
        CreatedAt = DateTime.UtcNow;
        SaleDate = DateTime.UtcNow;
        SaleNumber = GenerateSaleNumber();
        IsCancelled = false;
    }

    /// <summary>
    /// Creates a new sale with customer and branch information.
    /// </summary>
    public Sale(Guid customerId, string customerName, Guid branchId, string branchName) : this()
    {
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
    }

    /// <summary>
    /// Adds an item to the sale.
    /// </summary>
    /// <param name="productId">The ID of the product</param>
    /// <param name="productName">The name of the product</param>
    /// <param name="quantity">The quantity being purchased</param>
    /// <param name="unitPrice">The unit price of the product</param>
    /// <returns>The newly added SaleItem</returns>
    public SaleItem AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled sale");

        // Check if we already have this product in the sale
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            // Update existing item instead of adding a new one
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            CalculateTotals();
            return existingItem;
        }

        // Enforce business rule: max 20 items per product
        if (quantity > 20)
            throw new InvalidOperationException("Cannot sell more than 20 identical items");

        var item = new SaleItem(productId, productName, quantity, unitPrice);
        ((List<SaleItem>)Items).Add(item);
        
        CalculateTotals();
        UpdatedAt = DateTime.UtcNow;
        
        return item;
    }

    /// <summary>
    /// Updates the quantity of an existing sale item.
    /// </summary>
    /// <param name="itemId">The ID of the item to update</param>
    /// <param name="quantity">The new quantity</param>
    public void UpdateItemQuantity(Guid itemId, int quantity)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot update items in a cancelled sale");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new KeyNotFoundException($"Item with ID {itemId} not found in this sale");

        item.UpdateQuantity(quantity);
        CalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels an individual item in the sale.
    /// </summary>
    /// <param name="itemId">The ID of the item to cancel</param>
    public void CancelItem(Guid itemId)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot cancel items in an already cancelled sale");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new KeyNotFoundException($"Item with ID {itemId} not found in this sale");

        item.Cancel();
        CalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the entire sale.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        
        // Cancel all items
        foreach (var item in Items)
        {
            item.Cancel();
        }
        
        TotalAmount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Recalculates the total amount of the sale based on all active items.
    /// </summary>
    private void CalculateTotals()
    {
        TotalAmount = Items
            .Where(i => !i.IsCancelled)
            .Sum(i => i.TotalAmount);
    }

    /// <summary>
    /// Generates a unique sale number.
    /// </summary>
    private string GenerateSaleNumber()
    {
        return $"SALE-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }

    /// <summary>
    /// Performs validation of the sale entity.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing validation results
    /// </returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}