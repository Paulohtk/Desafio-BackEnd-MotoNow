using System.Text.Json.Serialization;

namespace MotoNow.Application.DTOs
{
    public sealed class ReturnRequestDto
    {
        [JsonPropertyName("data_devolucao")]
        public DateTime ReturnDate { get; set; }
    }
}
