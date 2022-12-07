using AM.Services.Market.Domain.Entities.Catalogs;

namespace AM.Services.Market.Domain.Entities.Interfaces;

public interface IRating : IDataIdentity
{
    Status Status { get; set; }
    byte StatusId { get; set; }

    decimal? Result { get; set; }
}