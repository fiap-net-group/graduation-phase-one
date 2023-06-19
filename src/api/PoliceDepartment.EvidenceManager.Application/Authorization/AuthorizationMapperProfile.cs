using AutoMapper;
using PoliceDepartment.EvidenceManager.SharedKernel.Authorization;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Authorization
{
    public class AuthorizationMapperProfile : Profile
    {
        public AuthorizationMapperProfile()
        {
            CreateMap<AccessTokenModel, AccessTokenViewModel>().ReverseMap();
        }
    }
}
