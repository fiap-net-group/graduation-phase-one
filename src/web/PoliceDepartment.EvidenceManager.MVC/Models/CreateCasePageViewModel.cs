using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PoliceDepartment.EvidenceManager.MVC.Models
{
    public class CreateCasePageViewModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public string Name { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public string Description { get; set; }
    }
}
