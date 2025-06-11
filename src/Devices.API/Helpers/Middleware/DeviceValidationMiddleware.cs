using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Devices.Entities.DTOs;
using Devices.Entities.Validation;
using Devices.Services;

namespace Devices.API.Helpers.Middleware;

public class DeviceValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DeviceValidationMiddleware> _logger;

    public DeviceValidationMiddleware(RequestDelegate next, IWebHostEnvironment env,
        ILogger<DeviceValidationMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Beginning to validate");
        try
        {
            if (context.Request.Path.StartsWithSegments("/api/devices") &&
                (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put))
            {
                var deviceService = context.RequestServices.GetService<IDeviceService>();
                context.Request.EnableBuffering();

                using var reader = new StreamReader(context.Request.Body, encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                var deviceDto = JsonSerializer.Deserialize<CreateDeviceDto>(body, new JsonSerializerOptions
                    { PropertyNameCaseInsensitive = true });

                if (deviceDto == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Invalid request");
                    return;
                }

                var path = Path.Combine(_env.ContentRootPath, "Helpers", "Middleware", "validation_rules.json");
                var configurationText = await File.ReadAllTextAsync(path);
                var configuration = JsonSerializer.Deserialize<Validation>(
                    configurationText,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                _logger.LogInformation("Raw validation config: {Json}", configurationText);
                _logger.LogInformation("Parsed rule count: {Count}", configuration?.Validations?.Count ?? 0);

                if (configuration == null)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Validation configuration could not be loaded");
                    return;
                }

                var serviceDeviceType = await deviceService.GetDeviceTypeNameById(deviceDto.TypeId,
                    token: context.RequestAborted);
                var deviceType = configuration.Validations
                    .FirstOrDefault(x =>
                        x.Type.Equals(serviceDeviceType, StringComparison.OrdinalIgnoreCase));

                if (deviceType != null)
                {
                    var deviceDtoType = typeof(CreateDeviceDto);
                    var property = deviceDtoType.GetProperty(deviceType.PreRequestName, BindingFlags.IgnoreCase
                        | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        var value = property.GetValue(deviceDto)?.ToString().ToLower();
                        if (value == deviceType.PreRequestValue.ToLower())
                        {
                            var additionalProperties = JsonSerializer.Serialize(deviceDto.AdditionalProperties);
                            var additionalPropertiesDict =
                                JsonSerializer.Deserialize<Dictionary<string, object>>(additionalProperties);

                            var errors = new List<string>();
                            foreach (var rule in deviceType.Rules)
                            {
                                additionalPropertiesDict.TryGetValue(rule.ParamName, out var additionalPropertyValue);
                                var paramValueString = additionalPropertyValue?.ToString();

                                if (rule.Regex.ValueKind == JsonValueKind.Array)
                                {
                                    var allowedValues = rule.Regex.EnumerateArray().Select(x => x.GetString()).ToList();
                                    if (!allowedValues.Contains(paramValueString))
                                        errors.Add(
                                            $"{rule.ParamName} must be one of: {string.Join(", ", allowedValues)}");
                                }
                                else if (rule.Regex.ValueKind == JsonValueKind.String)
                                {
                                    var regex = new Regex(rule.Regex.GetString() ?? "");
                                    if (!regex.IsMatch(paramValueString ?? ""))
                                        errors.Add($"{rule.ParamName} is not valid according to regex {regex}");
                                }
                            }

                            if (errors.Any())
                            {
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors }));
                                return;
                            }
                        }
                    }
                }
            }
            await _next(context);
            _logger.LogInformation($"Finished validation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed validation");
            throw;
        }
    }
}