using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Shared.Persistense.Abstractions.Entities.Catalogs;

namespace Shared.Persistense.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Catalog : ICatalog
{
    public Catalog()
    {

    }

    public Catalog(int id, string name)
    {
        Id = id;
        Name = name.ToUpperInvariant();
    }

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [Required, StringLength(100, MinimumLength = 3)]
    public string Name { get; init; } = null!;

    [StringLength(200)]
    public string? Info { get; set; }

    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
}