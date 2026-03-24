using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Readify.Models;

[Index("NomRole", Name = "UQ__Roles__ADB14FA63AE30155", IsUnique = true)]
public partial class Role
{
    [Key]
    public int RoleID { get; set; }

    [StringLength(50)]
    public string NomRole { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}
