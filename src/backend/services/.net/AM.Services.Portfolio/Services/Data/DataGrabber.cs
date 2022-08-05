using System.Collections.Generic;
using System.Threading.Tasks;
using AM.Services.Portfolio.Models.Api.Mq;
using static AM.Services.Portfolio.Enums;

namespace AM.Services.Portfolio.Services.Data;

public abstract class DataGrabber
{
    private readonly Dictionary<Providers, IDataGrabber> grabber;
    protected DataGrabber(Dictionary<Providers, IDataGrabber> grabber) => this.grabber = grabber;

    public Task ProcessAsync(ProviderReportDto report, Providers provider) => grabber.ContainsKey(provider) 
        ? grabber[provider].ProcessAsync(report) 
        : Task.CompletedTask;
}