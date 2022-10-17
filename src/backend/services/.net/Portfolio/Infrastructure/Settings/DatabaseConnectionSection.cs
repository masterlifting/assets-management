﻿using Shared.Persistense.Settings.Connections;

namespace AM.Services.Portfolio.Infrastructure.Settings;

public sealed class DatabaseConnectionSection
{
    public const string Name = "DatabaseConnections";
    public PostgreConnectionSettings Postgres { get; set; } = null!;
}