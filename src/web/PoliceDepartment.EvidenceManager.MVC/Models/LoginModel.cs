using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PoliceDepartment.EvidenceManager.MVC.Models
{
    public sealed class LoginModel
    {
        [DisplayName("Username")]
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "{0} is invalid")]
        public string Username { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
