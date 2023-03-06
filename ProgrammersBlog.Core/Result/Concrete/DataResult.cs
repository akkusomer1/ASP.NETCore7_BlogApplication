using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Abstract;

namespace ProgrammersBlog.Core.Result.Concrete
{
    public class DataResult<T> : IDataResult<T>
    {
        public T Data { get; }
        public ResultStatusType ResultStatus { get; }
        public string Message { get; }
        public Exception Exception { get; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }

        public DataResult(T data, ResultStatusType resultStatus, IEnumerable<ValidationError> validationErrors)
        {
            Data = data;
            ResultStatus = resultStatus;
            ValidationErrors = validationErrors;
        }

        public DataResult(T data, ResultStatusType resultStatus, string message, IEnumerable<ValidationError> validationErrors)
        {
            Data = data;
            ResultStatus = resultStatus;
            Message = message;
            ValidationErrors = validationErrors;
        }

        public DataResult(T data, ResultStatusType resultStatus)
        {
            Data = data;
            ResultStatus = resultStatus;
        }

        public DataResult(T data, ResultStatusType resultStatus, string message)
        {
            Data = data;
            ResultStatus = resultStatus;
            Message = message;
        }

        public DataResult(ResultStatusType resultStatus, string message)
        {
            ResultStatus = resultStatus;
            Message = message;
        }

        public DataResult(T data, ResultStatusType resultStatus, string message, Exception exception)
        {
            Data = data;
            ResultStatus = resultStatus;
            Message = message;
            Exception = exception;
        }
    }
}
