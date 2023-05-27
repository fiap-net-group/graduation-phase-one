using FluentValidation;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Case
{
    public class CaseValidator : AbstractValidator<CaseViewModel>
    {
        public CaseValidator()
        {
            RuleFor(c => c.Name).Cascade(CascadeMode.Continue).NotEmpty().WithMessage("Name is required");     
            
            RuleFor(c => c.OfficerId).NotEmpty().WithMessage("OfficerId is required");
            RuleFor(c => c.Description).NotEmpty().WithMessage("Description is required");
        }
    }
}
