﻿using System.Text.Json.Serialization;

using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;

using Shared.Persistense.Entities;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Entities.Catalogs;

public class Provider : Catalog
{
    [JsonIgnore]
    public virtual IEnumerable<Deal>? Deals { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<Event>? Events { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<Account>? Accounts { get; set; }
    [JsonIgnore]
    public virtual IEnumerable<Report>? ReportFiles { get; set; }
}