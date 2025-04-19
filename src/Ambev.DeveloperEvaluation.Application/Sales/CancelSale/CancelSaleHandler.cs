using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleCommand requests
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler
    /// </summary>
    public CancelSaleHandler(
        ISaleRepository saleRepository,
        IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request
    /// </summary>
    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");
            
        if (sale.IsCancelled)
            return new CancelSaleResult { Success = true, Id = sale.Id, SaleNumber = sale.SaleNumber };
            
        // Cancel the sale
        sale.Cancel();
        
        // Update the sale in repository
        await _saleRepository.UpdateAsync(sale, cancellationToken);
        
        // Publish the SaleCancelled event
        await _mediator.Publish(new SaleCancelledEvent { SaleId = sale.Id }, cancellationToken);

        return new CancelSaleResult { Success = true, Id = sale.Id, SaleNumber = sale.SaleNumber };
    }
}