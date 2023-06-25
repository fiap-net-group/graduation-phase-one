using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.MVC.Models
{
    public class EvidenceDetailViewModel
    {
        public Guid Id { get; set; }
        public string CaseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageId { get; set; }
        public string ImageUrl { get; set; }

        public EvidenceDetailViewModel() { }

        public EvidenceDetailViewModel(EvidenceViewModel viewModel, string imageUrl)
        {
            Id = viewModel.Id;
            CaseId = viewModel.CaseId;
            Name = viewModel.Name;
            Description = viewModel.Description;
            ImageId = viewModel.ImageId;
            ImageUrl = imageUrl;
        }

        public bool Valid()
        {
            return Id != Guid.Empty;
        }
    }
}
