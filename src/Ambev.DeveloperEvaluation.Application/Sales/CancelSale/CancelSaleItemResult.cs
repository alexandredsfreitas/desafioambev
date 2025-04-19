namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Response model for CancelSaleItem operation
/// </summary>
public class CancelSaleItemResult
{
    /// <summary>
    /// Indicates whether the cancellation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The ID of the sale containing the cancelled item
    /// </summary>
    public Guid SaleId { get; set; }
    
    /// <summary>
    /// The ID of the cancelled item
    /// </summary>
    public Guid ItemId { get; set; }
    
    /// <summary>
    /// The updated total amount of the sale after cancellation
    /// </summary>
    public decimal UpdatedSaleTotal { get; set; }
}
