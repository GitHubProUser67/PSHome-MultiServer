using System.Collections.Generic;

namespace Horizon.LIBRARY.Database.Models
{
    public class AuthenticationRequest
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticationResponse
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
    }
}