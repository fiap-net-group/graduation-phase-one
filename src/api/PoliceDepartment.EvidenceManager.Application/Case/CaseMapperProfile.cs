using AutoMapper;
using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Case
{
    public class CaseMapperProfile : Profile
    {
        public CaseMapperProfile()
        {
           // CreateMap<CaseEntity, CaseViewModel>().ReverseMap();

            CreateMap<CaseEntity, CaseViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(date => DateTime.Now))
                .ForMember(dest => dest.Evidences, opt => opt.Ignore())
                .ReverseMap();
        }               
    }
}
