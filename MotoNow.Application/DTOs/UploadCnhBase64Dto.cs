using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MotoNow.Application.DTOs
{
    public sealed class UploadCnhBase64Dto
    {
        [JsonPropertyName("imagem_cnh")]
        public string Base64 { get; set; } = default!;
    }
}
