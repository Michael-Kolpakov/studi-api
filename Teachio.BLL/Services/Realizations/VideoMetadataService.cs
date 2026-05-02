using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentResults;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Teachio.BLL.Models.Media;
using Teachio.BLL.Resources.SharedResource;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.SharedResources;
using Teachio.DAL.Utils.Constants;

namespace Teachio.BLL.Services.Realizations;

public class VideoMetadataService : IVideoMetadataService
{
    private static readonly Dictionary<string, string> _contentTypeToExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        ["video/mp4"] = "mp4",
        ["video/quicktime"] = "mov"
    };

    private readonly FfprobeOptions _options;
    private readonly ILoggerService _logger;
    private readonly IStringLocalizer<VideoMetadataSharedResource> _stringLocalizerVideoMetadata;

    public VideoMetadataService(
        IOptions<FfprobeOptions> options,
        ILoggerService logger,
        IStringLocalizer<VideoMetadataSharedResource> stringLocalizerVideoMetadata)
    {
        _options = options.Value;
        _logger = logger;
        _stringLocalizerVideoMetadata = stringLocalizerVideoMetadata;
    }

    public async Task<Result<VideoFileMetadata>> GetMetadataAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            var errorMessage = _stringLocalizerVideoMetadata[
                nameof(VideoMetadataSharedResource_en.VideoFilePathIsEmpty)
            ].Value;

            return Result.Fail(errorMessage);
        }

        if (!File.Exists(filePath))
        {
            var errorMessage = _stringLocalizerVideoMetadata[
                nameof(VideoMetadataSharedResource_en.VideoFileNotFound),
                filePath
            ].Value;

            return Result.Fail(errorMessage);
        }

        var ffprobeResult = await ExecuteFfprobeAsync(filePath, cancellationToken);

        if (ffprobeResult.IsFailed)
        {
            return Result.Fail(ffprobeResult.Errors[0].Message);
        }

        var parseResult = ParseMetadata(ffprobeResult.Value, filePath);

        if (parseResult.IsFailed)
        {
            return Result.Fail(parseResult.Errors[0].Message);
        }

        return parseResult;
    }

    private async Task<Result<string>> ExecuteFfprobeAsync(string filePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ExecutablePath))
        {
            var errorMessage = _stringLocalizerVideoMetadata[
                nameof(VideoMetadataSharedResource_en.FfprobeExecutablePathNotConfigured)
            ].Value;

            return Result.Fail(errorMessage);
        }

        var timeoutSeconds = _options.AnalysisTimeoutSeconds <= 0
            ? 30
            : _options.AnalysisTimeoutSeconds;

        var processStartInfo = new ProcessStartInfo
        {
            FileName = _options.ExecutablePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        processStartInfo.ArgumentList.Add("-v");
        processStartInfo.ArgumentList.Add("error");
        processStartInfo.ArgumentList.Add("-print_format");
        processStartInfo.ArgumentList.Add("json");
        processStartInfo.ArgumentList.Add("-select_streams");
        processStartInfo.ArgumentList.Add("v:0");
        processStartInfo.ArgumentList.Add("-show_entries");
        processStartInfo.ArgumentList.Add("format=duration,format_name:stream=width,height");
        processStartInfo.ArgumentList.Add(filePath);

        using var process = new Process
        {
            StartInfo = processStartInfo
        };

        try
        {
            if (!process.Start())
            {
                var errorMessage = _stringLocalizerVideoMetadata[
                    nameof(VideoMetadataSharedResource_en.FfprobeProcessNotStarted)
                ].Value;

                return Result.Fail(errorMessage);
            }

            var standardOutputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var standardErrorTask = process.StandardError.ReadToEndAsync(cancellationToken);

            using var timeoutCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                await process.WaitForExitAsync(timeoutCancellationTokenSource.Token);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                TryTerminateProcess(process);

                var errorMessage = _stringLocalizerVideoMetadata[
                    nameof(VideoMetadataSharedResource_en.FfprobeAnalysisTimedOut),
                    timeoutSeconds
                ].Value;

                return Result.Fail(errorMessage);
            }

            var standardOutput = await standardOutputTask;
            var standardError = await standardErrorTask;

            if (process.ExitCode != 0)
            {
                var errorMessage = string.IsNullOrWhiteSpace(standardError)
                    ? _stringLocalizerVideoMetadata[
                        nameof(VideoMetadataSharedResource_en.FfprobeFailedWithExitCode),
                        process.ExitCode
                    ].Value
                    : standardError;

                _logger.LogError(null, "FFprobe returned non-zero exit code.", errorMessage);

                return Result.Fail(errorMessage);
            }

            if (string.IsNullOrWhiteSpace(standardOutput))
            {
                var errorMessage = _stringLocalizerVideoMetadata[
                    nameof(VideoMetadataSharedResource_en.FfprobeReturnedEmptyOutput)
                ].Value;

                return Result.Fail(errorMessage);
            }

            return Result.Ok(standardOutput);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            var errorMessage = _stringLocalizerVideoMetadata[
                nameof(VideoMetadataSharedResource_en.FfprobeExecutionFailed)
            ].Value;
            _logger.LogError(null, errorMessage, ex.ToString());

            return Result.Fail(errorMessage);
        }
    }

    private Result<VideoFileMetadata> ParseMetadata(string ffprobeJson, string filePath)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<FfprobePayload>(ffprobeJson);
            var format = payload?.Format;
            var stream = payload?.Streams is { Count: > 0 }
                ? payload.Streams[0]
                : null;

            if (format is null)
            {
                var errorMessage = _stringLocalizerVideoMetadata[
                    nameof(VideoMetadataSharedResource_en.FfprobeOutputMissingFormat)
                ].Value;

                return Result.Fail(errorMessage);
            }

            if (stream is null)
            {
                var errorMessage = _stringLocalizerVideoMetadata[
                    nameof(VideoMetadataSharedResource_en.FfprobeOutputMissingResolution)
                ].Value;

                return Result.Fail(errorMessage);
            }

            if (!double.TryParse(format.Duration, NumberStyles.Float, CultureInfo.InvariantCulture, out var durationSecondsRaw))
            {
                var errorMessage = _stringLocalizerVideoMetadata[
                    nameof(VideoMetadataSharedResource_en.FfprobeOutputInvalidDuration)
                ].Value;

                return Result.Fail(errorMessage);
            }

            if (double.IsNaN(durationSecondsRaw) || double.IsInfinity(durationSecondsRaw) || durationSecondsRaw < 0)
            {
                var errorMessage = _stringLocalizerVideoMetadata[
                    nameof(VideoMetadataSharedResource_en.FfprobeInvalidDurationValue)
                ].Value;

                return Result.Fail(errorMessage);
            }

            var durationSeconds = checked((int)Math.Ceiling(durationSecondsRaw));

            if (durationSeconds > EntityConstants.MaxVideoDurationSeconds)
            {
                var errorMessage = _stringLocalizerVideoMetadata[
                    nameof(VideoMetadataSharedResource_en.VideoDurationExceedsLimit),
                    EntityConstants.MaxVideoDurationSeconds
                ].Value;

                return Result.Fail(errorMessage);
            }

            var contentTypeResult = ResolveContentType(format.FormatName, filePath);

            if (contentTypeResult.IsFailed)
            {
                return Result.Fail(contentTypeResult.Errors[0].Message);
            }

            if (!_contentTypeToExtension.TryGetValue(contentTypeResult.Value, out var fileExtension))
            {
                var errorMessage = _stringLocalizerVideoMetadata[
                    nameof(VideoMetadataSharedResource_en.ContentTypeExtensionCannotResolve),
                    contentTypeResult.Value
                ].Value;

                return Result.Fail(errorMessage);
            }

            var resolutionResult = ResolveResolution(stream.Width, stream.Height);

            if (resolutionResult.IsFailed)
            {
                return Result.Fail(resolutionResult.Errors[0].Message);
            }

            return Result.Ok(new VideoFileMetadata
            {
                ContentType = contentTypeResult.Value,
                DurationSeconds = durationSeconds,
                FileExtension = fileExtension,
                Resolution = resolutionResult.Value
            });
        }
        catch (JsonException)
        {
            var errorMessage = _stringLocalizerVideoMetadata[
                nameof(VideoMetadataSharedResource_en.FfprobeOutputParseFailed)
            ].Value;

            return Result.Fail(errorMessage);
        }
    }

    private Result<string> ResolveContentType(string? ffprobeFormatName, string filePath)
    {
        var normalizedFormats = ffprobeFormatName?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? [];

        var matchedContentType = normalizedFormats.Contains("mp4")
            ? "video/mp4"
            : normalizedFormats.Contains("mov")
                ? "video/quicktime"
                : null;

        if (matchedContentType is null)
        {
            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();

            matchedContentType = fileExtension switch
            {
                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                _ => null
            };
        }

        if (matchedContentType is null)
        {
            var allowedFormats = BuildAllowedFormatsDisplayValue();

            var errorMessage = _stringLocalizerVideoMetadata[
                nameof(VideoMetadataSharedResource_en.UnsupportedVideoFormat),
                allowedFormats
            ].Value;

            return Result.Fail(errorMessage);
        }

        var isAllowedContentType = EntityConstants.AllowedVideoContentTypes.Contains(matchedContentType, StringComparer.OrdinalIgnoreCase);

        if (!isAllowedContentType)
        {
            var allowedContentTypes = string.Join(", ", EntityConstants.AllowedVideoContentTypes);
            var errorMessage = _stringLocalizerVideoMetadata[
                nameof(VideoMetadataSharedResource_en.UnsupportedContentType),
                matchedContentType,
                allowedContentTypes
            ].Value;

            return Result.Fail(errorMessage);
        }

        return Result.Ok(matchedContentType);
    }

    private Result<int> ResolveResolution(int? width, int? height)
    {
        if (!width.HasValue || !height.HasValue || width.Value <= 0 || height.Value <= 0)
        {
            var errorMessage = _stringLocalizerVideoMetadata[
                nameof(VideoMetadataSharedResource_en.InvalidVideoResolution)
            ].Value;

            return Result.Fail(errorMessage);
        }

        var normalizedResolution = Math.Min(width.Value, height.Value);
        var isAllowedResolution = EntityConstants.AllowedVideoResolutions.Contains(normalizedResolution);

        if (!isAllowedResolution)
        {
            var allowedResolutions = BuildAllowedResolutionsDisplayValue();

            var errorMessage = _stringLocalizerVideoMetadata[
                nameof(VideoMetadataSharedResource_en.UnsupportedVideoResolution),
                normalizedResolution,
                allowedResolutions
            ].Value;

            return Result.Fail(errorMessage);
        }

        return Result.Ok(normalizedResolution);
    }

    private static string BuildAllowedFormatsDisplayValue()
    {
        var allowedContentTypes = EntityConstants.AllowedVideoContentTypes;

        var allowedFormats = allowedContentTypes
            .Select(contentType => _contentTypeToExtension.TryGetValue(contentType, out var extension)
                ? extension
                : contentType)
            .Select(value => value.ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase);

        return string.Join(", ", allowedFormats);
    }

    private static string BuildAllowedResolutionsDisplayValue()
    {
        var allowedResolutions = EntityConstants.AllowedVideoResolutions
            .OrderBy(resolution => resolution)
            .Select(resolution => $"{resolution}p");

        return string.Join(", ", allowedResolutions);
    }

    private static void TryTerminateProcess(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
        }
        catch
        {
            // Ignore process termination errors intentionally
        }
    }

    private sealed class FfprobePayload
    {
        [JsonPropertyName("format")]
        public FfprobeFormat? Format { get; set; }

        [JsonPropertyName("streams")]
        public List<FfprobeStream> Streams { get; set; } = [];
    }

    private sealed class FfprobeFormat
    {
        [JsonPropertyName("duration")]
        public string? Duration { get; set; }

        [JsonPropertyName("format_name")]
        public string? FormatName { get; set; }
    }

    private sealed class FfprobeStream
    {
        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }
    }
}
