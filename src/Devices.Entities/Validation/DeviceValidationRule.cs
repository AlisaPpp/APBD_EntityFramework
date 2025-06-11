namespace Devices.Entities.Validation;

public class DeviceValidationRule
{
    public string Type {get;set;} = null!;
    public string PreRequestName { get; set; } = null!;
    public string PreRequestValue { get; set; } = null!;
    public List<Rule> Rules { get; set; } = null!;
}