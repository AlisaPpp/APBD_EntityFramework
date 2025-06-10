namespace Devices.Entities.DTOs;

public class CreateDeviceDto
{
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public object AdditionalProperties { get; set; } = new();
    public int TypeId { get; set; } 

}