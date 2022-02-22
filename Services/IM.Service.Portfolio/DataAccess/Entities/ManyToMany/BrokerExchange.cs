﻿using IM.Service.Portfolio.DataAccess.Entities.Catalogs;

namespace IM.Service.Portfolio.DataAccess.Entities.ManyToMany;

public class BrokerExchange
{
    public virtual Broker Broker { get; init; } = null!;
    public byte BrokerId { get; init; }

    public virtual Exchange Exchange { get; init; } = null!;
    public byte ExchangeId { get; init; }
}