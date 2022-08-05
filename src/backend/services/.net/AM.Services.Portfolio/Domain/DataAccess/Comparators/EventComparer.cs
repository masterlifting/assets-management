using System.Collections.Generic;
using AM.Services.Portfolio.Domain.Entities;

namespace AM.Services.Portfolio.Domain.DataAccess.Comparators;

public class EventComparer : IEqualityComparer<Event>
{
    public bool Equals(Event? x, Event? y) => x!.Id != 0 && x.Id == y!.Id;
    public int GetHashCode(Event obj) => obj.Id.GetHashCode();
}