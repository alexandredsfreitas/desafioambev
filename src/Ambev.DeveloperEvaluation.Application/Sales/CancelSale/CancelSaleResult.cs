namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Response model for CancelSale operation
/// </summary>
public class CancelSaleResult
{
    /// <summary>
    /// Indicates whether the cancellation was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The ID of the cancelled sale
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The sale number of the cancelled sale
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;
}