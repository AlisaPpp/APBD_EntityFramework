namespace Devices.Entities.DTOs;

public class EmployeeByIdDto
{
    public PersonInfoDto Person { get; set; } = null!;
    public decimal Salary { get; set; }
    public string Position { get; set; } = null!;
    public DateTime HireDate { get; set; }
}