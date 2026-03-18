using System.Linq.Expressions;

namespace Teachio.XIntegrationTests.Utils;

/// <summary>
/// Represents the <see cref="ClientInitializer{TResult}"/> type.
/// </summary>
/// <typeparam name="TResult">The type of result.</typeparam>
public static class ClientInitializer<TResult>
{
    public static readonly Func<HttpClient, string, TResult> Initialize = CreateClientInitializerFunction();

    private static Func<HttpClient, string, TResult> CreateClientInitializerFunction()
    {
        var testClassConstructorCache = typeof(TResult).GetConstructor([typeof(HttpClient), typeof(string)])!;
        var clientParameter = Expression.Parameter(typeof(HttpClient), "_client");
        var secondPartUrlParameter = Expression.Parameter(typeof(string), "secondPartUrl");

        var constructorExpression = Expression.New(
            testClassConstructorCache,
            clientParameter,
            secondPartUrlParameter);

        var lambdaExpression = Expression.Lambda<Func<HttpClient, string, TResult>>(
            constructorExpression,
            clientParameter,
            secondPartUrlParameter);

        return lambdaExpression.Compile();
    }
}
