using Shared.Persistense.Entities;

namespace Shared.Persistense;

public static class Constants
{
    internal const string Create = "Добавление";
    internal const string Update = "Обновление";
    internal const string Delete = "Удаление";
    internal const string NoData = "Отсутствуют данные для операции";
    internal const string Success = "Успешно";

    public static readonly Catalog[] States =
    {
        new() {Id = (int) Enums.States.Ready, Name = nameof(Enums.States.Ready), Description = "Готов к обработке" },
        new() {Id = (int) Enums.States.Processing, Name = nameof(Enums.States.Processing), Description = "Обрабатывается" },
        new() {Id = (int) Enums.States.Processed, Name = nameof(Enums.States.Processed), Description = "Обработан" },
        new() {Id = (int) Enums.States.Error, Name = nameof(Enums.States.Error), Description = "Ошибка" },
    };
}