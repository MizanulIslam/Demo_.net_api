using Microsoft.AspNetCore.Identity;

namespace Demo_Elmah.Identity.User
{
    public class GetUserByEmailResponse
    {
        public string Email { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<IdentityRole> Roles { get; set; }
    }
}
