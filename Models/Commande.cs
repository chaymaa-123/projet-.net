using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readify.Models;

public partial class Commande
{
    [Key]
    public int CommandeID { get; set; }

    public int UtilisateurID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateCommande { get; set; }

    [StringLength(50)]
    public string Statut { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal MontantTotal { get; set; }

    [InverseProperty("Commande")]
    public virtual ICollection<Achats_Utilisateur> Achats_Utilisateurs { get; set; } = new List<Achats_Utilisateur>();

    [InverseProperty("Commande")]
    public virtual ICollection<Details_Commande> Details_Commandes { get; set; } = new List<Details_Commande>();

    [ForeignKey("UtilisateurID")]
    [InverseProperty("Commandes")]
    public virtual Utilisateur Utilisateur { get; set; } = null!;

}
