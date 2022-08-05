using AM.Services.Portfolio.Domain.DataAccess;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Services.Data.Reports.Implementations;
using Microsoft.Extensions.Logging;
using static AM.Services.Portfolio.Enums;

namespace AM.Services.Portfolio.Services.Data.Reports;

public sealed class ReportGrabber : DataGrabber
{
    public ReportGrabber(
        Repository<Account> accountRepo,
        Repository<Derivative> derivativeRepo,
        Repository<Deal> dealRepo,
        Repository<Event> eventRepo,
        Repository<Report> reportRepo,
        ILogger<ReportGrabber> logger)
        : base(new()
        {
            { Providers.BCS, new BcsReportGrabber(accountRepo, derivativeRepo, dealRepo, eventRepo, reportRepo, logger) }
        }) { }
}