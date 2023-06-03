using FluentValidation;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Officer
{
    public class OfficerValidator : AbstractValidator<CreateOfficerViewModel>
    {
        public OfficerValidator(){
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email is required");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.OfficerType).NotNull().WithMessage("OfficerType is required");
        }
        
    }
}