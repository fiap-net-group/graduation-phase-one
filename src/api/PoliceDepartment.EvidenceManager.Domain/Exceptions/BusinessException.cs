using System.Runtime.Serialization;

namespace PoliceDepartment.EvidenceManager.Domain.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        public BusinessException() { }

        public BusinessException(string message) : base(message) { }

        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        public static void ThrowIfNull<T>(T obj)
        {
            if (obj is null)
                throw new BusinessException("Invalid parameter");
        }
    }
}
