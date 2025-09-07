using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using StoreService.Application.Produtos.UseCases;
using StoreService.Application.Produtos.DTOs;

namespace SportsStoreService.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    [Authorize(Policy = "AdminOnly")]
    public class ProdutoController : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar produtos",
            Description = "Retorna a lista de produtos cadastrados.",
            Tags = new[] { "Produtos" },
            OperationId = "Produtos_010_Listar"
        )]
        [ProducesResponseType(typeof(List<ProdutoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<ProdutoResponse>>> Get(
            [FromServices] ListarProdutosUseCase useCase,
            CancellationToken ct)
        {
            var result = await useCase.ExecuteAsync(ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter produto por Id",
            Description = "Retorna os dados de um produto pelo **Id**.",
            Tags = new[] { "Produtos" },
            OperationId = "Produtos_020_ObterPorId"
        )]
        [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProdutoResponse>> GetById(
            Guid id,
            [FromServices] ObterProdutoPorIdUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                var result = await useCase.ExecuteAsync(id, ct);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = "NOT_FOUND", message = ex.Message });
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar produto",
            Description = "Cria um novo produto. Campos obrigatórios: **nome**, **descricao**, **preco**.",
            Tags = new[] { "Produtos" },
            OperationId = "Produtos_030_Criar"
        )]
        [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProdutoResponse>> Post(
            [FromBody] CreateProdutoRequest request,
            [FromServices] CriarProdutoUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                var result = await useCase.ExecuteAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(new { error = "VALIDATION", message = ex.Message });
            }
        }

        [HttpPut]
        [SwaggerOperation(
        Summary = "Atualizar produto",
        Description = "Atualiza os dados de um produto existente. O Id é informado no corpo da requisição.",
        Tags = new[] { "Produtos" },
        OperationId = "Produtos_040_Atualizar")]
        [ProducesResponseType(typeof(ProdutoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProdutoResponse>> Put(
            [FromBody] UpdateProdutoRequest request,
            [FromServices] AtualizarProdutoUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                var result = await useCase.ExecuteAsync(request, ct);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(new { error = "VALIDATION", message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Excluir produto",
            Description = "Exclui um produto pelo **Id**.",
            Tags = new[] { "Produtos" },
            OperationId = "Produtos_050_Excluir"
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(
            Guid id,
            [FromServices] RemoverProdutoUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                await useCase.ExecuteAsync(id, ct);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = "NOT_FOUND", message = ex.Message });
            }
        }
    }
}
