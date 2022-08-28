namespace Shared.Contracts;

public static class Enums
{
    public enum States : byte
    {
        Ready,
        Processing,
        Processed,
        Error = 255
    }
}