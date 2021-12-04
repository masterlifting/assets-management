﻿using IM.Service.Company.Analyzer.DataAccess.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace IM.Service.Company.Analyzer.Services.CalculatorServices.Interfaces;

public interface IAnalyzerCalculator
{
    Task<AnalyzedEntity[]> ComputeAsync(IEnumerable<AnalyzedEntity> data);
}