namespace PoliceDepartment.EvidenceManager.SharedKernel.Officer.UseCases
{
    public interface ICreateOfficer<TViewModel,TResult>
    {
        Task<TResult> RunAsync(TViewModel viewModel,CancellationToken cancellationToken);
    }
}