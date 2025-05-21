namespace Devices.Entities.DTOs;

public class EmployeeByIdDto
{
    public PersonInfoDto PersonInfo { get; set; }
    public decimal Salary { get; set; }
    public PositionDto Position { get; set; }
    public DateTime HireDate { get; set; }
}