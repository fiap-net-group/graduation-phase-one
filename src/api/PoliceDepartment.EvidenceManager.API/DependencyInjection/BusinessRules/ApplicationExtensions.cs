using FluentValidation;
using PoliceDepartment.EvidenceManager.Application.Authorization;
using PoliceDepartment.EvidenceManager.Application.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Application.Officer;
using PoliceDepartment.EvidenceManager.Application.Officer.UseCases;
using PoliceDepartment.EvidenceManager.Application.Case;
using PoliceDepartment.EvidenceManager.Application.Case.UseCases;
using PoliceDepartment.EvidenceManager.Application.Evidence;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Authorization.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Case.UseCases;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;
using System.Diagnostics.CodeAnalysis;
using PoliceDepartment.EvidenceManager.Domain.Officer.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.API.Application.Evidence;

namespace PoliceDepartment.EvidenceManager.API.DependencyInjection.BusinessRules
{
    [ExcludeFromCodeCoverage]
    internal static class ApplicationExtensions
    {
        internal static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
        {
            services.AddScoped<ILogin<LoginViewModel, BaseResponseWithValue<AccessTokenModel>>, Login>();
            services.AddScoped<ILogOut<LogOutViewModel, BaseResponse>, LogOut>();

            services.AddScoped<ICreateOfficer<CreateOfficerViewModel, BaseResponse>, Officer>();

            services.AddScoped<IValidator<CreateOfficerViewModel>, OfficerValidator>();

            services.AddAutoMapper(typeof(OfficerMapperProfile));

            services.AddScoped<IGetCasesByOfficerId<BaseResponseWithValue<IEnumerable<CaseViewModel>>>, GetCaseByOfficerId>();
            services.AddScoped<IGetById<BaseResponseWithValue<CaseViewModel>>, GetCaseById>();
            services.AddScoped<IUpdateCase<CaseViewModel, BaseResponse>, UpdateCase>();
            services.AddScoped<IDeleteCase<BaseResponse>, DeleteCase>();
            services.AddScoped<ICreateCase<CreateCaseViewModel, BaseResponse>, CreateCase>();
            services.AddScoped<ICreateEvidence<CreateEvidenceViewModel, BaseResponse>, CreateEvidence>();

            services.AddScoped<IGetEvidenceById<BaseResponseWithValue<EvidenceViewModel>>, GetEvidenceById>();

            services.AddScoped<IValidator<CreateCaseViewModel>, CaseValidator>();
            services.AddScoped<IValidator<CreateEvidenceViewModel>, EvidenceValidator>();

            services.AddAutoMapper(typeof(EvidenceMapperProfile));

            services.AddScoped<CreateAdminIfNeeded>();

            return services;
        }
    }
}
