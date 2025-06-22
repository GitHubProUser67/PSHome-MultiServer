using System.ComponentModel.DataAnnotations;

namespace AlcatrazService.DTO
{
    public class UserRegisterModel
    {
        public string? Username { get; set; }

        public string? Password { get; set; }

        [MaxLength(14, ErrorMessage = "Nickname can't be longer than 14 characters (sorry)")]
        public string? PlayerNickName { get; set; }
    }
}
