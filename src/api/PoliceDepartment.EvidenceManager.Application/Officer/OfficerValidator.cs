using FluentValidation;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Officer
{
    public class OfficerValidator : AbstractValidator<CreateOfficerViewModel>
    {
        public OfficerValidator(){
            RuleFor(x => x.Email).Cascade(CascadeMode.Continue)
                                 .NotEmpty()
                                 .EmailAddress()
                                 .WithMessage("Email is required");

            RuleFor(x => x.UserName).Cascade(CascadeMode.Continue)
                                    .NotEmpty()
                                    .WithMessage("UserName is required");

            RuleFor(x => x.Password).Cascade(CascadeMode.Continue)
                                    .NotEmpty()
                                    .WithMessage("Password is required");

            RuleFor(x => x.OfficerType).Cascade(CascadeMode.Continue)
                                       .NotNull()
                                       .WithMessage("OfficerType is required");
        }
        
    }
}