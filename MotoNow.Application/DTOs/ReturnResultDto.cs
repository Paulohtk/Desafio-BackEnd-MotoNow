using System.Text.Json.Serialization;

namespace MotoNow.Application.DTOs
{
    public sealed class ReturnResultDto
    {
        [JsonPropertyName("data_inicio")]
        public DateTime StartAt { get; set; }

        [JsonPropertyName("data_previsao_termino")]
        public DateTime ExpectedEndAt { get; set; }

        [JsonPropertyName("data_devolucao")]
        public DateTime ReturnDate { get; set; }

        [JsonPropertyName("dias_utilizados")]
        public int UsedDays { get; set; }

        [JsonPropertyName("valor_diarias")]
        public decimal BaseAmount { get; set; }

        [JsonPropertyName("valor_multa")]
        public decimal PenaltyAmount { get; set; }

        [JsonPropertyName("valor_diarias_extras")]
        public decimal ExtraDaysAmount { get; set; }

        [JsonPropertyName("valor_total")]
        public decimal Total { get; set; }
    }
}
