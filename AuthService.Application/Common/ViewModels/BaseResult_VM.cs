using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Common.ViewModels
{
    public record BaseResult_VM
    {
        public int ErrorCode { get; init; }
        public string Message { get; init; }
        public object Result { get; init; }

        public bool IsSuccess => ErrorCode == 0;

        private BaseResult_VM(int errorCode, string message, object result)
        {
            ErrorCode = errorCode;
            Message = message;
            Result = result;
        }

        public static BaseResult_VM Ok(object result = null, string message = "OK")
            => new(0, message, result);

        public static BaseResult_VM Fail(int errorCode, string message)
            => new(errorCode, message, null);
    }

    // نسخه جنریک برای تایپ‌سیف بودن (توصیه می‌شود)
    public record BaseResult_VM<T>
    {
        public int ErrorCode { get; init; }
        public string Message { get; init; }
        public T? Result { get; init; }

        public bool IsSuccess => ErrorCode == 0;

        private BaseResult_VM(int errorCode, string message, T? result)
        {
            ErrorCode = errorCode;
            Message = message;
            Result = result;
        }

        public static BaseResult_VM<T> Ok(T result, string message = "OK")
            => new(0, message, result);

        public static BaseResult_VM<T> Fail(int errorCode, string message)
            => new(errorCode, message, default);
    }

    public enum ErrorCodes
    {
        None = 0,
        ValidationFailed = 1001,
        DuplicateEmail = 1002,
        DuplicateUsername = 1003,
        Unauthorized = 1401,
        NotFound = 1404,
        Conflict = 1409,
        InternalError = 1500
    }
}
