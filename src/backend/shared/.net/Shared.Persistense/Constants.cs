using Shared.Persistense.Entities;

using static Shared.Persistense.Constants.Enums;

namespace Shared.Persistense;

public static class Constants
{
    public static class Actions
    {
        public const string Start = "Was start";
        public const string Success = "Success";

        public const string Create = "Creating";
        public const string Update = "Updating";
        public const string Delete = "Deleting";
        public const string NoData = "Data not found";
        public static class ValueObject
        {
            public const string Set = "Setting ValueObject properties";
            public static string ValueNotValid(string? value) => value is null ? "Value not found" : $"Value: '{value}' not valid";
            public static string ValueNotValid(int value) => $"Value: '{value}' not valid";
        }
        public static class EntityState
        {
            public static string StepNotImplemented(string name) => $"Step: '{name}' not implemented'";
            public static string StepNotFound(Steps step) => $"Step: '{step}' not found'";
        }
    }
    public static class Enums
    {
        public enum States
        {
            Ready = 1,
            Processing,
            Processed,
            Error = -1
        }
        public enum Steps
        {
            Loading = 1,
            Parsing,
            Serialization,
            Computing,
            Sending
        }
        public enum ContentTypes
        {
            Excel = 1,
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
            new((int) Enums.States.Ready, nameof(Enums.States.Ready)) { Info = "Ready to processing data" },
            new((int) Enums.States.Processing, nameof(Enums.States.Processing)) { Info = "Processing data" },
            new((int) Enums.States.Processed, nameof(Enums.States.Processed)) { Info = "Processed data" },
            new((int) Enums.States.Error, nameof(Enums.States.Error)) { Info = "Error of processing" }
        };
        public static readonly Catalog[] Steps =
        {
            new((int) Enums.Steps.Loading, nameof(Enums.Steps.Loading)),
            new((int) Enums.Steps.Parsing, nameof(Enums.Steps.Parsing)),
            new((int) Enums.Steps.Serialization, nameof(Enums.Steps.Serialization)),
            new((int) Enums.Steps.Computing, nameof(Enums.Steps.Computing)),
            new((int) Enums.Steps.Sending, nameof(Enums.Steps.Sending))
        };
        public static readonly Catalog[] ContentTypes =
        {
            new((int) Enums.ContentTypes.Excel, nameof(Enums.ContentTypes.Excel)) { Info = "Excel file" },
            new((int) Enums.ContentTypes.Html, nameof(Enums.ContentTypes.Html)) { Info = "HTML page" }
        };

        public static readonly Dictionary<string, ContentTypes> ContentTypeDictionary = new()
        {
            {"application/vnd.ms-excel", Enums.ContentTypes.Excel}
        };
    }
}