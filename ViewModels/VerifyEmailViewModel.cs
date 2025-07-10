using System.ComponentModel.DataAnnotations;

namespace RoleBasedAuthorization.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;
    }
}
