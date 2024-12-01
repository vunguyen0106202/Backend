using System.ComponentModel.DataAnnotations;

namespace API.Dtos.ModelRequest
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
