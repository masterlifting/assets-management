using System.Threading.Tasks;
using AM.Services.Portfolio.Models.Api.Mq;

namespace AM.Services.Portfolio.Services.Data;

public interface IDataGrabber
{
    Task ProcessAsync(ProviderReportDto report);
}