﻿using AM.Services.Portfolio.Core.Domain.Persistense.Entities.EntityState;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Background.Abstractions.EntityState;
using Shared.Background.Settings.Sections;

namespace AM.Services.Portfolio.Host.Services.Background.EntityState;

public sealed class ReportDataBackgroundService : EntityStateBackgroundService<ReportData>
{
    public ReportDataBackgroundService(IServiceScopeFactory scopeFactory, IOptionsMonitor<BackgroundTaskSection> options, ILogger<ReportDataBackgroundService> logger)
         : base(options, logger, scopeFactory) { }
}