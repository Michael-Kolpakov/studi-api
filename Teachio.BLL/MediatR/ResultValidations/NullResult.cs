using FluentResults;

namespace Teachio.BLL.MediatR.ResultValidations;

/// <summary>
/// Represents the <see cref="NullResult{T}"/> type.
/// </summary>
/// <typeparam name="T">The type of t.</typeparam>
public class NullResult<T> : Result<T>
{
    public NullResult()
        : base()
    {
    }
}
