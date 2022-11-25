using Shared.Persistense.Abstractions.Entities.Catalogs;
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
            public const string Validate = "Validation property";
            public static string ValueNotValidError(string? value) => value is null ? "Value was not found" : $"Value: '{value}' is not valid";
            public static string ValueNotValidError(int value) => $"Value: '{value}' is not valid";
        }
    }
    public static class Enums
    {
        public enum Statuses
        {
            Draft = 1,
            Ready,
            Processing,
            Processed,
            Error = -1
        }
        public enum Steps
        {
            Loading = 1,
            Parsing,
            Serialization,
            Validation,
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
        public static readonly Catalog[] Statuses =
        {
        new((int) Enums.Statuses.Draft, nameof(Enums.Statuses.Draft)) { Info = "Draft" },
        new((int) Enums.Statuses.Ready, nameof(Enums.Statuses.Ready)) { Info = "Ready to processing data" },
        new((int) Enums.Statuses.Processing, nameof(Enums.Statuses.Processing)) { Info = "Processing data" },
        new((int) Enums.Statuses.Processed, nameof(Enums.Statuses.Processed)) { Info = "Processed data" },
        new((int) Enums.Statuses.Error, nameof(Enums.Statuses.Error)) { Info = "Error of processing" }
    };
        public static readonly Catalog[] Steps =
        {
        new((int) Enums.Steps.Loading, nameof(Enums.Steps.Loading)),
        new((int) Enums.Steps.Parsing, nameof(Enums.Steps.Parsing)),
        new((int) Enums.Steps.Serialization, nameof(Enums.Steps.Serialization)),
        new((int) Enums.Steps.Validation, nameof(Enums.Steps.Validation)),
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