using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readify.Models;

[Table("Livre")]
public partial class Livre
{
    [Key]
    public int LivreId { get; set; }

    [StringLength(255)]
    [Required(ErrorMessage = "Le titre est obligatoire")]
    public string? Titre { get; set; } = null!;

    public string? Resume { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    [Required(ErrorMessage = "Le prix est obligatoire")]
    public decimal Prix { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DatePublication { get; set; }

    public bool EstPubliee { get; set; }

    // --- GESTION DES IMAGES ---
    public string? ImageUrl { get; set; } // Stocké en base de données (ex: "photo.jpg")

    [NotMapped] // Pas dans la base de données
    [Display(Name = "Image de couverture")]
    public IFormFile? ImageFile { get; set; } // Reçu du formulaire
    // --------------------------

    public int GenreID { get; set; }

    [ForeignKey("GenreID")]
    [InverseProperty("Livres")]
    public virtual Genre Genre { get; set; } = null!;

    [InverseProperty("Livre")]
    public virtual ICollection<Achats_Utilisateur> Achats_Utilisateurs { get; set; } = new List<Achats_Utilisateur>();

    [InverseProperty("Livre")]
    public virtual ICollection<Details_Commande> Details_Commandes { get; set; } = new List<Details_Commande>();
}