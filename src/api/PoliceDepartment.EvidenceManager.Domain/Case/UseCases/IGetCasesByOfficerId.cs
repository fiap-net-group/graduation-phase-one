namespace PoliceDepartment.EvidenceManager.SharedKernel.Case.UseCases
{
    public interface IGetCasesByOfficerId<TResult>
    {
        Task<TResult> RunAsync(Guid officerId, CancellationToken cancellationToken);
    }
}
