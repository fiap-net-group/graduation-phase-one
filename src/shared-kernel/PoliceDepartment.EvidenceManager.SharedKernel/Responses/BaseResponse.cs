using PoliceDepartment.EvidenceManager.SharedKernel.Extensions;
using System.Runtime.Serialization;

namespace PoliceDepartment.EvidenceManager.SharedKernel.Responses
{
    public class BaseResponse
    {
        private ResponseMessage _responseMessage;
        public bool Success { get; set; }
        public ResponseDetails ResponseDetails { get; set; }

        public BaseResponse AsError(ResponseMessage? message = null, params string[] errors)
        {
            errors ??= Array.Empty<string>();
            Success = false;
            _responseMessage = message ?? ResponseMessage.GenericError;
            ResponseDetails = new ResponseDetails
            {
                Message = _responseMessage.GetDescription(),
                Errors = errors
            };
            return this;
        }

        public BaseResponse AsSuccess()
        {
            Success = true;
            _responseMessage = ResponseMessage.Success;
            ResponseDetails = new ResponseDetails
            {
                Message = _responseMessage.GetDescription(),
                Errors = default
            };
            return this;
        }

        public bool ResponseMessageEqual(ResponseMessage compare)
        {
            return _responseMessage == compare;
        }
    }
}