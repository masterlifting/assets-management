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
        internal static class EntityStates
        {
            internal const string PrepareNewData = ". Preparing new data to processing";
            internal const string PrepareUnhandledData = ". Repeated preparing data to processing";
            internal const string PrepareData = ". Preparing data to processing";
            internal const string GetData = ". Receiving data";
            internal const string HandleData = ". Processing received data";
            internal const string UpdateData = ". Updating processed data";
        }
    }
}