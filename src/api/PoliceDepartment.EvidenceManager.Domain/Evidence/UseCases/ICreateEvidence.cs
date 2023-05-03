namespace PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases
{
    public interface ICreateEvidence<TViewModel, TResult>
    {
        Task<TResult> RunAsync(TViewModel evidence, CancellationToken cancellationToken);
    }
}
