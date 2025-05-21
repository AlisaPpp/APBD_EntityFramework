namespace Devices.Entities.DTOs;

public class CreateDeviceDto
{
    public string Name { get; set; }
    public string DeviceType { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public object AdditionalProperties { get; set; }
}