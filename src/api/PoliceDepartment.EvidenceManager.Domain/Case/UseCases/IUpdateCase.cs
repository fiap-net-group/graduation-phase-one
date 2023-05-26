namespace PoliceDepartment.EvidenceManager.Domain.Case.UseCases
{
    public interface IUpdateCase<TViewModel, TResponse>
    {
        Task<TResponse> RunAsync(Guid id, TViewModel parameter, CancellationToken cancellationToken);
    }
}
