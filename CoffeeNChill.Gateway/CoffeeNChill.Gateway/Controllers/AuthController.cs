using Azure;
using CoffeeNChill.Gateway.DTOs;
using CoffeeNChill.Gateway.Models;
using CoffeeNChill.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeNChill.Gateway.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(
            UserService userService,
            IPasswordHasher<UserEntity> passwordHasher,
            JwtTokenService jwtTokenService)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest("Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Password is required.");
            }

            var existingUser =
                await _userService.GetUserByEmailAsync(email);

            if (existingUser != null)
            {
                return Conflict("A user with this email already exists.");
            }

            var user = new UserEntity
            {
                PartitionKey = "Customer",
                RowKey = email,
                FullName = request.FullName.Trim(),
                CreatedDate = DateTime.UtcNow
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    request.Password);

            try
            {
                await _userService.CreateUserAsync(user);
            }
            catch (RequestFailedException ex)
                when (ex.Status == 409)
            {
                return Conflict("A user with this email already exists.");
            }

            return StatusCode(201, new
            {
                message = "User registered successfully.",
                email = user.RowKey,
                role = user.PartitionKey
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash =
                    _passwordHasher.HashPassword(user, request.Password);

                await _userService.UpdateUserAsync(user);
            }

            var (token, expiresAt) =
                _jwtTokenService.GenerateToken(user);

            return Ok(new
            {
                token,
                tokenType = "Bearer",
                expiresAt,
                fullName = user.FullName,
                email = user.RowKey,
                role = user.PartitionKey
            });
        }

    }
}
