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
            Loading,
            Parsing,
            Computing,
            Sending
        }
        public enum ContentTypes
        {
            Excel,
            Html
        }
        public enum Comparisons
        {
            Equal,
            More,
            Less
        }
    }
    public static class Catalogs
    {
        public static readonly Catalog[] States =
        {
            new() {Id = (int) Enums.States.Ready, Name = nameof(Enums.States.Ready), Info = "Готов к обработке" },
            new() {Id = (int) Enums.States.Processing, Name = nameof(Enums.States.Processing), Info = "Обрабатывается" },
            new() {Id = (int) Enums.States.Processed, Name = nameof(Enums.States.Processed), Info = "Обработан" },
            new() {Id = (int) Enums.States.Error, Name = nameof(Enums.States.Error), Info = "Ошибка обработки" }
        };
        public static readonly Catalog[] Steps =
        {
            new() {Id = (int) Enums.Steps.Loading, Name = nameof(Enums.Steps.Loading), Info = "Загрузка" },
            new() {Id = (int) Enums.Steps.Parsing, Name = nameof(Enums.Steps.Parsing), Info = "Парсинг" },
            new() {Id = (int) Enums.Steps.Computing, Name = nameof(Enums.Steps.Computing), Info = "Вычисления" },
            new() {Id = (int) Enums.Steps.Sending, Name = nameof(Enums.Steps.Sending), Info = "Отправка" }
        };
        public static readonly Catalog[] ContentTypes =
        {
            new() {Id = (int) Enums.ContentTypes.Excel, Name = nameof(Enums.ContentTypes.Excel), Info = "Excel файл" },
            new() {Id = (int) Enums.ContentTypes.Html, Name = nameof(Enums.ContentTypes.Html), Info = "HTML страница" }
        };

        public static readonly Dictionary<string, Enums.ContentTypes> ContentTypeDictionary = new()
        {
            {"", Enums.ContentTypes.Excel}
        };
    }
}