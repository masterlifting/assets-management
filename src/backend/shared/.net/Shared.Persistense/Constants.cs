using Shared.Persistense.Entities;

namespace Shared.Persistense;

public static class Constants
{
    internal static class Actions
    {
        internal const string Start = "Запущено";
        internal const string Success = "Успешно";

        internal const string Create = "Добавление";
        internal const string Update = "Обновление";
        internal const string Delete = "Удаление";
        internal const string NoData = "Отсутствуют данные для операции";

        internal static class EntityStates
        {
            internal const string PrepareNewData = ". Подготовка новых данных для обработки";
            internal const string PrepareUnhandledData = ". Подготовка необработанных данных для обработки";
            internal const string PrepareData = ". Подготовка данных для обработки";
            internal const string GetData = ". Получение данных";
            internal const string HandleData = ". Обработка полученных данных";
            internal const string UpdateData = ". Обновление обработанных данных";
        }
    }
    public static class Enums
    {
        public enum States
        {
            Ready,
            Processing,
            Processed,
            Error = -1
        }
        public enum Steps
        {
            Parsing,
            Calculating,
            Sending
        }
        public enum Comparisons
        {
            Equal,
            More,
            Less
        }
        public enum ContentTypes
        {
            Excel,
            Html
        }
    }
    public static class Catalogs
    {
        public static readonly Catalog[] States =
        {
        new() {Id = (int) Enums.States.Ready, Name = nameof(Enums.States.Ready), Description = "Готов к обработке" },
        new() {Id = (int) Enums.States.Processing, Name = nameof(Enums.States.Processing), Description = "Обрабатывается" },
        new() {Id = (int) Enums.States.Processed, Name = nameof(Enums.States.Processed), Description = "Обработан" },
        new() {Id = (int) Enums.States.Error, Name = nameof(Enums.States.Error), Description = "Ошибка" },
    };
        public static readonly Catalog[] Steps =
        {
        new() {Id = (int) Enums.States.Ready, Name = nameof(Enums.States.Ready), Description = "Готов к обработке" },
        new() {Id = (int) Enums.States.Processing, Name = nameof(Enums.States.Processing), Description = "Обрабатывается" },
        new() {Id = (int) Enums.States.Processed, Name = nameof(Enums.States.Processed), Description = "Обработан" },
        new() {Id = (int) Enums.States.Error, Name = nameof(Enums.States.Error), Description = "Ошибка" },
        };
    }
}