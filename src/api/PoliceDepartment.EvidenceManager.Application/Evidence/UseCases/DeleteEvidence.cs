using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;

namespace PoliceDepartment.EvidenceManager.API.Application.Evidence.UseCases
{
    public class DeleteEvidence : IDeleteEvidence<BaseResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly BaseResponse _response;
        private readonly ILoggerManager _logger;

        public DeleteEvidence(IUnitOfWork uow, ILoggerManager logger)
        {
            _uow = uow;
            _response = new BaseResponse();
            _logger = logger;
        }
        public async Task<BaseResponse> RunAsync(Guid id,Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin Delete evidence", ("id", id), ("userId", userId));

            var evidence = await _uow.Evidence.GetByIdAsync(id, cancellationToken);

            if(!evidence.Exists()){
                _logger.LogWarning("Evidence doesn't exists to delete", ("id", id), ("userId", userId));

                return _response.AsError(ResponseMessage.EvidenceDontExists);
            }

            var @case = await _uow.Case.GetByIdAsync(evidence.CaseId, cancellationToken);

            if(@case.OfficerId != userId){
                _logger.LogWarning("Officer is not the evidence owner", ("id", id), ("userId", userId));

                return _response.AsError(ResponseMessage.Forbidden);
            }

            await _uow.BeginTransactionAsync(cancellationToken);

            await _uow.Evidence.DeleteByIdAsync(evidence.Id);

            if(!await _uow.SaveChangesAsync(cancellationToken) || !await _uow.CommmitAsync(cancellationToken)){
                _logger.LogError("An error ocurred at the database", default, ("id", id), ("userId", userId));

                throw new InfrastructureException("An unexpected error ocurred");
            }

            _logger.LogDebug("Success Delete evidence", ("id", id), ("userId", userId));

            return _response.AsSuccess();
        }
    }
}
