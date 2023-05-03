namespace PoliceDepartment.EvidenceManager.Domain.Evidence.UseCases
{
    public interface IUpdateEvidence<TViewModel, TResult>
    {
        Task<TResult> RunAsync(Guid id, TViewModel enterprise, CancellationToken cancellationToken);
    }
}
