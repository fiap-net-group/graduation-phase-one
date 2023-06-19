namespace PoliceDepartment.EvidenceManager.SharedKernel.Authorization.UseCases
{
    public interface ILogin<TViewModel,TResult>
    {
        Task<TResult> RunAsync(TViewModel viewModel, CancellationToken cancellationToken);
    }
}
