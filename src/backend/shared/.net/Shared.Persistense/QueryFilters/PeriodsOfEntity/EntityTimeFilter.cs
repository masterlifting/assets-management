using Shared.Persistense.Abstractions.Entities;
using Shared.Persistense.Abstractions.Entities.PeriodsOfEntity;

using System.Linq.Expressions;

using static Shared.Persistense.Constants.Enums;

namespace Shared.Persistense.QueryFilters.PeriodsOfEntity;

public sealed class EntityTimeFilter<T> : IEntityPeriodFilter<T> where T : class, IEntityTime, IEntity
{
    public int Hour { get; }
    public int Minute { get; }
    public int Second { get; }

    public Expression<Func<T, bool>> Predicate { get; set; }

    public EntityTimeFilter(Comparisons comparisons, int hour)
    {
        Hour = hour;
        Minute = 1;
        Second = 1;

        Predicate = comparisons switch
        {
            Comparisons.Equal => x => x.Time.Hour == Hour,
            Comparisons.More => x => x.Time.Hour >= Hour,
            Comparisons.Less => x => x.Time.Hour <= Hour,
            _ => throw new ArgumentOutOfRangeException(nameof(comparisons), comparisons, null)
        };
    }
    public EntityTimeFilter(Comparisons comparisons, int hour, int minute)
    {
        Hour = hour;
        Minute = minute;
        Second = 1;

        Predicate = comparisons switch
        {
            Comparisons.Equal => x => x.Time.Hour == Hour && x.Time.Minute == Minute,
            Comparisons.More => x => x.Time.Hour > Hour || x.Time.Hour == Hour && x.Time.Minute >= Minute,
            Comparisons.Less => x => x.Time.Hour < Hour || x.Time.Hour == Hour && x.Time.Minute <= Minute,
            _ => throw new ArgumentOutOfRangeException(nameof(comparisons), comparisons, null)
        };
    }
    public EntityTimeFilter(Comparisons comparisons, int hour, int minute, int second)
    {
        Hour = hour;
        Minute = minute;
        Second = second;

        Predicate = comparisons switch
        {
            Comparisons.Equal => x => x.Time.Hour == Hour && x.Time.Minute == Minute && x.Time.Second == Second,
            Comparisons.More => x => x.Time.Hour > Hour || x.Time.Hour == Hour && (x.Time.Minute == Minute && x.Time.Second >= Second || x.Time.Minute > Minute),
            Comparisons.Less => x => x.Time.Hour < Hour || x.Time.Hour == Hour && (x.Time.Minute == Minute && x.Time.Second <= Second || x.Time.Minute < Minute),
            _ => throw new ArgumentOutOfRangeException(nameof(comparisons), comparisons, null)
        };
    }
}