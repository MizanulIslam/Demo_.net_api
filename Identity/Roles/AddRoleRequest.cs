using System.ComponentModel.DataAnnotations;

namespace Demo_Elmah.Identity.Roles
{
    public class AddRoleRequest
    {
       
        public string Role { get; set; }
        public List<string> Roles { get; set; }

    }
}
