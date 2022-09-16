using AM.Services.Portfolio.Core.Domain.Persistense.Entities.States;
using AM.Services.Portfolio.Core.Domain.Persistense.Models;

namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Parsing.Reports.Bcs.Models;

public class BcsReportBody
{
    public int AccountId { get; }
    public IEnumerable<Deal> Deals { get; }
    public IEnumerable<Event> Events { get; }

    private readonly List<Deal> _deals;
    private readonly List<Event> _events;
    public BcsReportBody(int accountId, int capacity)
    {
        AccountId = accountId;

        _deals = new List<Deal>(capacity);
        _events = new List<Event>(capacity);

        Deals = _deals;
        Events = _events;
    }

    public void AddDeal(DealModel model) => _deals.Add(model.GetEntity());
    public void AddEvent(EventModel model) => _events.Add(model.GetEntity());
}