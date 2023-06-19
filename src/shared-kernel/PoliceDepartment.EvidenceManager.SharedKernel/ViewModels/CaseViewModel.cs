namespace PoliceDepartment.EvidenceManager.SharedKernel.ViewModels
{
    public class CaseViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid OfficerId { get; set; }
        public IEnumerable<EvidenceViewModel> Evidences { get; set; }

        public bool Valid()
        {
            return Id != Guid.Empty;
        }
    }
}
