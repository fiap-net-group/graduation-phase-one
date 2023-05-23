namespace PoliceDepartment.EvidenceManager.SharedKernel.Responses
{
    public class BaseResponseWithValue<T> : BaseResponse
    {
        public T Value { get; set; }

        public BaseResponseWithValue<T> AsError(T value, params string[] errors)
        {
            AsError(errors);
            Value = value;
            return this;
        }

        public new BaseResponseWithValue<T> AsError(params string[] errors)
        {
            errors ??= Array.Empty<string>();
            base.AsError(errors);
            return this;
        }

        public BaseResponseWithValue<T> AsSuccess(T value)
        {
            base.AsSuccess();
            Value = value;
            return this;
        }
    }
}