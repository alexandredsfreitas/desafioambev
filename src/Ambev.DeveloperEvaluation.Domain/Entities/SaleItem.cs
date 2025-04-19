using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an individual item within a sale.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets the ID of the product being sold.
    /// </summary>
    public Guid ProductId { get; private set; }
    
    /// <summary>
    /// Gets the name of the product (denormalized for reference).
    /// </summary>
    public string ProductName { get; private set; }

    /// <summary>
    /// Gets the quantity of the product being purchased.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Gets the discount percentage applied to this item.
    /// </summary>
    public decimal DiscountPercentage { get; private set; }

    /// <summary>
    /// Gets the total discount amount applied to this item.
    /// </summary>
    public decimal DiscountAmount { get; private set; }

    /// <summary>
    /// Gets the total amount for this item after discounts.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets whether this item has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Gets the ID of the sale this item belongs to.
    /// </summary>
    public Guid SaleId { get; private set; }

    /// <summary>
    /// Reference to the parent sale (for entity framework).
    /// </summary>
    public Sale Sale { get; private set; }

    /// <summary>
    /// Initializes a new instance of the SaleItem class.
    /// </summary>
    protected SaleItem()
    {
        IsCancelled = false;
    }

    /// <summary>
    /// Creates a new sale item.
    /// </summary>
    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        
        if (quantity > 20)
            throw new ArgumentException("Cannot sell more than 20 identical items", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero", nameof(unitPrice));

        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        IsCancelled = false;

        ApplyDiscounts();
        CalculateTotal();
    }

    /// <summary>
    /// Updates the quantity of this item and recalculates discounts and totals.
    /// </summary>
    /// <param name="newQuantity">The new quantity</param>
    public void UpdateQuantity(int newQuantity)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled item");
            
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));
        
        if (newQuantity > 20)
            throw new ArgumentException("Cannot sell more than 20 identical items", nameof(newQuantity));

        Quantity = newQuantity;
        ApplyDiscounts();
        CalculateTotal();
    }

    /// <summary>
    /// Cancels this item.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        TotalAmount = 0;
        DiscountAmount = 0;
    }

    /// <summary>
    /// Applies quantity-based discounts according to business rules.
    /// </summary>
    private void ApplyDiscounts()
    {
        // Apply business rules for discounts
        if (Quantity >= 10 && Quantity <= 20)
        {
            // 20% discount for 10-20 items
            DiscountPercentage = 0.20m;
        }
        else if (Quantity >= 4)
        {
            // 10% discount for 4+ items
            DiscountPercentage = 0.10m;
        }
        else
        {
            // No discount for less than 4 items
            DiscountPercentage = 0;
        }

        DiscountAmount = UnitPrice * Quantity * DiscountPercentage;
    }

    /// <summary>
    /// Calculates the total amount for this item after discounts.
    /// </summary>
    private void CalculateTotal()
    {
        var subtotal = UnitPrice * Quantity;
        TotalAmount = subtotal - DiscountAmount;
    }
}