using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Readify.Models;

[Index("UtilisateurID", "LivreId", Name = "UQ__Achats_U__09D40066FE6B8B02", IsUnique = true)]
public partial class Achats_Utilisateur
{
    [Key]
    public int AchatID { get; set; }

    public int UtilisateurID { get; set; }

    public int LivreId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateAcquisition { get; set; }

    public int CommandeID { get; set; }

    [ForeignKey("CommandeID")]
    [InverseProperty("Achats_Utilisateurs")]
    public virtual Commande Commande { get; set; } = null!;

    [ForeignKey("LivreId")]
    [InverseProperty("Achats_Utilisateurs")]
    public virtual Livre Livre { get; set; } = null!;

    [ForeignKey("UtilisateurID")]
    [InverseProperty("Achats_Utilisateurs")]
    public virtual Utilisateur Utilisateur { get; set; } = null!;
}
