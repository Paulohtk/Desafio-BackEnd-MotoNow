using Microsoft.AspNetCore.Mvc;
using MotoNow.Application.DTOs;
using MotoNow.Application.Results;
using MotoNow.Application.Services;

namespace MotoNow.API.Controllers;

[ApiController]
[Route("/Locacao")]
public sealed class RentalController : ControllerBase
{
    private readonly IRentalService _service;
    public RentalController(IRentalService service) =>
        _service = service ?? throw new ArgumentNullException(nameof(service));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRentalDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return Ok(new ApiSuccess<object>(
          Sucesso: true,
          Mensagem: "Locação criada com sucesso!",
          Dados: result));
    }

    [HttpPut("{id}/devolucao")]
    public async Task<IActionResult> Return(string id, [FromBody] ReturnRequestDto body, CancellationToken ct)
    {
        var result = await _service.ReturnAsync(id, body.ReturnDate, ct);
        return Ok(new ApiSuccess<object>(
           Sucesso: true,
           Mensagem: "Data de devolução informada com sucesso",
           Dados: result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RentalDetailsDto>> GetById([FromRoute] string id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return Ok(new ApiSuccess<object>(
           Sucesso: true,
           Mensagem: "Locação encontrada com sucesso!",
           Dados: result));
    }

}
