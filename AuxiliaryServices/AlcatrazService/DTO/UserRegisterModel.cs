using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AlcatrazService.DTO
{
    public class UserRegisterModel
    {
        public string? Username { get; set; }

        public string? Password { get; set; }

        [MaxLength(14, ErrorMessage = "Nickname can't be longer than 14 characters (sorry)")]
        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>"
        )]
        public string? PlayerNickName { get; set; }
    }
}
