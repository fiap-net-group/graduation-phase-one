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

            CreateMap<CreateEvidenceViewModel, EvidenceEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.ImageId))
                .ForMember(dest => dest.Case, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ReverseMap();
        }
    }
}
