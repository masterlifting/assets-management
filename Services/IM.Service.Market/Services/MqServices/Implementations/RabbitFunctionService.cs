﻿using IM.Service.Common.Net;
using IM.Service.Common.Net.RabbitServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;
using IM.Service.Common.Net.RabbitServices.Configuration;
using IM.Service.Market.Services.DataServices.Prices;
using IM.Service.Market.Services.DataServices.Reports;
using IM.Service.Market.Services.DataServices.StockSplits;
using IM.Service.Market.Services.DataServices.StockVolumes;

namespace IM.Service.Market.Services.MqServices.Implementations;

public class RabbitFunctionService : IRabbitActionService
{
    private readonly IServiceScopeFactory scopeFactory;
    public RabbitFunctionService(IServiceScopeFactory scopeFactory) => this.scopeFactory = scopeFactory;

    public async Task<bool> GetActionResultAsync(QueueEntities entity, QueueActions action, string companyId)
    {
        if (action != QueueActions.Get)
            return true;

        try
        {
            switch (entity)
            {
                case QueueEntities.Price:
                    {
                        var priceLoader = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<PriceLoader>();
                        await priceLoader.DataSetAsync(companyId);
                        break;
                    }
                case QueueEntities.Prices:
                    {
                        var priceLoader = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<PriceLoader>();
                        await priceLoader.DataSetAsync();
                        break;
                    }
                case QueueEntities.Report:
                    {
                        var reportLoader = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ReportLoader>();
                        await reportLoader.DataSetAsync(companyId);
                        break;
                    }
                case QueueEntities.Reports:
                    {
                        var reportLoader = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ReportLoader>();
                        await reportLoader.DataSetAsync();
                        break;
                    }
                case QueueEntities.Split:
                    {
                        var reportLoader = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<StockSplitLoader>();
                        await reportLoader.DataSetAsync(companyId);
                        break;
                    }
                case QueueEntities.Splits:
                    {
                        var reportLoader = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<StockSplitLoader>();
                        await reportLoader.DataSetAsync();
                        break;
                    }
                case QueueEntities.Float:
                    {
                        var reportLoader = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<StockVolumeLoader>();
                        await reportLoader.DataSetAsync(companyId);
                        break;
                    }
                case QueueEntities.Floats:
                    {
                        var reportLoader = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<StockVolumeLoader>();
                        await reportLoader.DataSetAsync();
                        break;
                    }
            }

            return true;
        }
        catch (Exception exception)
        {
            var logger = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogger<RabbitFunctionService>>();
            logger.LogError(LogEvents.Function, "Entity: {entity} Queue action: {action} failed! \nError: {error}", Enum.GetName(entity), action, exception.Message);
            return false;
        }
    }
}