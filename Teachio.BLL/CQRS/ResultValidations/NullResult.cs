using FluentResults;

namespace Teachio.BLL.CQRS.ResultValidations;

public class NullResult<T> : Result<T>
{
    public NullResult()
        : base()
    {
    }
}
