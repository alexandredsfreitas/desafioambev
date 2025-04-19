namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Represents the response returned after successfully creating a new sale.
/// </summary>
public class CreateSaleResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the newly created sale.
    /// </summary>
    public Guid Id { get; set; }
        
    /// <summary>
    /// Gets or sets the sale number of the newly created sale.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;
}