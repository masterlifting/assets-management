using System;

namespace AM.Services.Recommendations.Settings.Sections;

public class SaleSettings
{
    public decimal[] Profits { get; set; } = Array.Empty<decimal>();
}