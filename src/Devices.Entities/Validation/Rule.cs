using System.Text.Json;
using System.Text.Json.Serialization;

namespace Devices.Entities.Validation;

public class Rule
{
    public string ParamName { get; set; } = null!;
    public JsonElement Regex { get; set; }
}