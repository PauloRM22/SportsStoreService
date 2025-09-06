using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.Products.DTOs;
using StoreService.Application.Products.UseCases;

namespace SportsStoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class ProdutoController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ProdutoResponse>>> Get(
            [FromServices] ListarProdutosUseCase useCase,
            CancellationToken ct)
        {
            var result = await useCase.ExecuteAsync(ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
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

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProdutoResponse>> Put(
            Guid id,
            [FromBody] UpdateProdutoRequest request,
            [FromServices] AtualizarProdutoUseCase useCase,
            CancellationToken ct)
        {
            if (id != request.Id)
                return BadRequest(new { error = "INVALID_ID", message = "Id do path difere do body." });

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