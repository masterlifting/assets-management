using System.Threading.Tasks;

using AM.Services.Common.Contracts.Models.RabbitMq.Api;
using AM.Services.Portfolio.Services.Entity;
using Shared.Core.Extensions;

namespace AM.Services.Portfolio.ControllersMq;

public class MqSyncController
{
    private readonly AssetService _service;
    public MqSyncController(AssetService service) => _service = service;

    //public Task SyncAsset(string payload)
    //{
    //    var asset = JsonSerializerExtensions.Deserialize<AssetMqDto>(payload);
    //}
}