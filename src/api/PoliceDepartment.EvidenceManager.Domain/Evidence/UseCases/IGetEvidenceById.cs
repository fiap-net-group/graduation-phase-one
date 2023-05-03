namespace PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases
{
    public interface IGetEvidenceById<TResult>
    {
        Task<TResult> RunAsync(Guid id, CancellationToken cancellationToken);
    }
}
