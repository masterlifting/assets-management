namespace Shared.Background;

internal static class Constants
{
    internal static class Actions
    {
        internal const string Start = "Start";
        internal const string Done = "Done";
        internal const string Stop = "Stop";
        internal const string NoConfig = "Task configuration not found";
        internal const string Limit = "Size of data for processing reached the limit";
        internal const string NextStart = "Next start over: ";

        internal const string NoData = "Data for processing not found";
        internal const string Success = "Success";
        internal static class ProcessableActions
        {
            internal const string GetProcessableData = ". Getting data to processing";
            internal const string GetUnprocessableData = ". Repeated getting data to processing";
            internal const string HandleProcessableData = ". Processing received data";
            internal const string SaveProcessableData = ". Save processed result";
        }
    }
}