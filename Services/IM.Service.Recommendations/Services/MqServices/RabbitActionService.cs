﻿using System.Collections.Generic;
using IM.Service.Common.Net.RabbitServices;
using IM.Service.Recommendations.DataAccess.Entities;
using IM.Service.Recommendations.DataAccess.Repository;
using IM.Service.Recommendations.Services.MqServices.Implementations;

namespace IM.Service.Recommendations.Services.MqServices
{
    public class RabbitActionService : RabbitService
    {
        public RabbitActionService( RepositorySet<Company> companyRepository) : base(
            new Dictionary<QueueExchanges, IRabbitActionService>
            {
                { QueueExchanges.Sync, new RabbitSyncService(companyRepository) },
            })
        { }
    }
}