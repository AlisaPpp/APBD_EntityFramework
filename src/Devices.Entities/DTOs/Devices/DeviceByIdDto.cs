namespace Devices.Entities.DTOs;

public class DeviceByIdDto
{
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public object AdditionalProperties { get; set; } = new();
    public string Type { get; set; } = null!;
}