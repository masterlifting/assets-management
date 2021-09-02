﻿using CommonServices;

using IM.Services.Companies.Reports.Api.DataAccess.Entities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace IM.Services.Companies.Reports.Api.Services.ReportServices
{
    public static class ReportLoaderHelper
    {
        public static IEnumerable<Report> GetReportsWithoutLastQuarter(IEnumerable<Report> lastReports) => lastReports.Where(x => IsMissingLastQuarter(x.Year, x.Quarter));
        public static Report FindLastReport(IEnumerable<Report> reports) => reports.GroupBy(x => x.Year).OrderBy(x => x.Key).Last().OrderBy(x => x.Quarter).Last();
        public static (Report[] toAdd, Report[] toUpdate) SeparateReports(Report[] loadedReports, Report lastReport)
        {
            var reportsToAdd = Array.Empty<Report>();
            var reportsToUpdate = Array.Empty<Report>();

            if (loadedReports.Any())
            {
                reportsToAdd = loadedReports.Where(x => IsNewQuarter((x.Year, x.Quarter), (lastReport.Year, lastReport.Quarter))).ToArray();
                reportsToUpdate = loadedReports.Where(x => !IsNewQuarter((x.Year, x.Quarter), (lastReport.Year, lastReport.Quarter))).ToArray();
            }

            return (reportsToAdd, reportsToUpdate);
        }
        public static bool IsMissingLastQuarter(int lastYear, byte lastQuarter)
        {
            var (controlYear, controlQuarter) = CommonHelper.SubstractQuarter(DateTime.UtcNow);
            return IsNewQuarter((controlYear, controlQuarter), (lastYear, lastQuarter));
        }
        public static bool IsNewQuarter((int year, byte quarter) current, (int year, byte qarter) last) =>
            current.year > last.year
            || (current.year == last.year && current.quarter > last.qarter);
    }
}
