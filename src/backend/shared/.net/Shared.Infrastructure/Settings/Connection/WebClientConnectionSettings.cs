﻿namespace Shared.Infrastructure.Settings.Connection;

public class WebClientConnectionSettings
{
    public string Schema { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string? ApiKey { get; set; }
}