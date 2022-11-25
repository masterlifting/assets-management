namespace Shared.Persistense.Abstractions.Entities.Catalogs;

//[Index(nameof(Name), IsUnique = true)]
public abstract class Catalog : IEntityCatalog
{

    // [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    //[Required, StringLength(100, MinimumLength = 3)]
    public string Name { get; init; } = null!;

    //[StringLength(200)]
    public string? Info { get; set; }

    public DateTime Created { get; init; } = DateTime.UtcNow;
}