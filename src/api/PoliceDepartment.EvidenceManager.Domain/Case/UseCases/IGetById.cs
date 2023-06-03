namespace PoliceDepartment.EvidenceManager.Domain.Case.UseCases
{
    public interface IGetById<TResult>
    {
        Task<TResult> RunAsync(Guid officerId, CancellationToken cancellationToken);
    }
}
