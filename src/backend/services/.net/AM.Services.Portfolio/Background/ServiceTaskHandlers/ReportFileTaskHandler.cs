using AM.Services.Portfolio.Domain.DataAccess;
using AM.Services.Portfolio.Domain.Entities;
using AM.Services.Portfolio.Services.ReportService;

using Microsoft.EntityFrameworkCore;

using Shared.Contracts.Domains.Entities;
using Shared.Contracts.Settings;
using Shared.Core.Abstractions.Background.EntityState;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static AM.Services.Portfolio.Enums;

namespace AM.Services.Portfolio.Background.ServiceTaskHandlers;

public class ReportFileTaskHandler : IEntityStateBackgroundTaskHandler<ReportFile, BackgroundTaskSettings>
{
    private readonly DatabaseContext _context;
    private readonly IReportParserService _reportParser;

    public ReportFileTaskHandler(
        DatabaseContext context
        , IReportParserService reportParser)
    {
        _context = context;
        _reportParser = reportParser;
    }

    public async Task<long[]> GetProcessingIdsAsync(byte stepId, BackgroundTaskSettings settings, CancellationToken cToken)
    {
        LongId[] result;

        if (settings.Retry is null)
            result = await _context.LongIds.FromSqlInterpolated(@$"
                DECLARE 
                    @StepId TINYINT = {stepId}
                    , @Limit INT = {settings.Limit}
                    , @UpdatedIds TABLE (Id BIGINT)
                UPDATE TOP (@Limit) ReportFiles SET
	                StateId = 2, 
	                UpdateTime = GETDATE(),
	                OUTPUT INSERTED.Id INTO @UpdatedIds
		        WHERE StepId = @StepId AND StateId = 1 
		        SELECT Id FROM @UpdatedIds").ToArrayAsync(cToken);
        else
        {
            var dateNow = DateTime.UtcNow;
            var unprocessedDate = dateNow.Add(-TimeSpan.FromMinutes(settings.Retry!.Minutes));

            result = await _context.LongIds.FromSqlInterpolated(@$"
                DECLARE 
                    @StepId TINYINT = {stepId}
                    , @Limit INT = {settings.Limit}
                    , @UpdateDate DATETIME2 = {unprocessedDate}
                    , @MaxAttempts INT = {settings.Retry.MaxAttempts}
                    , @UpdatedIds TABLE (Id BIGINT)
                UPDATE TOP (@Limit) ReportFiles SET
	                StateId = 2, 
	                Attempt = Attempt+1,
	                UpdateDate = GETDATE()
	                OUTPUT INSERTED.Id INTO @UpdatedIds
		        WHERE 
			        StepId = @StepId 
			        AND ((StateId = 2 AND UpdateDate < @UpdateDate) OR (StateId = 255))
			        AND Attempt < @MaxAttempts
                SELECT Id FROM @UpdatedIds").ToArrayAsync(cToken);
        }

        return result.Select(x => x.Id).ToArray();
    }
    public Task<ReportFile[]> GetEntitiesAsync(byte stepId, long[] ids, CancellationToken cToken) =>
        _context.ReportFiles.Where(x => x.StepId == stepId && ids.Contains(x.Id)).ToArrayAsync(cToken);
    public Task HandleEntitiesAsync(byte stepId, IEnumerable<ReportFile> entities, CancellationToken cToken) => stepId switch
    {
        (byte)Steps.Parsing => _reportParser.ParseAsync(entities, cToken),
        _ => throw new ArgumentOutOfRangeException(nameof(stepId), stepId, null)
    };
    public async Task UpdateEntitiesAsync(byte? stepId, IEnumerable<ReportFile> entities, CancellationToken cToken)
    {
        if (stepId.HasValue)
            foreach (var entity in entities)
            {
                entity.StepId = stepId.Value;
                entity.UpdateTime = DateTime.UtcNow;
            }

        _context.UpdateRange(entities, cToken);
        await _context.SaveChangesAsync(cToken);
    }
}