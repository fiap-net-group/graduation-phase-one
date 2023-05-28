namespace PoliceDepartment.EvidenceManager.Domain.Case.UseCases
{
    public interface IGetCasesByOfficerId<TResult>
    {
        Task<TResult> RunAsync(Guid officerId, CancellationToken cancellationToken);
    }
}
