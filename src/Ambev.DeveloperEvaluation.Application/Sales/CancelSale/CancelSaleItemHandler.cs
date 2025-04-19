using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleItemCommand requests
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of CancelSaleItemHandler
    /// </summary>
    public CancelSaleItemHandler(
        ISaleRepository saleRepository,
        IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the CancelSaleItemCommand request
    /// </summary>
    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");
            
        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot modify a cancelled sale");
            
        // Cancel the specific item
        sale.CancelItem(request.ItemId);
        
        // Update the sale in repository
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        
        // Publish the ItemCancelled event
        await _mediator.Publish(new ItemCancelledEvent 
        { 
            SaleId = sale.Id,
            ItemId = request.ItemId
        }, cancellationToken);

        return new CancelSaleItemResult 
        { 
            Success = true, 
            SaleId = sale.Id, 
            ItemId = request.ItemId,
            UpdatedSaleTotal = updatedSale.TotalAmount
        };
    }
}