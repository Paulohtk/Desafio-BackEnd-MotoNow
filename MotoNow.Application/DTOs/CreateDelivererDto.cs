using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MotoNow.Application.DTOs
{
    public sealed class CreateDelivererDto
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = default!;

        [JsonPropertyName("nome")]
        public string Name { get; set; } = default!;

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = default!;

        [JsonPropertyName("data_nascimento")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("numero_cnh")]
        public string DriverLicenseNumber { get; set; } = default!;

        [JsonPropertyName("tipo_cnh")]
        public string DriverLicenseClass { get; set; } = default!; 

        [JsonPropertyName("imagem_cnh")]
        public string? DriverLicenseImageBase64 { get; set; } 
    }
}
