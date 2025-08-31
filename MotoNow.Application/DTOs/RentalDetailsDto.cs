using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MotoNow.Application.DTOs
{
    public sealed class RentalDetailsDto
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = default!;

        [JsonPropertyName("valor_diaria")]
        public int DailyRate { get; set; }

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

        [JsonPropertyName("data_devolucao")]
        public DateTime? ReturnDate { get; set; }

        [JsonPropertyName("valor_total")]
        public decimal TotalAmount { get; set; }
    }
}
