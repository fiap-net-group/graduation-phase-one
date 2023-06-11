using System.ComponentModel.DataAnnotations;

namespace PoliceDepartment.EvidenceManager.MVC.Models
{
    public sealed class LoginModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "{0} is invalid")]
        public string Username { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Password { get; set; }
    }
}
