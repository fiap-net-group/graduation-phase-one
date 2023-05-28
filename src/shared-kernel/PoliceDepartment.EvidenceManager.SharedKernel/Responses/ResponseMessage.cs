using System.ComponentModel;

namespace PoliceDepartment.EvidenceManager.SharedKernel.Responses
{
    public enum ResponseMessage
    {
        [Description("An error ocurred")]
        GenericError = 0,
        [Description("Success")]
        Success = 1,
        [Description("Case does't exists")]
        CaseDontExists = 2,
        [Description("Invalid case")]
        InvalidCase = 3,
        [Description("Invalid credentials")]
        InvalidCredentials = 4
    }
}