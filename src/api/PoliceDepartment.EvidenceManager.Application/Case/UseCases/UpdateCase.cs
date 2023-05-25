using PoliceDepartment.EvidenceManager.Domain.Case;
using PoliceDepartment.EvidenceManager.Domain.Case.UseCases;
using PoliceDepartment.EvidenceManager.Domain.Database;
using PoliceDepartment.EvidenceManager.Domain.Exceptions;
using PoliceDepartment.EvidenceManager.Domain.Logger;
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

        public async Task<BaseResponse> RunAsync(Guid id, CaseViewModel parameter, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Begin Update case", ("id", id));

            var entity = await _uow.Case.GetByIdAsync(id, cancellationToken);

            if(!entity.Exists())
            {
                _logger.LogWarning("Case doesn't exists to update", ("id", id));

                return _response.AsError(ResponseMessage.CaseDontExists);
            }

            if(!entity.Update())
            {
                _logger.LogWarning("Invalid case properties", ("id", id));

                return _response.AsError(ResponseMessage.InvalidCase);
            }

            await _uow.Case.UpdateAsync(entity, cancellationToken);

            if(!await _uow.SaveChangesAsync())
            {
                _logger.LogError("An error ocurred at the database");

                throw new InfrastructureException("An unexpected error ocurred");
            }

            _logger.LogDebug("Success Update case", ("id", id));

            return _response.AsSuccess();
        }
    }
}
