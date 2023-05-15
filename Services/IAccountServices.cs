using Demo_Elmah.Identity;

namespace Demo_Elmah.Services
{
    public interface IAccountServices
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<AddUserResponse> RegisterAsync(AddUserRequest request);
        Task<RevokeTokenResponse> RevokeToken(RevokeTokenRequest request);
    }
}