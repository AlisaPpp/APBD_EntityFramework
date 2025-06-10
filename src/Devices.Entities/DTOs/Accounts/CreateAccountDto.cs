namespace Devices.Entities.DTOs;

public class CreateAccountDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int EmployeeId { get; set; }
    public int RoleId { get; set; }

}