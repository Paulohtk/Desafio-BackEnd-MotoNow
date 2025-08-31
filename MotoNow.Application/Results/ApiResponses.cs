using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoNow.Application.Results
{
    public interface IApiEnvelope { }
    public record ApiSuccess<T>(bool Sucesso, string Mensagem, T Dados) : IApiEnvelope;
    public record ApiError(bool Sucesso, string Codigo, string Mensagem, object? Detalhes = null, string? TraceId = null) : IApiEnvelope;
}
