
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoffeeNChill.Gateway.Controllers
{
    [ApiController]
    [Route("api/testauth")]
    public class TestAuthController : ControllerBase
    {
        [HttpGet("public")]
        [AllowAnonymous]
        public IActionResult Public()
        {
            return Ok(new
            {
                message = "This endpoint is public."
            });
        }

        [HttpGet("customer")]
        [Authorize]
        public IActionResult Customer()
        {
            return Ok(new
            {
                message = "You are authenticated.",
                name = User.FindFirstValue(ClaimTypes.Name),
                role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        [HttpGet("staff")]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Staff()
        {
            return Ok(new
            {
                message = "Welcome, authorised staff member!"
            });
        }
    }
}
