using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MotoNow.Application.DTOs
{
    public sealed class CreateRentalDto
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = default!;

        [JsonPropertyName("entregador_id")]
        public string DeliveryDriverId { get; set; } = default!; 

        [JsonPropertyName("moto_id")]
        public string MotorcycleId { get; set; } = default!; 

        [JsonPropertyName("data_inicio")]
        public DateTime StartAt { get; set; }

        [JsonPropertyName("data_termino")]
        public DateTime? EndAt { get; set; }

        [JsonPropertyName("data_previsao_termino")]
        public DateTime ExpectedEndAt { get; set; }

        [JsonPropertyName("plano")]
        public int PlanDays { get; set; }
    }
}
