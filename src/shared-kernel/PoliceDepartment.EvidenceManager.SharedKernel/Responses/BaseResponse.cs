namespace PoliceDepartment.EvidenceManager.SharedKernel.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public ResponseDetails ResponseDetails { get; set; }

        public BaseResponse AsError(params string[] errors)
        {
            errors ??= Array.Empty<string>();
            Success = false;
            ResponseDetails = new ResponseDetails
            {
                Message = "An error ocurred",
                Errors = errors
            };
            return this;
        }

        public BaseResponse AsSuccess()
        {
            Success = true;
            ResponseDetails = new ResponseDetails
            {
                Message = "Success",
                Errors = default
            };
            return this;
        }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public T Value { get; set; }

        public BaseResponse<T> AsError(T value, params string[] errors)
        {
            errors ??= Array.Empty<string>();
            AsError(errors);
            Value = value;
            return this;
        }

        public BaseResponse<T> AsSuccess(T value)
        {
            AsSuccess();
            Value = value;
            return this;
        }
    }
}