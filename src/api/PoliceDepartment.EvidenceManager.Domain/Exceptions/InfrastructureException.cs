using System.Runtime.Serialization;

namespace PoliceDepartment.EvidenceManager.SharedKernel.Exceptions
{
    [Serializable]
    public class InfrastructureException : Exception
    {
        public InfrastructureException(string message) : base(message) { }
        protected InfrastructureException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
