namespace PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces
{
    public interface IOfficerUser
    {
        bool IsAuthenticated { get; }
        HttpContext HttpContext { get; }
        string Name { get; }
        Guid Id { get; }
        string AccessToken { get; }
    }
}
