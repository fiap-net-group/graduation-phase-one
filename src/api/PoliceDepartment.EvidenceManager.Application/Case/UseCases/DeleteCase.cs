using PoliceDepartment.EvidenceManager.Domain.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.Application.Case.UseCases
{
    public class DeleteCase: IDeleteCase<BaseResponse>
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _uow;
        private readonly BaseResponse _response;

        public DeleteCase(ILoggerManager logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
            _response = new();
        }

        public async Task<BaseResponse> RunAsync(Guid id, Guid officerId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin Delete case", ("id", id), ("officerId", officerId));

            var entity = await _uow.Case.GetByIdAsync(id, cancellationToken);

            if (!entity.Exists())
            {
                _logger.LogWarning("Case doesn't exists to update", ("id", id), ("officerId", officerId));

                return _response.AsError(ResponseMessage.CaseDontExists);
            }

            if (entity.OfficerId != officerId)
            {
                _logger.LogWarning("Officer is not the case owner", ("id", id), ("officerId", officerId));

                return _response.AsError(ResponseMessage.Forbidden);
            }

            await _uow.BeginTransactionAsync(cancellationToken);

            await _uow.Evidence.DeleteByCaseAsync(id, cancellationToken);

            await _uow.Case.DeleteAsync(entity, cancellationToken);

            if (!await _uow.SaveChangesAsync(cancellationToken) || !await _uow.CommmitAsync(cancellationToken))
            {
                _logger.LogError("An error ocurred at the database", default, ("id", id), ("officerId", officerId));

                throw new InfrastructureException("An unexpected error ocurred");
            }

            _logger.LogDebug("Success Delete case", ("id", id), ("officerId", officerId));

            return _response.AsSuccess();
        }
    }
}
