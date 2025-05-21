namespace Devices.Entities.DTOs;

public class DeviceByIdDto
{
    public string Name { get; set; }
    public string DeviceType { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public object AdditionalProperties { get; set; } = null!;
    public AllEmployeesDto? CurrentEmployee { get; set; }
}