using Microsoft.Extensions.Configuration;
using PoliceDepartment.EvidenceManager.Domain.Authorization;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;

namespace PoliceDepartment.EvidenceManager.Application.Authorization.UseCases
{
    public class CreateAdminIfNeeded
    {
        private readonly ILoggerManager _logger;
        private readonly IIdentityManager _identityManager;
        private readonly IConfiguration _configuration;

        public CreateAdminIfNeeded(ILoggerManager logger,
                                   IIdentityManager identityManager,
                                   IConfiguration configuration)
        {
            _logger = logger;
            _identityManager = identityManager;
            _configuration = configuration;
        }

        public async Task RunAsync()
        {
            _logger.LogDebug("Begin Create admin if needed");

            if (await _identityManager.FindByEmailAsync(_configuration["Admin:Email"]) is not null)
            {
                _logger.LogDebug("Admin exists");

                return;
            }

            var response = await _identityManager.CreateAsync(_configuration["Admin:Email"],
                                                              _configuration["Admin:Email"],
                                                              _configuration["Admin:Password"],
                                                              Enum.GetName(OfficerType.Administrator));

            if (!response.Succeeded)
            {
                _logger.LogError("Error creating admin", default, (nameof(response), response));

                return;
            }

            _logger.LogDebug("Success Create admin if needed");
        }
    }
}
