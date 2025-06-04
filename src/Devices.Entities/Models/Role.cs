using System.ComponentModel.DataAnnotations;

namespace Devices.API;

public class Role
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

}