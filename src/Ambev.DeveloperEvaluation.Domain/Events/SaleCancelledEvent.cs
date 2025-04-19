using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class SaleCancelledEvent : INotification
{
    public Guid SaleId { get; set; }
}