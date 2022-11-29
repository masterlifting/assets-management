﻿namespace Shared.Persistense.Abstractions;

public static class Constants
{
    public static class Enums
    {
        public enum ProcessableEntityStatuses
        {
            Error = -1,
            Draft = 1,
            Ready,
            Processing,
            Processed
        }
    }
}