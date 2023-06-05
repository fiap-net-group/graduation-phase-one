namespace PoliceDepartment.EvidenceManager.SharedKernel.ViewModels
{
    public class CreateEvidenceViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CaseId { get; set; }
        public Guid ImageId { get; set; }
    }
}
