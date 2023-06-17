using PoliceDepartment.EvidenceManager.Domain.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using PoliceDepartment.EvidenceManager.SharedKernel.ViewModels;

namespace PoliceDepartment.EvidenceManager.Application.Case.UseCases
{
    public class UpdateCase : IUpdateCase<CaseViewModel, BaseResponse>
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _uow;
        private readonly BaseResponse _response;

        public UpdateCase(ILoggerManager logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _response = new();
        }

        public async Task<BaseResponse> RunAsync(Guid id, Guid officerId, CaseViewModel parameter, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin Update case", ("id", id));

            var entity = await _uow.Case.GetByIdAsync(id, cancellationToken);

            if (!entity.Exists())
            {
                _logger.LogWarning("Case doesn't exists to update", ("id", id));

                return _response.AsError(ResponseMessage.CaseDontExists);
            }

            if (entity.OfficerId != officerId)
            {
                _logger.LogWarning("Officer is not the case owner", ("id", id), ("officerId", officerId));

                return _response.AsError(ResponseMessage.Forbidden);
            }

            if (!entity.Update(parameter.Name, parameter.Description))
            {
                _logger.LogWarning("Invalid case properties", ("id", id));

                return _response.AsError(ResponseMessage.InvalidCase);
            }

            await _uow.Case.UpdateAsync(entity, cancellationToken);

            if (!await _uow.SaveChangesAsync(cancellationToken))
            {
                _logger.LogError("An error ocurred at the database", default, ("id", id));

                throw new InfrastructureException("An unexpected error ocurred");
            }

            _logger.LogDebug("Success Update case", ("id", id));

            return _response.AsSuccess();
        }
    }
}
