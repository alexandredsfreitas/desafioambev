using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        // Create the sale
        var sale = new Sale(command.CustomerId, command.CustomerName, command.BranchId, command.BranchName);
        
        // Add items to the sale
        foreach (var itemCommand in command.Items)
        {
            sale.AddItem(
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice
            );
        }
        
        // Validate the sale
        var saleValidation = sale.Validate();
        if (!saleValidation.IsValid)
        {
            throw new ValidationException(
                string.Join(", ", saleValidation.Errors.Select(e => e.Error)));
        }
        
        // Persist the sale
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        
        // Publish the SaleCreated event
        await _mediator.Publish(new SaleRegisteredEvent() { SaleId = createdSale.Id }, cancellationToken);
        
        // Return the result
        return new CreateSaleResult
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber
        };
    }
}