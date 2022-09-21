namespace Shared.MessagesQueue
{
    public static class Constants
    {
        internal const int Limit = 10_000;
        internal static class Actions
        {
            internal const string Connect = "Подключение";
            internal const string Disconnect = "Отключение";
            internal const string Start = "Запущено";
            internal const string Done = "Завершено";
            internal const string Success = "Успешно";
            internal const string Post = "Отправка";
        }
        public static class Enums
        {
            public static class RabbitMq
            {
                public enum ExchangeNames
                {
                    In,
                    Apc,
                    Sync
                }
                public enum ExchangeTypes
                {
                    Direct,
                    Fannout,
                    Topic,
                    Headers
                }
            }
        }
    }
}