﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using AM.Services.Common.Contracts.Models.Entity.Interfaces;

namespace AM.Services.Common.Contracts.Models.Entity;

public class Country<TAsset> : Catalog, ICountry<TAsset> where TAsset : class, IAsset 
{
    [JsonIgnore]
    public virtual IEnumerable<TAsset>? Assets { get; set; }
}