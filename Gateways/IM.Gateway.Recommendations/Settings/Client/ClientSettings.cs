﻿namespace IM.Gateway.Recommendations.Settings.Client
{
    public abstract class ClientSettings
    {
        public HostModel CompanyPrices { get; set; } = null!;
        public HostModel CompanyReports { get; set; } = null!;
    }
}
