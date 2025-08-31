using MassTransit;
using MotoNow.Application.Abstractions.Messages;
using MotoNow.Domain.Entities;
using MotoNow.Domain.Repositories;

namespace MotoNow.Application.Services.Consumers;

public sealed class MotorcycleRegisteredConsumer : IConsumer<MotorcycleRegisteredEvent>
{
    private readonly IRepository<MotorcycleNotification> _notifications;

    public MotorcycleRegisteredConsumer(IRepository<MotorcycleNotification> notifications)
        => _notifications = notifications;

    public async Task Consume(ConsumeContext<MotorcycleRegisteredEvent> context)
    {
        var msg = context.Message;
        if (msg.Year != 2024) return;

        var notif = new MotorcycleNotification(
             msg.Identifier, msg.Plate, msg.Model, msg.Year, msg.OccurredAt);

        await _notifications.AddAsync(notif, context.CancellationToken);
        await _notifications.SaveChangesAsync(context.CancellationToken);
    }
}
