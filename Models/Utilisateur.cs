using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Readify.Models;

[Index("NomUtilisateur", Name = "UQ__Utilisat__49EDB0E5EA7D36D2", IsUnique = true)]
[Index("Email", Name = "UQ__Utilisat__A9D1053443451E8C", IsUnique = true)]
public partial class Utilisateur
{
    [Key]
    public int UtilisateurID { get; set; }

    [StringLength(50)]
    public string NomUtilisateur { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string MotDePasseHash { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? DateInscription { get; set; }

    public int RoleID { get; set; }

    [InverseProperty("Utilisateur")]
    public virtual ICollection<Achats_Utilisateur> Achats_Utilisateurs { get; set; } = new List<Achats_Utilisateur>();

    [InverseProperty("Utilisateur")]
    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();

    [ForeignKey("RoleID")]
    [InverseProperty("Utilisateurs")]
    public virtual Role Role { get; set; } = null!;
}
