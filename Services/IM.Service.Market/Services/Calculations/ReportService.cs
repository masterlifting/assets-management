﻿using IM.Service.Market.Domain.DataAccess;
using IM.Service.Market.Domain.Entities;

namespace IM.Service.Market.Services.Calculations;

public sealed class ReportService : ChangeStatusService<Report>
{
    public ReportService(Repository<Report> repository) : base(repository)
    {
    }
}