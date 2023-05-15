using Demo_Elmah.Identity;
using Demo_Elmah.Identity.Roles;
using Demo_Elmah.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo_Elmah.Controllers
{
    //[Route("api/[controller]")]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountService;
        public AccountController(IAccountServices accountServices)
        {
            _accountService = accountServices;
        }
        /// <summary>
        /// Get User List
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[HttpGet("getalluser")]
        //public async Task<ActionResult<AddUserResponse>> GetAllUserAsync(AddUserRequest request)
        //{
        //    //return Ok(await _accountService.GetAllUserAsync(request));
        //}

        /// <summary>
        /// get user by email include roles
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[HttpGet("getuserbyemail")]
        //public async Task<ActionResult<AddUserResponse>> GetUserbyEmailAsync(AddUserRequest request)
        //{
        //    return Ok(await _accountService.GetUserByEmailAsync(request));
        //}

        /// <summary>
        /// get all role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[HttpGet("getrole")]
        //public async Task<ActionResult<AddUserResponse>> GetRolesAsync(AddUserRequest request)
        //{
        //    return Ok(await _accountService.GetRolesAsync(request));
        //}
        //test comment
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _accountService.AuthenticateAsync(request));
        }

        [HttpPost("adduser")]
        public async Task<ActionResult<AddUserResponse>> RegisterAsync(AddUserRequest request)
        {
            return Ok(await _accountService.RegisterAsync(request));
        }

        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest request)
        //{
        //    return Ok(await _accService.RefreshTokenAsync(request));
        //}

        //[HttpPost("revoke-token")]
        //public async Task<IActionResult> RevokeTokenAsync(RevokeTokenRequest request)
        //{
        //    var response = await _accService.RevokeToken(request);
        //    if (response.Message == "Token is required")
        //        return BadRequest(response);
        //    else if (response.Message == "Token did not match any users")
        //        return NotFound(response);
        //    else
        //        return Ok(response);
        //}
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync(AddRoleRequest request)
        {
            return Ok(await _accountService.AddRoleToDBAsync(request));
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleToUserRequest request)
        {
            return Ok(await _accountService.AddRoletoUserAsync(request));
        }
    }
}
