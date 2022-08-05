using System.Collections.Generic;
using AM.Services.Common.Contracts.RabbitMq;
using AM.Services.Portfolio.Models.Api.Mq;
using AM.Services.Portfolio.Services.RabbitMq;
using Microsoft.AspNetCore.Http;

namespace AM.Services.Portfolio.Services.Http;

public class ReportApi
{
    private readonly RabbitAction rabbitAction;
    public ReportApi(RabbitAction rabbitAction) => this.rabbitAction = rabbitAction;

    public string Load(IFormFileCollection files, string userId)
    {
        var queueTaskParams = new List<(QueueNames, QueueEntities, QueueActions, object)>(files.Count);

        foreach (var file in files)
        {
            var payload = new byte[file.Length];
            using var stream = file.OpenReadStream();
            var _ = stream.Read(payload, 0, (int)file.Length);

            var data = new ProviderReportDto(file.FileName, file.ContentType, payload, userId);
            queueTaskParams.Add((QueueNames.Portfolio, QueueEntities.Report, QueueActions.Get, data));
        }

        rabbitAction.Publish(QueueExchanges.Function, queueTaskParams);

        return $"Load files (count: {files.Count}) is running...";
    }
}