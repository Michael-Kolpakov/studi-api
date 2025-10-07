using FluentResults;

namespace Teachio.BLL.MediatR.ResultValidations;

public class NullResult<T> : Result<T>
{
    public NullResult()
        : base()
    {
    }
}
