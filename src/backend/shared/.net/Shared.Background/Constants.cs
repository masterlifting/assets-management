namespace Shared.Background;

public static class Constants
{
    internal static class Actions
    {
        internal const string Start = "Запущено";
        internal const string Done = "Выполнено";
        internal const string Stop = "Остановлено";
        internal const string NoConfig = "Конфигурация задачи не найдена";
        internal const string Limit = "Установлено максимальное количество объектов для обработки";
        internal const string NextStart = "Следующий запуск через: ";

        internal const string NoData = "Данные отсутствуют";
        internal const string Success = "Успешно";
        internal static class EntityStates
        {
            internal const string PrepareNewData = ". Подготовка новых данных для обработки";
            internal const string PrepareUnhandledData = ". Повторная подготовка данных для обработки";
            internal const string PrepareData = ". Подготовка данных для обработки";
            internal const string GetData = ". Получение данных";
            internal const string HandleData = ". Обработка полученных данных";
            internal const string UpdateData = ". Обновление обработанных данных";
        }
    }
}