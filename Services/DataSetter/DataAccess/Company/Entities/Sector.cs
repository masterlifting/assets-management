﻿using IM.Service.Common.Net.Models.Entity;

using System.Collections.Generic;

namespace DataSetter.DataAccess.Company.Entities;

public class Sector : CommonEntityType
{
    public virtual IEnumerable<Industry>? Industries { get; set; }
}