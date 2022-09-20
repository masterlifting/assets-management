namespace Shared.Background.Settings;

public sealed class SchedulerSettings
{
    private bool _isOnce;
    private List<int> _workDays;
    public SchedulerSettings() => _workDays = new() { 1, 2, 3, 4, 5, 6, 7 };

    public bool IsEnable { get; set; } = false;
    public bool IsOnce { get; set; } = false;
    public string WorkDays { get; set; } = "1,2,3,4,5,6,7";
    public string WorkTime { get; set; } = "00:10:00";

    public DateTime DateTimeStart { get; set; } = DateTime.UtcNow;
    public DateTime? DateTimeStop { get; set; }

    public bool IsReady(out string info)
    {
        info = string.Empty;
        var now = DateTime.UtcNow;

        if (!IsEnable)
        {
            info = $"Задача выключена настройкой '{nameof(IsEnable)}'";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(WorkDays))
        {
            var workDays = WorkDays.Split(",");
            _workDays = new List<int>(workDays.Length);

            foreach (var number in workDays)
                if (int.TryParse(number.Trim(), out var workDay))
                    _workDays.Add(workDay);
        }

        var dayNowNumber = now.DayOfWeek == 0 ? 7 : (int)now.DayOfWeek;
        if (!_workDays.Contains(dayNowNumber))
        {
            info = $"Текущего дня недели нет в списке '{nameof(WorkDays)}'";
            return false;
        }

        return true;
    }
    public bool IsStart(out string info)
    {
        info = string.Empty;
        var now = DateTime.UtcNow;

        if (DateTimeStart > now)
        {
            info = $"Дата старта задачи '{nameof(DateTimeStart)}: {DateTimeStart: yyyy-MM-dd HH:mm:ss}' еще не наступила";
            return false;
        }

        return true;
    }
    public bool IsStop(out string info)
    {
        info = string.Empty;
        var now = DateTime.UtcNow;

        if (DateTimeStop.HasValue && DateTimeStop.Value < now)
        {
            info = $"Дата и время остановки задачи указанные в настройке '{nameof(DateTimeStop)}: {DateTimeStop: yyyy-MM-dd HH:mm:ss}' наступили";
            return true;
        }

        if (IsOnce && _isOnce)
        {
            info = $"Однократный запуск задачи установлен настройкой '{nameof(IsOnce)}'";
            return true;
        }

        return false;
    }
    public void SetOnce() => _isOnce = true;
}