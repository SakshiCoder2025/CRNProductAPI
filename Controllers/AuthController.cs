using Asp.Versioning;
using CRNProductAPI.Authentication;
using CRNProductAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CRNProductAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        #region Constructor
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(ITokenService tokenService, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {  
            if (dto.Username != "admin" || dto.Password != "Admin@123")
            {
                _logger.LogWarning("Failed login attempt for username {Username}", dto.Username);
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            var tokens = await _tokenService.GenerateTokensAsync(dto.Username);
            _logger.LogInformation("User {Username} logged in successfully", dto.Username);

            return Ok(tokens);
        }
        #endregion

        #region Refresh Token
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var tokens = await _tokenService.RefreshTokenAsync(dto.RefreshToken);

            if (tokens == null)
                return Unauthorized(new { Message = "Invalid or expired refresh token" });

            return Ok(tokens);
        }
        #endregion

        #region Revoke Token
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto dto)
        {
            await _tokenService.RevokeRefreshTokenAsync(dto.RefreshToken);
            return Ok(new { Message = "Token revoked successfully" });
        }
        #endregion
    }
}