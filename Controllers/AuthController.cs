using Asp.Versioning;
using CRNProductAPI.Interfaces;
using CRNProductAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CRNProductAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ITokenService tokenService, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        // POST: api/v1/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            // NOTE: Assignment scope ke liye simple hardcoded check.
            // Production me: Users table + BCrypt/Argon2 se hashed password verify karna chahiye.
            if (dto.Username != "admin" || dto.Password != "Admin@123")
            {
                _logger.LogWarning("Failed login attempt for username {Username}", dto.Username);
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            var tokens = await _tokenService.GenerateTokensAsync(dto.Username);
            _logger.LogInformation("User {Username} logged in successfully", dto.Username);

            return Ok(tokens);
        }

        // POST: api/v1/Auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var tokens = await _tokenService.RefreshTokenAsync(dto.RefreshToken);

            if (tokens == null)
                return Unauthorized(new { Message = "Invalid or expired refresh token" });

            return Ok(tokens);
        }

        // POST: api/v1/Auth/revoke
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto dto)
        {
            await _tokenService.RevokeRefreshTokenAsync(dto.RefreshToken);
            return Ok(new { Message = "Token revoked successfully" });
        }
    }
}