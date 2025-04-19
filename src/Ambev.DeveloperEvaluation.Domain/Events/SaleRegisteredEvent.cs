using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleRegisteredEvent : INotification
{
    public Guid SaleId { get; set; }
}