namespace PoliceDepartment.EvidenceManager.Domain.Case.UseCases
{
    public interface IUpdateCase<TViewModel, TResponse>
    {
        Task<TResponse> RunAsync(Guid id, Guid officerId, TViewModel parameter, CancellationToken cancellationToken);
    }
}
