using Demo_Elmah.Identity;
using Demo_Elmah.Identity.Roles;
using Demo_Elmah.Identity.User;

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
        Task<object> GetRolesAsync();
        Task<GetUserByEmailResponse> GetUserByIdAsync(string id);
        Task<GetUserByEmailResponse> GetUserByEmailAsync(string email);

        Task<object> GetUsersAsync();



    }
}