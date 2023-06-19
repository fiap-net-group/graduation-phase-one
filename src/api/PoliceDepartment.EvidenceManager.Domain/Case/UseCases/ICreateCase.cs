namespace PoliceDepartment.EvidenceManager.SharedKernel.Case.UseCases
{
    public interface ICreateCase<TViewModel, TResponse>
    {
        Task<TResponse> RunAsync(TViewModel @case, CancellationToken cancellationToken);
    }
}
