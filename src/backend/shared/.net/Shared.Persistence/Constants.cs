namespace Shared.Persistence;

public static class Constants
{
    public static class Actions
    {
        internal const string Success = "Success";

        public const string Create = "Creating";
        public const string Update = "Updating";
        public const string Delete = "Deleting";
        public const string NoData = "Data not found";
    }
    public static class Enums
    {
        public enum Comparisons
        {
            Equal,
            More,
            Less
        }
    }
}