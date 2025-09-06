using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.Pedidos.DTOs;
using StoreService.Application.Pedidos.UseCases;

namespace SportsStoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "VendedorOnly")]
    public class PedidoController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<PedidoResponse>> Post(
            [FromBody] CreatePedidoRequest request,
            [FromServices] CriarPedidoComItensUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                var resp = await useCase.ExecuteAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { id = resp.Id }, resp);
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(new { error = "VALIDATION", message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PedidoResponse>> GetById(
            Guid id,
            [FromServices] ObterPedidoPorIdUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                var resp = await useCase.ExecuteAsync(id, ct);
                return Ok(resp);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = "NOT_FOUND", message = ex.Message });
            }
        }

        [HttpPost("{id:guid}/confirmar")]
        public async Task<ActionResult<PedidoResponse>> Confirmar(
            Guid id,
            [FromServices] ConfirmarPedidoUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                var resp = await useCase.ExecuteAsync(id, ct);
                return Ok(resp);
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(new { error = "VALIDATION", message = ex.Message });
            }
        }
    }
}