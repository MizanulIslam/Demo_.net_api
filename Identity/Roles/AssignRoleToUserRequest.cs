using System.ComponentModel.DataAnnotations;

namespace Demo_Elmah.Identity.Roles
{
    public class AssignRoleToUserRequest
    {
        [Required]
        public string UserEmail { get; set; }


        public string Role { get; set; }

        public List<string> Roles { get; set; }
    }
}
