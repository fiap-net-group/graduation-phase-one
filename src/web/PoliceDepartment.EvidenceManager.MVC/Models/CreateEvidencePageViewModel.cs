using System.ComponentModel.DataAnnotations;

namespace PoliceDepartment.EvidenceManager.MVC.Models
{
    public class CreateEvidencePageViewModel
    {
        public CreateEvidencePageViewModel()
        {
            ImageId = Guid.NewGuid();
        }

        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public string Description { get; set; }
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public Guid CaseId { get; set; }
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public Guid OfficerId { get; set; }
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public IFormFile Image { get; set; }
        public Guid ImageId { get; set; }
    }
}
