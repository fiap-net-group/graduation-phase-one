namespace PoliceDepartment.EvidenceManager.MVC.Models
{
    public class EvidenceDetailViewModel
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid ImageId { get; set; }

        public bool Valid()
        {
            return Id != Guid.Empty;
        }
    }
}
