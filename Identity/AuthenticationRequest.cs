namespace Demo_Elmah.Identity
{
    public class AuthenticationRequest
    {
        //get the data from React
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
