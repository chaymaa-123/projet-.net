using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Readify.Models;

[Table("Details_Commande")]
[Index("CommandeID", "LivreId", Name = "UQ__Details___8D54B1284A1A73FA", IsUnique = true)]
public partial class Details_Commande
{
    [Key]
    public int DetailID { get; set; }

    public int CommandeID { get; set; }

    public int LivreId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PrixAchat { get; set; }

    [ForeignKey("CommandeID")]
    [InverseProperty("Details_Commandes")]
    public virtual Commande Commande { get; set; } = null!;

    [ForeignKey("LivreId")]
    [InverseProperty("Details_Commandes")]
    public virtual Livre Livre { get; set; } = null!;
}
