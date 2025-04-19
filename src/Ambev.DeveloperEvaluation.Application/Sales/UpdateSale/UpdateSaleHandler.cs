using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler
    /// </summary>
    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        // Get the sale
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");
            
        if (sale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale");
            
        // Process the items
        foreach (var itemCommand in command.Items)
        {
            if (itemCommand.ItemId.HasValue)
            {
                // Update existing item
                sale.UpdateItemQuantity(itemCommand.ItemId.Value, itemCommand.Quantity);
            }
            else
            {
                // Add new item
                sale.AddItem(
                    itemCommand.ProductId, 
                    itemCommand.ProductName, 
                    itemCommand.Quantity, 
                    itemCommand.UnitPrice
                );
            }
        }
        
        // Validate the sale
        var saleValidation = sale.Validate();
        if (!saleValidation.IsValid)
        {
            throw new ValidationException(
                string.Join(", ", saleValidation.Errors.Select(e => e.Error)));
        }
        
        // Persist the sale
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        
        // Publish the SaleModified event
        await _mediator.Publish(new SaleModifiedEvent() { SaleId = updatedSale.Id }, cancellationToken);
        
        // Return the result
        return new UpdateSaleResult
        {
            Id = updatedSale.Id,
            SaleNumber = updatedSale.SaleNumber,
            TotalAmount = updatedSale.TotalAmount
        };
    }
}