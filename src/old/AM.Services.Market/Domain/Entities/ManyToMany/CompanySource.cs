using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AM.Services.Market.Domain.Entities.Interfaces;
using static AM.Services.Market.Enums;

namespace AM.Services.Market.Domain.Entities.ManyToMany;

public class CompanySource : IDataIdentity
{
    [JsonIgnore]
    public virtual Company Company { get; init; } = null!;
    public string CompanyId { get; set; } = null!;
    
    [JsonIgnore]
    public virtual Source Source { get; init; } = null!;
    public byte SourceId { get; set; } = (byte)Sources.Manual;


    [StringLength(300, MinimumLength = 1)]
    public string? Value { get; set; }
}