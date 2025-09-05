using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreService.Application.Auth.DTOs;
using StoreService.Application.Auth.UseCases;

namespace SportsStoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            [FromServices] LoginUseCase useCase,
            CancellationToken ct)
        {
            try
            {
                var resp = await useCase.ExecuteAsync(request, ct);
                return Ok(resp);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 401 para credenciais inválidas
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}