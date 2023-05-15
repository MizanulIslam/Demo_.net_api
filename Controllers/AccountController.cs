using Demo_Elmah.Identity;
using Demo_Elmah.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo_Elmah.Controllers
{
    //[Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accService;
        public AccountController(IAccountServices accountServices)
        {
            _accService = accountServices;
        }
        //test comment
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _accService.AuthenticateAsync(request));
        }

        [HttpPost("adduser")]
        public async Task<ActionResult<AddUserResponse>> RegisterAsync(AddUserRequest request)
        {
            return Ok(await _accService.RegisterAsync(request));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest request)
        {
            return Ok(await _accService.RefreshTokenAsync(request));
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeTokenAsync(RevokeTokenRequest request)
        {
            var response = await _accService.RevokeToken(request);
            if (response.Message == "Token is required")
                return BadRequest(response);
            else if (response.Message == "Token did not match any users")
                return NotFound(response);
            else
                return Ok(response);
        }
    }
}
