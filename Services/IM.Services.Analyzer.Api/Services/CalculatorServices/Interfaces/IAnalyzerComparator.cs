﻿namespace IM.Services.Analyzer.Api.Services.CalculatorServices.Interfaces
{
    public interface IAnalyzerComparator<T> where T : class
    {
        public T[] GetCoparedSample();
    }
}
