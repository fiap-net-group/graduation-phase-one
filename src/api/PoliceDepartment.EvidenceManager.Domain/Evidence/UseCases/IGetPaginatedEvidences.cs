namespace PoliceDepartment.EvidenceManager.SharedKernel.Evidence.UseCases
{
    public interface IGetPaginatedEvidences<TResult>
    {
        Task<TResult> RunAsync(int page, int rows, CancellationToken cancellationToken);
    }
}
