using AutoMapper;
using PoliceDepartment.EvidenceManager.Domain.Evidence;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Evidence
{
    public class EvidenceMapperProfile : Profile
    {
        public EvidenceMapperProfile()
        {
            CreateMap<EvidenceEntity, EvidenceViewModel>().ReverseMap();
        }
    }
}
