namespace PoliceDepartment.EvidenceManager.SharedKernel.Authorization.UseCases
{
    public interface ILogOut<TViewModel, TResult>
    {
        Task<TResult> RunAsync(Guid userId,CancellationToken cancellationToken);
    }
}