using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MotoNow.Application.DTOs
{
    public sealed class UpdatePlateMotorCycleDto
    {
        [JsonPropertyName("placa")]
        public string Plate { get; set; } = default!;
    }
}
