using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace Readify.Models // Vérifie que c'est bien le namespace de ton projet
{
    [Table("Panier")]
    public class Panier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Quantite { get; set; }

        [Required]
        public DateTime DateAjout { get; set; }

        // --- Clé étrangère vers la table Livre ---

        // 1. La colonne brute (l'entier stocké en base)
        [Required]
        public int LivreId { get; set; }

        [ForeignKey("LivreId")]
        public virtual Livre Livre { get; set; } = null!; // Ajoute = null!;
    }
}