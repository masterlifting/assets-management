namespace Shared.Contracts.Settings;

public class SchedulerSettings
{
    private List<DayOfWeek> _workDays;
    public SchedulerSettings() => _workDays = new()
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday
        };

    public bool IsEnable { get; set; }
    public bool IsOnce { get; set; } = false;
    public string? WorkDays { get; set; }
    public string WorkTime { get; set; } = null!;
    public uint RetryMinutes { get; set; }

    public DateTime DateStart { get; set; } = DateTime.UtcNow;
    public DateTime? DateStop { get; set; }

    public bool IsReady(out string info)
    {
        info = string.Empty;
        var now = DateTime.UtcNow;

        if (!IsEnable)
            return false;

        if (DateStart.Date > now.Date)
        {
            info = $"Дата старта задачи '{nameof(DateStart)}' еще не наступила";
            return false;
        }

        if (DateStop.HasValue && DateStop.Value < now)
        {
            info = $"Дата и время остановки задачи '{nameof(DateStop)}' уже наступили";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(WorkDays))
        {
            var workDays = WorkDays.Split(",");
            _workDays = new List<DayOfWeek>(workDays.Length);

            foreach (var number in workDays)
                if (Enum.TryParse<DayOfWeek>(number.Trim(), true, out var workDay))
                    _workDays.Add(workDay);
        }

        if (!_workDays.Contains(now.DayOfWeek))
        {
            info = $"Текущего дня недели нет в списке '{nameof(WorkDays)}'";
            return false;
        }

        if (!IsOnce)
            return true;

        var workTime = TimeOnly.Parse(WorkTime);
        var currentTime = TimeOnly.FromDateTime(now);

        if (workTime.Hour == currentTime.Hour
            && (workTime.Minute <= 0 || workTime.Minute == currentTime.Minute)
            && (workTime.Second <= 0 || workTime.Second == currentTime.Second))
        {
            WorkTime = workTime.AddMinutes(RetryMinutes).ToShortTimeString();
            return true;
        }

        info = $"Установленное время выполнения задачи c включенной настройкой '{nameof(IsOnce)}' не совпало";
        return false;
    }
}