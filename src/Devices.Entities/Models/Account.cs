using System.ComponentModel.DataAnnotations;

namespace Devices.API;

public class Account
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [RegularExpression(@"^[^\d].*", ErrorMessage = "Username shouldn’t start with a number.")]
    public string Username { get; set; } = null!;
    
    [Required]
    [MinLength(12, ErrorMessage = "Password should have length at least 12.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{12,}$",
        ErrorMessage = "Password must have at least one small letter, one capital letter, one number and one symbol.")]
    public string Password { get; set; } = null!;
    
    public int EmployeeId { get; set; }
    
    [Required]
    public virtual Employee Employee { get; set; } = null!;
    
    [Required]
    public int RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
}