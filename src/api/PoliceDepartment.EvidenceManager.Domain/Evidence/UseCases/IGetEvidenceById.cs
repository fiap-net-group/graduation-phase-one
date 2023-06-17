namespace PoliceDepartment.EvidenceManager.SharedKernel.Evidence.UseCases
{
    public interface IGetEvidenceById<TResult>
    {
        Task<TResult> RunAsync(Guid id, CancellationToken cancellationToken);
    }
}
