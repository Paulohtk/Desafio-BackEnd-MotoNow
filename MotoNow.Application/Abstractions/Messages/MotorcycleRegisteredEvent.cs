using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoNow.Application.Abstractions.Messages;

public sealed class MotorcycleRegisteredEvent
{
    public string Identifier { get; init; } = default!;
    public string Plate { get; init; } = default!;
    public string Model { get; init; } = default!;
    public int Year { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
