using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public class ItemCancelledEvent : INotification
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
}