﻿using System;
using IM.Service.Common.Net.RepositoryService;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IM.Service.Company.DataAccess.Repository;

public class CompanyRepository : IRepositoryHandler<Entities.Company>
{
    private readonly DatabaseContext context;
    public CompanyRepository(DatabaseContext context) => this.context = context;

    public Task GetCreateHandlerAsync(ref Entities.Company entity)
    {
        return Task.CompletedTask;
    }
    public Task GetCreateHandlerAsync(ref Entities.Company[] entities, IEqualityComparer<Entities.Company> comparer)
    {
        var exist = GetExist(entities);

        if (exist.Any())
            entities = entities.Except(exist, comparer).ToArray();

        return Task.CompletedTask;
    }
    
    public Task GetUpdateHandlerAsync(ref Entities.Company entity)
    {
        var ctxEntity = context.Companies.FindAsync(entity.Id).GetAwaiter().GetResult();

        if (ctxEntity is null)
            throw new NullReferenceException(nameof(ctxEntity));

        ctxEntity.Name = entity.Name;
        ctxEntity.IndustryId = entity.IndustryId;
        ctxEntity.Description = entity.Description;

        entity = ctxEntity;

        return Task.CompletedTask;
    }
    public Task GetUpdateHandlerAsync(ref Entities.Company[] entities)
    {
        var exist = GetExist(entities).ToArrayAsync().GetAwaiter().GetResult();

        var result = exist
            .Join(entities, x => x.Id, y => y.Id,
                (x, y) => (Old: x, New: y))
            .ToArray();

        foreach (var (Old, New) in result)
        {
            Old.Name = New.Name;
            Old.IndustryId = New.IndustryId;
            Old.Description = New.Description;
        }

        entities = result.Select(x => x.Old).ToArray();

        return Task.CompletedTask;
    }

    public Task SetPostProcessAsync(Entities.Company entity) => Task.CompletedTask;
    public Task SetPostProcessAsync(Entities.Company[] entities) => Task.CompletedTask;

    private IQueryable<Entities.Company> GetExist(IEnumerable<Entities.Company> entities)
    {
        var existData = entities
            .GroupBy(x => x.Id)
            .Select(x => x.Key)
            .ToArray();

        return context.Companies.Where(x => existData.Contains(x.Id));
    }
}