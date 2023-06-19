namespace PoliceDepartment.EvidenceManager.SharedKernel.Evidence.UseCases
{
    public interface ICreateEvidence<TViewModel, TResult>
    {
        Task<TResult> RunAsync(TViewModel evidence, CancellationToken cancellationToken);
    }
}
