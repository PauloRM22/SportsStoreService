using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using StoreService.Application.Users.DTOs;
using StoreService.Application.Users.UseCases;

namespace SportsStoreService.Controllers
{
    [ApiController]
    [Route("api/usuarios")]                // 🔹 plural, minúsculo
    [Authorize(Policy = "AdminOnly")]
    public class UsuariosController : ControllerBase
    {
        private readonly IHostEnvironment _env;
        private readonly CriarUsuarioUseCase _criar;
        private readonly AtualizarUsuarioUseCase _atualizar;
        private readonly ExcluirUsuarioUseCase _excluir;
        private readonly ObterUsuarioPorIdUseCase _obterPorId;
        private readonly ListarUsuariosUseCase _listar;

        public UsuariosController(
            IHostEnvironment env,
            CriarUsuarioUseCase criar,
            AtualizarUsuarioUseCase atualizar,
            ExcluirUsuarioUseCase excluir,
            ObterUsuarioPorIdUseCase obterPorId,
            ListarUsuariosUseCase listar)
        {
            _env = env;
            _criar = criar;
            _atualizar = atualizar;
            _excluir = excluir;
            _obterPorId = obterPorId;
            _listar = listar;
        }

        [HttpPost("bootstrap")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Criar primeiro usuário (bootstrap, DEV)",
            Description = "Cria o **primeiro usuário** sem autenticação. Após existir 1 usuário, retorna **403**.",
            Tags = new[] { "Usuários" }
        )]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Bootstrap([FromBody] CreateUsuarioRequest req, CancellationToken ct)
        {
            if (!_env.IsDevelopment())
                return NotFound();

            var existentes = await _listar.ExecuteAsync(ct);
            if (existentes.Any())
            {
                return Problem(
                    statusCode: StatusCodes.Status403Forbidden,
                    title: "Cadastro público desabilitado",
                    detail: "Já existe usuário no sistema. Faça login como Admin e use /api/usuarios."
                );
            }

            var resp = await _criar.ExecuteAsync(req, ct);
            return CreatedAtAction(nameof(ObterPorId), new { id = resp.Id }, resp);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar usuário",
            Description = "Cria um novo usuário. **Apenas Admin**.",
            Tags = new[] { "Usuários" }
        )]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Criar([FromBody] CreateUsuarioRequest req, CancellationToken ct)
        {
            var resp = await _criar.ExecuteAsync(req, ct);
            return CreatedAtAction(nameof(ObterPorId), new { id = resp.Id }, resp);
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Atualizar usuário",
            Description = "Atualiza nome/e-mail/tipo. **Apenas Admin**.",
            Tags = new[] { "Usuários" }
        )]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar([FromBody] UpdateUsuarioRequest req, CancellationToken ct)
        {
            var resp = await _atualizar.ExecuteAsync(req, ct);
            return Ok(resp);
        }

        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Excluir usuário",
            Description = "Exclui um usuário pelo **Id**. **Apenas Admin**.",
            Tags = new[] { "Usuários" }
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir([FromRoute] Guid id, CancellationToken ct)
        {
            await _excluir.ExecuteAsync(id, ct);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter usuário por Id",
            Description = "Retorna os dados de um usuário (por **Id**). **Apenas Admin**.",
            Tags = new[] { "Usuários" }
        )]
        [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id, CancellationToken ct)
        {
            var resp = await _obterPorId.ExecuteAsync(id, ct);
            return Ok(resp);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar usuários",
            Description = "Lista usuários com paginação futura (TODO). **Apenas Admin**.",
            Tags = new[] { "Usuários" }
        )]
        [ProducesResponseType(typeof(IEnumerable<UsuarioResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Listar(CancellationToken ct)
        {
            var list = await _listar.ExecuteAsync(ct);
            return Ok(list);
        }
    }
}
