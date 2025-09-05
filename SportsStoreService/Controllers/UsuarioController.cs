using Microsoft.AspNetCore.Mvc;
using StoreService.Application.Users.DTOs;
using StoreService.Application.Users.UseCases;

namespace SportsStoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly CriarUsuarioUseCase _criar;
        private readonly AtualizarUsuarioUseCase _atualizar;
        private readonly ExcluirUsuarioUseCase _excluir;
        private readonly ObterUsuarioPorIdUseCase _obterPorId;
        private readonly ListarUsuariosUseCase _listar;

        public UsuarioController(
            CriarUsuarioUseCase criar,
            AtualizarUsuarioUseCase atualizar,
            ExcluirUsuarioUseCase excluir,
            ObterUsuarioPorIdUseCase obterPorId,
            ListarUsuariosUseCase listar)
        {
            _criar = criar;
            _atualizar = atualizar;
            _excluir = excluir;
            _obterPorId = obterPorId;
            _listar = listar;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CreateUsuarioRequest req, CancellationToken ct)
        {
            var resp = await _criar.ExecuteAsync(req, ct);
            return CreatedAtAction(nameof(ObterPorId), new { id = resp.Id }, resp);
        }

        [HttpPut]
        public async Task<IActionResult> Atualizar([FromBody] UpdateUsuarioRequest req, CancellationToken ct)
        {
            var resp = await _atualizar.ExecuteAsync(req, ct);
            return Ok(resp);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Excluir([FromRoute] Guid id, CancellationToken ct)
        {
            await _excluir.ExecuteAsync(id, ct);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id, CancellationToken ct)
        {
            var resp = await _obterPorId.ExecuteAsync(id, ct);
            return Ok(resp);
        }

        [HttpGet]
        public async Task<IActionResult> Listar(CancellationToken ct)
        {
            var list = await _listar.ExecuteAsync(ct);
            return Ok(list);
        }
    }
}