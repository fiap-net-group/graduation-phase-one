using FluentValidation;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Case
{
    public class CaseValidator : AbstractValidator<CaseViewModel>
    {
        public CaseValidator()
        {
            RuleFor(c => c.Name).Cascade(CascadeMode.Continue).NotEmpty();     
            
            RuleFor(c => c.OfficerId).NotEmpty();
        }
    }
}
