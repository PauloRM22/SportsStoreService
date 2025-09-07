using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using StoreService.Application.Estoque.DTOs;
using StoreService.Application.Estoque.UseCases;

namespace SportsStoreService.Controllers
{
    [ApiController]
    [Route("api/estoque")]
    [Authorize(Policy = "AdminOnly")]
    public class EstoqueController : ControllerBase
    {
        [HttpPost("entrada")]
        [SwaggerOperation(
            Summary = "Registrar entrada de estoque",
            Description = "Registra a **entrada** de itens (NF, quantidade, observação) para um produto.",
            Tags = new[] { "Estoque" },
            OperationId = "Estoque_010_RegistrarEntrada"
        )]
        [ProducesResponseType(typeof(EntradaEstoqueResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
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
        [SwaggerOperation(
            Summary = "Consultar saldo do produto",
            Description = "Retorna o **saldo atual** em estoque para o produto informado.",
            Tags = new[] { "Estoque" },
            OperationId = "Estoque_020_ObterSaldo"
        )]
        [ProducesResponseType(typeof(SaldoProdutoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
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