﻿using System.Collections.Generic;

using IM.Service.Company.Analyzer.DataAccess.Entities;

namespace IM.Service.Company.Analyzer.DataAccess.Comparators;

public class AnalyzedEntityComparer : IEqualityComparer<AnalyzedEntity>
{
    public bool Equals(AnalyzedEntity? x, AnalyzedEntity? y) =>(x!.CompanyId, x.AnalyzedEntityTypeId, x.Date) == (y!.CompanyId, y.AnalyzedEntityTypeId, y.Date);
    public int GetHashCode(AnalyzedEntity obj) => (obj.CompanyId, obj.AnalyzedEntityTypeId, obj.Date).GetHashCode();
}