using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;

/// <summary>
/// Handler for the SaleRegistered event
/// </summary>
public class SaleRegisteredEventHandler : INotificationHandler<SaleRegisteredEvent>
{
    private readonly ILogger<SaleRegisteredEventHandler> _logger;

    public SaleRegisteredEventHandler(ILogger<SaleRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sale created event: Sale ID {SaleId}", notification.SaleId);
        
        return Task.CompletedTask;
    }
}