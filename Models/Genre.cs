using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;


namespace Readify.Models;

[Index("NomGenre", Name = "UQ__Genres__33AD853236116C3E", IsUnique = true)]
public partial class Genre
{
    [Key]
    public int GenreID { get; set; }

    [StringLength(100)]
    public string NomGenre { get; set; } = null!;


    [InverseProperty("Genre")]
    [JsonIgnore]
    public virtual ICollection<Livre> Livres { get; set; } = new List<Livre>();
}
