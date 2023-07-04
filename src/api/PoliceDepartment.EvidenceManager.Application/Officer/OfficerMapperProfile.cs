using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PoliceDepartment.EvidenceManager.SharedKernel.Officer;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Officer
{
    public class OfficerMapperProfile : Profile
    {
        public OfficerMapperProfile(){
            CreateMap<OfficerEntity, CreateOfficerViewModel>();
            CreateMap<OfficerEntity, IdentityUser>();
            CreateMap<IdentityUser, OfficerEntity>();
            CreateMap<CreateOfficerViewModel, OfficerEntity>();
        }
    }
}