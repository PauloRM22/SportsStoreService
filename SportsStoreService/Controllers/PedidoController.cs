using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using StoreService.Application.Pedidos.DTOs;
using StoreService.Application.Pedidos.UseCases;
using System.Security.Claims;

namespace SportsStoreService.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    [Authorize(Policy = "VendedorOnly")]
    public class PedidoController : ControllerBase
    {
        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar pedido",
            Description = "Cria um novo pedido com itens. O pedido fica **pendente** até ser confirmado.",
            Tags = new[] { "Pedidos" },
            OperationId = "Pedidos_010_Criar"
        )]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PedidoResponse>> Post(
            [FromBody] CreatePedidoRequest request,
            [FromServices] CriarPedidoComItensUseCase useCase,
            CancellationToken ct)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("uid")?.Value
                          ?? User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(idClaim, out var vendedorId))
                return Unauthorized(new { error = "NO_USER_ID", message = "Token sem identificador de usuário." });

            try
            {
                var resp = await useCase.ExecuteAsync(request, vendedorId, ct);  // ✅ passa o vendedorId
                return CreatedAtAction(nameof(GetById), new { id = resp.Id }, resp);
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(new { error = "VALIDATION", message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter pedido por Id",
            Description = "Retorna os dados de um pedido pelo **Id**.",
            Tags = new[] { "Pedidos" },
            OperationId = "Pedidos_020_ObterPorId"
        )]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
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
        [SwaggerOperation(
            Summary = "Confirmar pedido",
            Description = "Confirma o pedido e realiza a **baixa de estoque** dos itens.",
            Tags = new[] { "Pedidos" },
            OperationId = "Pedidos_030_Confirmar"
        )]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
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