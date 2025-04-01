using System.ComponentModel.DataAnnotations;

namespace LCMS.Blazor.ViewModels
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide UserName.")]
        [MaxLength(100)]
        public string? UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please provide Password.")]
        [MaxLength(100)]
        public string? Password { get; set; }
    }
}
