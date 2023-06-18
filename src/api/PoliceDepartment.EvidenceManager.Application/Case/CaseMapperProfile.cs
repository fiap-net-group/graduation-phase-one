using AutoMapper;
using PoliceDepartment.EvidenceManager.SharedKernel.Case;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Case
{
    public class CaseMapperProfile : Profile
    {
        public CaseMapperProfile()
        {
            CreateMap<CaseViewModel, CaseEntity>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Officer, opt => opt.Ignore())
                .ForMember(dest => dest.OfficerId, opt => opt.MapFrom(src => src.OfficerId))
                .ForMember(dest => dest.Evidences, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CreateCaseViewModel, CaseEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.Officer, opt => opt.Ignore())
                .ForMember(dest => dest.Evidences, opt => opt.Ignore())
                .ForMember(dest => dest.OfficerId, opt => opt.MapFrom(src => src.OfficerId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ReverseMap();

        }
    }
}
