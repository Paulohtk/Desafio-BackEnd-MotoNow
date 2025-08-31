using Microsoft.AspNetCore.Mvc;
using MotoNow.Application.DTOs;
using MotoNow.Application.Results;
using MotoNow.Application.Services;

namespace MotoNow.API.Controllers;

[ApiController]
[Route("entregadores")]
public sealed class DeliveryDriversController : ControllerBase
{
    private readonly IDeliveryDriverService _service;
    public DeliveryDriversController(IDeliveryDriverService service) =>
        _service = service ?? throw new ArgumentNullException(nameof(service));

    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] CreateDelivererDto body, CancellationToken ct)
    {
        var id = await _service.CreateAsync(body.Identifier, body.Name, body.Cnpj, body.BirthDate, body.DriverLicenseNumber, body.DriverLicenseClass, body.DriverLicenseImageBase64, ct);
        return Created($"/api/entregadores/{id}", new { id });
    }

    [HttpPost("{id}/cnh-imagem")]
    [Consumes("application/json")]
    public async Task<IActionResult> UploadDriverLicenseImage( [FromRoute] string id, [FromBody] UploadCnhBase64Dto body,  CancellationToken ct)
    {
        var response = await _service.UpdateDriverLicense(id, body, ct);
        return Ok(new ApiSuccess<object>(
                Sucesso: true,
                Mensagem: response,
                Dados: new { }
            ));
    }
}
