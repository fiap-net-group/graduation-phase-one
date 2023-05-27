using PoliceDepartment.EvidenceManager.Application.Authorization;
using PoliceDepartment.EvidenceManager.Application.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Application.Case.UseCases;
using PoliceDepartment.EvidenceManager.Application.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Case.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules
{
    [ExcludeFromCodeCoverage]
    internal static class ApplicationExtensions
    {
        internal static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
        {
            services.AddScoped<ILogin<LoginViewModel, BaseResponseWithValue<AccessTokenModel>>, Login>();

            services.AddScoped<IGetCasesByOfficerId<BaseResponseWithValue<IEnumerable<CaseViewModel>>>, GetCaseByOfficerId>();
            services.AddScoped<IGetById<BaseResponseWithValue<CaseViewModel>>, GetCaseById>();
            services.AddScoped<IUpdateCase<CaseViewModel, BaseResponse>, UpdateCase>();
            services.AddScoped<IDeleteCase<BaseResponse>, DeleteCase>();

            services.AddAutoMapper(typeof(EvidenceMapperProfile));

            return services;
        }
    }
}
