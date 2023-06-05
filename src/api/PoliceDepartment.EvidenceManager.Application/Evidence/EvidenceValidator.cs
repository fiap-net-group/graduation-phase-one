using FluentValidation;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.API.Application.Evidence
{
    public class EvidenceValidator : AbstractValidator<CreateEvidenceViewModel>
    {
        public EvidenceValidator()
        {
            RuleFor(c => c.Name).Cascade(CascadeMode.Continue).NotEmpty().WithMessage("Name is required");
            
            RuleFor(c => c.Description).NotEmpty().WithMessage("Description is required");

            RuleFor(c => c.ImageId).NotEmpty().WithMessage("ImageId is required");
        }   
    }
}
