using Microsoft.AspNetCore.Mvc;
using StoreService.Application.Estoque.DTOs;
using StoreService.Application.Estoque.UseCases;

namespace SportsStoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        [HttpPost("entrada")]
        public async Task<ActionResult<EntradaEstoqueResponse>> RegistrarEntrada(
            [FromBody] CreateEntradaEstoqueRequest request,
            [FromServices] RegistrarEntradaEstoqueUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                var resp = await useCase.ExecuteAsync(request, ct);
                return Ok(resp);
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(new { error = "VALIDATION", message = ex.Message });
            }
        }

        [HttpGet("saldo/{produtoId:guid}")]
        public async Task<ActionResult<SaldoProdutoResponse>> ObterSaldo(
            Guid produtoId,
            [FromServices] ConsultarSaldoProdutoUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                var resp = await useCase.ExecuteAsync(produtoId, ct);
                return Ok(resp);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = "NOT_FOUND", message = ex.Message });
            }
        }
    }
}