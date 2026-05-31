using FluentResults;

namespace Studi.BLL.CQRS.ResultValidations;

public class NullResult<T> : Result<T>
{
    public NullResult()
        : base()
    {
    }
}
