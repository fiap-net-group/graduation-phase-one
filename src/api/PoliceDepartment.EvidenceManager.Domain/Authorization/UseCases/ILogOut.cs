namespace PoliceDepartment.EvidenceManager.Domain.Authorization.UseCases
{
    public interface ILogOut<TViewModel, TResult>
    {
        Task<TResult> RunAsync(Guid userId,CancellationToken cancellationToken);
    }
}