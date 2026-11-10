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

        public AuthController(
            UserService userService,
            IPasswordHasher<UserEntity> passwordHasher)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
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
    }
}
