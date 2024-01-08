namespace Shared.Queue;

public static class Constants
{
    internal const int Limit = 10_000;
    internal static class Actions
    {
        internal const string Connect = "Connecting";
        internal const string Disconnect = "Disconnecting";
        internal const string Start = "Was start";
        internal const string Done = "Was done";
        internal const string Success = "Success";
        internal const string Post = "Sending";
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
