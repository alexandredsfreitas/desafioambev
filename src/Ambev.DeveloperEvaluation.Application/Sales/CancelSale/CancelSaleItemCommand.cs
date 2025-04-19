using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command for cancelling a specific item in a sale
/// </summary>
public class CancelSaleItemCommand : IRequest<CancelSaleItemResult>
{
    /// <summary>
    /// The ID of the sale containing the item
    /// </summary>
    public Guid SaleId { get; set; }
    
    /// <summary>
    /// The ID of the item to cancel
    /// </summary>
    public Guid ItemId { get; set; }
}