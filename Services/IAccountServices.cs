using Demo_Elmah.Identity;
using Demo_Elmah.Identity.Roles;

namespace Demo_Elmah.Services
{
    public interface IAccountServices
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
        //Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<AddUserResponse> RegisterAsync(AddUserRequest request);
        //Task<RevokeTokenResponse> RevokeToken(RevokeTokenRequest request);

        Task<string> AddRoletoUserAsync(AssignRoleToUserRequest request);
        Task<string> AddRoleToDBAsync(AddRoleRequest request);

    }
}