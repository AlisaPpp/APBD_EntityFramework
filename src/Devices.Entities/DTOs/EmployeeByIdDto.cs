namespace Devices.Entities.DTOs;

public class EmployeeByIdDto
{
    public PersonInfoDto PersonInfo { get; set; } = null!;
    public decimal Salary { get; set; }
    public PositionDto Position { get; set; } = null!;
    public DateTime HireDate { get; set; }
}