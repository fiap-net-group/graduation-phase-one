namespace PoliceDepartment.EvidenceManager.SharedKernel.Authorization
{
    public static class AuthorizationPolicies
    {
        public const string IsAdmin = nameof(IsAdmin);
        public const string IsPoliceOfficer = nameof(IsPoliceOfficer);
    }
}
