using AM.Services.Common.Contracts.Models.Service;
using AM.Services.Market.Domain.DataAccess;
using AM.Services.Market.Domain.DataAccess.Comparators;
using AM.Services.Market.Domain.Entities.ManyToMany;
using AM.Services.Market.Models.Api.Http;

namespace AM.Services.Market.Services.Http;

public class CompanySourceApi
{
    private readonly Repository<CompanySource> companySourceRepo;
    public CompanySourceApi(Repository<CompanySource> companySourceRepo) => this.companySourceRepo = companySourceRepo;

    public async Task<PaginationModel<SourceGetDto>> GetAsync(string companyId)
    {
        companyId = companyId.ToUpperInvariant();
        var sources = await companySourceRepo.GetSampleAsync(
            x => x.CompanyId == companyId,
            x => new SourceGetDto(x.SourceId, x.Source.Name, x.Value));

        return new PaginationModel<SourceGetDto>
        {
            Count = sources.Length,
            Items = sources
        };
    }
    public async Task<SourceGetDto> GetAsync(string companyId, byte sourceId)
    {
        companyId = companyId.ToUpperInvariant();
        var source = await companySourceRepo.FindAsync(companyId, sourceId);

        return source is null
            ? throw new NullReferenceException(nameof(source))
            : new SourceGetDto(sourceId, source.Source.Name, source.Value);
    }
    public async Task<CompanySource[]> CreateUpdateDeleteAsync(string companyId, IEnumerable<SourcePostDto> models)
    {
        companyId = companyId.ToUpperInvariant();
        return await companySourceRepo.CreateUpdateDeleteAsync(
        models
                .Select(x => new CompanySource
                {
                    CompanyId = companyId,
                    SourceId = x.Id,
                    Value = x.Value
                })
                .ToArray(),
            new CompanySourceComparer(),
            companyId);
    }
}