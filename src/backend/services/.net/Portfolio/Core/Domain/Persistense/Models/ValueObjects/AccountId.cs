using AM.Services.Portfolio.Core.Exceptions;

using static Shared.Persistense.Constants;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects;

public sealed record AccountId
{
    public int AsInt { get; }

    public AccountId(int value)
    {
        if (value <= 0)
            throw new PortfolioCoreException(nameof(AccountId), Actions.ValueObject.Set, new(Actions.ValueObject.ValueNotValidError(value)));

        AsInt = value;
    }

    public AccountId(string value, IDictionary<string, int> accountDictionary)
    {
        if (!accountDictionary.ContainsKey(value))
            throw new PortfolioCoreException(nameof(AccountId), Actions.ValueObject.Set, new(Actions.ValueObject.ValueNotValidError(value)));
        
        var asInt = accountDictionary[value];
        
        if (asInt <= 0)
            throw new PortfolioCoreException(nameof(AccountId), Actions.ValueObject.Set, new(Actions.ValueObject.ValueNotValidError(value)));
        
        AsInt = asInt;
    }
}