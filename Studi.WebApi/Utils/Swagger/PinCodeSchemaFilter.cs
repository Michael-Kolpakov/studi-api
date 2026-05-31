using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Studi.BLL.DTOs.Users.Auth.Request;
using Studi.BLL.Models.Auth;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Studi.WebApi.Utils.Swagger;

public sealed class PinCodeSchemaFilter : ISchemaFilter
{
    private readonly IOptions<EmailVerificationOptions> _options;

    public PinCodeSchemaFilter(IOptions<EmailVerificationOptions> options)
    {
        _options = options;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.MemberInfo is null)
        {
            return;
        }

        if (context.MemberInfo.DeclaringType != typeof(VerifyRegistrationPinRequestDto))
        {
            return;
        }

        if (!string.Equals(context.MemberInfo.Name, nameof(VerifyRegistrationPinRequestDto.PinCode), StringComparison.Ordinal))
        {
            return;
        }

        if (context.Type != typeof(string))
        {
            return;
        }

        var pinLength = _options.Value.PinLength;

        if (pinLength <= 0)
        {
            return;
        }

        schema.MinLength = pinLength;
        schema.MaxLength = pinLength;
    }
}
