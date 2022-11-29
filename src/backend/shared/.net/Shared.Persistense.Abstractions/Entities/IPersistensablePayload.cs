namespace Shared.Persistense.Abstractions.Entities;

public interface IPersistensablePayload : IPersistensable
{
    byte[] Payload { get; init; }
    byte[] PayloadHash { get; init; }
    string PayloadSource { get; init; }
    string PayloadContentType { get; init; }
    string PayloadHashAlgoritm { get; init; }
}