namespace PoliceDepartment.EvidenceManager.Domain.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException() { }

        public BusinessException(string message) : base(message) { }

        public static void ThrowIfNull<T>(T obj)
        {
            if (obj is null)
                throw new BusinessException("Invalid parameter");
        }
    }
}
