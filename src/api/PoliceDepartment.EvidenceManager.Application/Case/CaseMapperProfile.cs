using AutoMapper;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Case
{
    public class CaseMapperProfile : Profile
    {
        public CaseMapperProfile()
        {
            CreateMap<CaseEntity, CaseViewModel>().ReverseMap();
        }
    }
}
