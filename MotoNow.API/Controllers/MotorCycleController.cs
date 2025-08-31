using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MotoNow.Application.DTOs;
using MotoNow.Application.Results;
using MotoNow.Application.Services;

namespace MotoNow.API.Controllers
{
    [Route("motos")]
    [ApiController]
    public class MotorCycleController : ControllerBase
    {
        private readonly IMotorCycleService _service;
        public MotorCycleController(IMotorCycleService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateMotorCycleDto req, CancellationToken ct)
        {
            var id = await _service.CreateAsync(req.Identifier, req.Plate, req.Model, req.Year, ct);
            return CreatedAtAction(nameof(GetByIdAsync), id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id) => Ok(await _service.GetByIdAsync(id));

        [HttpGet]
        public async Task<IActionResult> GetAll(string? plate) => Ok(await _service.ListMotorcycleAsync(plate));


        [HttpDelete("motos/{id}")]
        public async Task<ActionResult> DeleteMoto(string id)
        {
            await _service.DeleteMotorcycleAsync(id);
            return Ok(new ApiSuccess<object>(
            Sucesso: true,
            Mensagem: "Moto removida com sucesso!",
            Dados: new { }));
        }

        [HttpPut("{id}/placa")]
        public async Task<IActionResult> UpdatePlate(string id, [FromBody] UpdatePlateMotorCycleDto body, CancellationToken ct)
        {
            var moto = await _service.UpdateMotorcyclePlate(id, body.Plate, ct);

            return Ok(new ApiSuccess<object>(
                Sucesso: true,
                Mensagem: "Placa modificada com sucesso",
                Dados: new { moto.Identifier, moto.Plate, moto.Model, moto.Year }
            ));
        }
    }
}
