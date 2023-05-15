using Demo_Elmah.Identity;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Demo_Elmah.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AccountServices(UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
        }
        /// <summary>
        /// Authentication 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="AuthenticationException"></exception>
        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            try
            {


                AuthenticationResponse response = new AuthenticationResponse();
                if (request.Email == null)
                {
                    response.Message = $"Credentials aren't valid. ";
                    return response;
                }
                var user = await _userManager.FindByEmailAsync(request.Email);


                if (user == null)
                {
                    response.IsAuthenticated = false;
                    response.Message = $"Credentials aren't valid. ";
                    return response;
                }
                //Authentication with Database.....Do we have do that with AD?
                var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    throw new AuthenticationException($"Credentials aren't valid'.");
                }


                //JWT Token
                JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

                //if (user.RefreshTokens == null)
                //{
                //    var refreshToken = GenerateRefreshToken();
                //    response.RefreshToken = refreshToken.Token;
                //    response.RefreshTokenExpiration = refreshToken.Expires;
                //    user.RefreshTokens.Add(refreshToken);
                //    await _userManager.UpdateAsync(user);
                //}
                //else if (user.RefreshTokens.Any(a => a.IsActive))
                //{
                //    var activeRefreshToken = user.RefreshTokens.FirstOrDefault(a => a.IsActive);
                //    response.RefreshToken = activeRefreshToken.Token;
                //    response.RefreshTokenExpiration = activeRefreshToken.Expires;
                //}
                //else
                //{
                //    var refreshToken = GenerateRefreshToken();
                //    response.RefreshToken = refreshToken.Token;
                //    response.RefreshTokenExpiration = refreshToken.Expires;
                //    user.RefreshTokens.Add(refreshToken);
                //    await _userManager.UpdateAsync(user);
                //}

                response.IsAuthenticated = true;
                response.Id = user.Id;
                response.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                response.Email = user.Email;
                response.UserName = user.UserName;

                return response;
            }
            catch (Exception ex)
            {
                AuthenticationResponse response = new AuthenticationResponse();
                response.IsAuthenticated = false;
                response.Email = ex.Message;
                return response;
            }
        }

        public async Task<AddUserResponse> RegisterAsync(AddUserRequest request)
        {
            var existingUser = await _userManager.FindByNameAsync(request.UserName);

            if (existingUser != null)
            {
                throw new ArgumentException($"Username '{request.UserName}' already exists.");
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                EmailConfirmed = true
            };

            var existingEmail = await _userManager.FindByEmailAsync(request.Email);

            if (existingEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, "Viewer");
                    return new AddUserResponse() { UserId = user.Id };
                }
                else
                {
                    return new AddUserResponse() { UserId = $"{result.Errors.ToString()}" };
                }
            }
            else
            {
                return new AddUserResponse() { UserId = "email already registered" };
            }
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            try
            {


                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                var roleClaims = new List<Claim>();

                for (int i = 0; i < roles.Count; i++)
                {
                    roleClaims.Add(new Claim("roles", roles[i]));
                }

                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
                .Union(userClaims)
                .Union(roleClaims);

                var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                    signingCredentials: signingCredentials);
                return jwtSecurityToken;
            }
            catch (Exception ex)
            {
                return new JwtSecurityToken(ex.Message);
            }
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(10),
                    Created = DateTime.UtcNow
                };
            }
        }


        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var response = new RefreshTokenResponse();
            var user = _userManager.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == request.Token));
            if (user == null)
            {
                response.IsAuthenticated = false;
                response.Message = $"Token did not match any users.";
                return response;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == request.Token);

            if (!refreshToken.IsActive)
            {
                response.IsAuthenticated = false;
                response.Message = $"Token Not Active.";
                return response;
            }

            //Revoke Current Refresh Token
            refreshToken.Revoked = DateTime.UtcNow;

            //Generate new Refresh Token and save to Database
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            //Generates new jwt
            response.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);
            response.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;
            response.RefreshToken = newRefreshToken.Token;
            response.RefreshTokenExpiration = newRefreshToken.Expires;
            return response;
        }

        public async Task<RevokeTokenResponse> RevokeToken(RevokeTokenRequest request)
        {
            var response = new RevokeTokenResponse();
            if (string.IsNullOrEmpty(request.Token))
            {
                response.IsRevoked = false;
                response.Message = "Token is required";
                return response;
            }

            var user = _userManager.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == request.Token));

            if (user == null)
            {
                response.IsRevoked = false;
                response.Message = "Token did not match any users";
                return response;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == request.Token);
            if (!refreshToken.IsActive)
            {
                response.IsRevoked = false;
                response.Message = "Token is not active";
                return response;
            }
            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            response.IsRevoked = true;
            response.Message = "Token revoked";
            return response;
        }

    }
}
