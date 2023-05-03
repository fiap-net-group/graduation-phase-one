namespace PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases
{
    public interface IDeleteEvidence<TResult>
    {
        Task<TResult> RunAsync(Guid id, CancellationToken cancellationToken);
    }
}
