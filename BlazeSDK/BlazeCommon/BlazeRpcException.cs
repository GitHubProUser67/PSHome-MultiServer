namespace BlazeCommon
{
    public class BlazeRpcException : Exception
    {
        public int ErrorCode { get; }
        public object? ErrorResponse { get; }

        public BlazeRpcException(int errorCode, object? errorResponse, string? message) : base(message)
        {
            ErrorCode = errorCode;
            ErrorResponse = errorResponse;
        }

        public BlazeRpcException(int errorCode, object? errorResponse, string? message, Exception? innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
            ErrorResponse = errorResponse;
        }

        public BlazeRpcException(int errorCode, object? errorResponse) : base()
        {
            ErrorCode = errorCode;
            ErrorResponse = errorResponse;
        }

        public BlazeRpcException(int errorCode, object? errorResponse, Exception? innerException) : base(null, innerException)
        {
            ErrorCode = errorCode;
            ErrorResponse = errorResponse;
        }

        public BlazeRpcException(int errorCode, string? message) : base(message)
        {
            ErrorCode = errorCode;
            ErrorResponse = null;
        }

        public BlazeRpcException(int errorCode, string? message, Exception? innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
            ErrorResponse = null;
        }

        public BlazeRpcException(int errorCode) : base()
        {
            ErrorCode = errorCode;
            ErrorResponse = null;
        }

        public BlazeRpcException(int errorCode, Exception? innerException) : base(null, innerException)
        {
            ErrorCode = errorCode;
            ErrorResponse = null;
        }

        //Same with enum

        int EnumToErrorCode(Enum errorEnum)
        {
            return Convert.ToInt32(errorEnum);
        }

        public BlazeRpcException(Enum errorEnum, object? errorResponse, string? message) : base(message)
        {
            ErrorCode = EnumToErrorCode(errorEnum);
            ErrorResponse = errorResponse;
        }

        public BlazeRpcException(Enum errorEnum, object? errorResponse, string? message, Exception? innerException) : base(message, innerException)
        {
            ErrorCode = EnumToErrorCode(errorEnum);
            ErrorResponse = errorResponse;
        }

        public BlazeRpcException(Enum errorEnum, object? errorResponse) : base()
        {
            ErrorCode = EnumToErrorCode(errorEnum);
            ErrorResponse = errorResponse;
        }

        public BlazeRpcException(Enum errorEnum, object? errorResponse, Exception? innerException) : base(null, innerException)
        {
            ErrorCode = EnumToErrorCode(errorEnum);
            ErrorResponse = errorResponse;
        }

        public BlazeRpcException(Enum errorEnum, string? message) : base(message)
        {
            ErrorCode = EnumToErrorCode(errorEnum);
            ErrorResponse = null;
        }

        public BlazeRpcException(Enum errorEnum, string? message, Exception? innerException) : base(message, innerException)
        {
            ErrorCode = EnumToErrorCode(errorEnum);
            ErrorResponse = null;
        }

        public BlazeRpcException(Enum errorEnum) : base()
        {
            ErrorCode = EnumToErrorCode(errorEnum);
            ErrorResponse = null;
        }

        public BlazeRpcException(Enum errorEnum, Exception? innerException) : base(null, innerException)
        {
            ErrorCode = EnumToErrorCode(errorEnum);
            ErrorResponse = null;
        }

    }
}
