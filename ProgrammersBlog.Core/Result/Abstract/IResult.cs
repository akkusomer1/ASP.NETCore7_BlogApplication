using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;

namespace ProgrammersBlog.Core.Result.Abstract
{
    public interface IResult
    {
        public ResultStatusType ResultStatus { get;  }

        public string Message { get;  }
        public Exception Exception { get;  }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }
    }
}
