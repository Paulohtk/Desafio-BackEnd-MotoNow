
namespace MotoNow.Domain.Entities;

public sealed class MotorcycleNotification : BaseEntity
{
    private MotorcycleNotification() { } // EF

    public MotorcycleNotification(string identifier, string plate, string model, int year, DateTime occurredAt)
    {
        Identifier = identifier;
        Plate = plate;
        Model = model;
        Year = year;
        OccurredAt = occurredAt;
        ReceivedAt = DateTime.UtcNow;
    }

    public string Plate { get; private set; } = default!;
    public string Model { get; private set; } = default!;
    public int Year { get; private set; }

    public DateTime OccurredAt { get; private set; }
    public DateTime ReceivedAt { get; private set; }
}
