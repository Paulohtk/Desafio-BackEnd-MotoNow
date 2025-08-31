// MotoNow.API/Middlewares/ApiExceptionHandler.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoNow.Application.Results;
using Npgsql;
using System.Text.Json;

namespace MotoNow.API.Middlewares;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception ex, CancellationToken ct)
    {
        var (status, code, message, details) = Map(ex);

        http.Response.StatusCode = status;
        http.Response.ContentType = "application/json; charset=utf-8";

        var payload = new ApiError(
            Sucesso: false,
            Codigo: code,
            Mensagem: message,
            Detalhes: details,
            TraceId: http.TraceIdentifier);

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await http.Response.WriteAsync(json, ct);
        return true;
    }

    private static (int status, string code, string message, object? details) Map(Exception ex)
    {
        if (ex is ArgumentException arg)
            return (StatusCodes.Status400BadRequest, "invalid_argument", arg.Message, new { param = arg.ParamName });

        if (ex is KeyNotFoundException)
            return (StatusCodes.Status404NotFound, "not_found", ex.Message, null);

        if (ex is InvalidOperationException)
            return (StatusCodes.Status409Conflict, "conflict", ex.Message, null);

        if (ex is ValidationException vex)
            return (StatusCodes.Status422UnprocessableEntity, "validation_error", vex.Message, vex.ValidationResult);

        if (ex is DbUpdateException db)
        {
            if (db.InnerException is PostgresException pg)
            {
                if (pg.SqlState == PostgresErrorCodes.UniqueViolation)
                    return (StatusCodes.Status409Conflict, "unique_violation", "Registro duplicado.", new { constraint = pg.ConstraintName });

                if (pg.SqlState == PostgresErrorCodes.ForeignKeyViolation)
                    return (StatusCodes.Status409Conflict, "foreign_key_violation", "Violação de integridade referencial.", new { constraint = pg.ConstraintName });
            }
            return (StatusCodes.Status400BadRequest, "db_error", "Erro ao persistir dados.", db.InnerException?.Message ?? db.Message);
        }

        return (StatusCodes.Status500InternalServerError, "internal_error", "Erro interno do servidor.", ex.Message);
    }
}
