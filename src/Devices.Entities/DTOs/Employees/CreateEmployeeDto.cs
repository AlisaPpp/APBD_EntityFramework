namespace Devices.Entities.DTOs;

public class CreateEmployeeDto
{
    public PersonInfoDto Person { get; set; } = null!;
    public decimal Salary { get; set; }
    public int PositionId { get; set; }
}