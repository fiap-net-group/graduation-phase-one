using System.ComponentModel.DataAnnotations;

namespace PoliceDepartment.EvidenceManager.MVC.Models
{
    public class EditEvidencePageViewModel
    {
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} is required", AllowEmptyStrings = false)]
        public string Description { get; set; }

        public IFormFile Image { get; set; }

        public EditEvidencePageViewModel(EvidenceDetailViewModel details)
        {
            Id = details.Id;
            Name = details.Name;
            Description = details.Description;
        }
    }
}
