using System.Buffers.Binary;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Localization;
using Studi.BLL.Models.Media;
using Studi.BLL.Resources.SharedResource;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.SharedResource;
using Studi.DAL.Utils.Constants;

namespace Studi.BLL.Services.Realizations;

public class AvatarMetadataService : IAvatarMetadataService
{
    private static readonly Dictionary<string, string[]> _contentTypeToExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/png"] = ["png"],
        ["image/jpeg"] = ["jpg", "jpeg"]
    };

    private static readonly byte[] _pngSignature =
    [
        0x89,
        0x50,
        0x4E,
        0x47,
        0x0D,
        0x0A,
        0x1A,
        0x0A
    ];

    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<AvatarMetadataSharedResource> _stringLocalizerAvatarMetadata;

    public AvatarMetadataService(
        ILoggerService logger,
        IStringLocalizer<AvatarMetadataSharedResource> stringLocalizerAvatarMetadata)
    {
        _logger = logger;
        _stringLocalizerAvatarMetadata = stringLocalizerAvatarMetadata;
    }

    public async Task<Result<AvatarFileMetadata>> GetMetadataAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            var errorMessage = _stringLocalizerAvatarMetadata[
                nameof(AvatarMetadataSharedResource_en.AvatarFilePathIsEmpty)
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (!File.Exists(filePath))
        {
            var errorMessage = _stringLocalizerAvatarMetadata[
                nameof(AvatarMetadataSharedResource_en.AvatarFileNotFound),
                filePath
            ].Value;

            return Result.Fail(errorMessage);
        }

        try
        {
            await using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true);

            var header = new byte[8];
            var read = await stream.ReadAsync(header, cancellationToken);

            if (read < header.Length)
            {
                return Result.Fail(CreateUnsupportedFormatMessage());
            }

            if (IsPngSignature(header))
            {
                return ParsePng(stream, filePath, cancellationToken);
            }

            if (IsJpegSignature(header))
            {
                if (stream.CanSeek)
                {
                    stream.Position = 2;
                }

                return ParseJpeg(stream, filePath, cancellationToken);
            }

            return Result.Fail(CreateUnsupportedFormatMessage());
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerAvatarMetadata[
                nameof(AvatarMetadataSharedResource_en.AvatarMetadataProcessingFailed)
            ].Value;
            _logger.LogError(null, errorMessage, ex.ToString());

            return Result.Fail(errorMessage);
        }
    }

    private Result<AvatarFileMetadata> ParsePng(Stream stream, string filePath, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            Span<byte> headerBuffer = stackalloc byte[4];
            stream.ReadExactly(headerBuffer);

            stream.ReadExactly(headerBuffer);
            var chunkType = Encoding.ASCII.GetString(headerBuffer);

            if (!string.Equals(chunkType, "IHDR", StringComparison.Ordinal))
            {
                return Result.Fail(CreateUnsupportedFormatMessage());
            }

            Span<byte> dimensionBuffer = stackalloc byte[8];
            stream.ReadExactly(dimensionBuffer);

            var width = BinaryPrimitives.ReadInt32BigEndian(dimensionBuffer[..4]);
            var height = BinaryPrimitives.ReadInt32BigEndian(dimensionBuffer[4..]);

            return BuildMetadata("image/png", filePath, width, height);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (EndOfStreamException)
        {
            return Result.Fail(CreateUnsupportedFormatMessage());
        }
    }

    private Result<AvatarFileMetadata> ParseJpeg(Stream stream, string filePath, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var markerPrefix = stream.ReadByte();

                if (markerPrefix == -1)
                {
                    break;
                }

                if (markerPrefix != 0xFF)
                {
                    continue;
                }

                var marker = stream.ReadByte();

                if (marker == -1)
                {
                    break;
                }

                while (marker == 0xFF)
                {
                    marker = stream.ReadByte();

                    if (marker == -1)
                    {
                        break;
                    }
                }

                if (marker == -1)
                {
                    break;
                }

                if (marker is 0xD8 or 0x01 or (>= 0xD0 and <= 0xD7))
                {
                    continue;
                }

                if (marker is 0xDA or 0xD9)
                {
                    break;
                }

                var segmentLength = ReadUInt16BigEndian(stream);

                if (segmentLength < 2)
                {
                    return Result.Fail(CreateUnsupportedFormatMessage());
                }

                if (IsStartOfFrameMarker(marker))
                {
                    if (stream.ReadByte() == -1)
                    {
                        return Result.Fail(CreateUnsupportedFormatMessage());
                    }

                    var height = ReadUInt16BigEndian(stream);
                    var width = ReadUInt16BigEndian(stream);

                    return BuildMetadata("image/jpeg", filePath, width, height);
                }

                stream.Seek(segmentLength - 2, SeekOrigin.Current);
            }

            return Result.Fail(CreateUnsupportedFormatMessage());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (EndOfStreamException)
        {
            return Result.Fail(CreateUnsupportedFormatMessage());
        }
    }

    private Result<AvatarFileMetadata> BuildMetadata(string contentType, string filePath, int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            var errorMessage = _stringLocalizerAvatarMetadata[
                nameof(AvatarMetadataSharedResource_en.AvatarResolutionInvalid)
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (width < EntityConstants.MinAvatarWidth
            || width > EntityConstants.MaxAvatarWidth
            || height < EntityConstants.MinAvatarHeight
            || height > EntityConstants.MaxAvatarHeight)
        {
            var errorMessage = _stringLocalizerAvatarMetadata[
                nameof(AvatarMetadataSharedResource_en.AvatarResolutionOutOfRange),
                EntityConstants.MinAvatarWidth,
                EntityConstants.MinAvatarHeight,
                EntityConstants.MaxAvatarWidth,
                EntityConstants.MaxAvatarHeight
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (width * EntityConstants.AvatarAspectRatioHeight
            != height * EntityConstants.AvatarAspectRatioWidth)
        {
            var errorMessage = _stringLocalizerAvatarMetadata[
                nameof(AvatarMetadataSharedResource_en.AvatarAspectRatioInvalid),
                EntityConstants.AvatarAspectRatioWidth,
                EntityConstants.AvatarAspectRatioHeight
            ].Value;

            return Result.Fail(errorMessage);
        }

        var resolution = $"{width}x{height}";

        if (resolution.Length > EntityConstants.MaxAvatarResolutionLength)
        {
            var errorMessage = _stringLocalizerAvatarMetadata[
                nameof(AvatarMetadataSharedResource_en.AvatarResolutionTooLong),
                EntityConstants.MaxAvatarResolutionLength
            ].Value;

            return Result.Fail(errorMessage);
        }

        var isAllowedContentType = EntityConstants.AllowedAvatarContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);

        if (!isAllowedContentType)
        {
            var allowedContentTypes = string.Join(", ", EntityConstants.AllowedAvatarContentTypes);
            var errorMessage = _stringLocalizerAvatarMetadata[
                nameof(AvatarMetadataSharedResource_en.UnsupportedContentType),
                contentType,
                allowedContentTypes
            ].Value;

            return Result.Fail(errorMessage);
        }

        var extensionResult = ResolveFileExtension(contentType, filePath);

        if (extensionResult.IsFailed)
        {
            return Result.Fail(extensionResult.Errors[0].Message);
        }

        return Result.Ok(new AvatarFileMetadata
        {
            ContentType = contentType,
            FileExtension = extensionResult.Value,
            Resolution = resolution,
            Width = width,
            Height = height
        });
    }

    private Result<string> ResolveFileExtension(string contentType, string filePath)
    {
        if (!_contentTypeToExtensions.TryGetValue(contentType, out var allowedExtensions)
            || allowedExtensions.Length == 0)
        {
            var errorMessage = _stringLocalizerAvatarMetadata[
                nameof(AvatarMetadataSharedResource_en.ContentTypeExtensionCannotResolve),
                contentType
            ].Value;

            return Result.Fail(errorMessage);
        }

        var extension = Path.GetExtension(filePath)
            .TrimStart('.')
            .ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(extension)
            && allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            return Result.Ok(extension);
        }

        return Result.Ok(allowedExtensions[0]);
    }

    private string CreateUnsupportedFormatMessage()
    {
        var allowedFormats = BuildAllowedFormatsDisplayValue();

        return _stringLocalizerAvatarMetadata[
            nameof(AvatarMetadataSharedResource_en.AvatarFormatNotSupported),
            allowedFormats
        ].Value;
    }

    private static string BuildAllowedFormatsDisplayValue()
    {
        var allowedExtensions = _contentTypeToExtensions.Values
            .SelectMany(values => values)
            .Select(value => value.ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase);

        return string.Join(", ", allowedExtensions);
    }

    private static bool IsPngSignature(ReadOnlySpan<byte> header)
    {
        return header.Length >= _pngSignature.Length
            && header[.._pngSignature.Length].SequenceEqual(_pngSignature);
    }

    private static bool IsJpegSignature(ReadOnlySpan<byte> header)
    {
        return header.Length >= 2
            && header[0] == 0xFF
            && header[1] == 0xD8;
    }

    private static bool IsStartOfFrameMarker(int marker)
    {
        return marker is 0xC0 or 0xC1 or 0xC2 or 0xC3
            or 0xC5 or 0xC6 or 0xC7
            or 0xC9 or 0xCA or 0xCB
            or 0xCD or 0xCE or 0xCF;
    }

    private static ushort ReadUInt16BigEndian(Stream stream)
    {
        var high = stream.ReadByte();
        var low = stream.ReadByte();

        if (high == -1 || low == -1)
        {
            throw new EndOfStreamException();
        }

        return (ushort)((high << 8) | low);
    }
}
