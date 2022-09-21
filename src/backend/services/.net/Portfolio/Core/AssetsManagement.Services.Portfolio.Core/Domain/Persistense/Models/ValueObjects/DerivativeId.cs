﻿using AM.Services.Portfolio.Core.Exceptions;

namespace AM.Services.Portfolio.Core.Domain.Persistense.Models.ValueObjects
{
    public sealed record DerivativeId
    {
        public string AsString { get; }

        public DerivativeId(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 20)
                throw new PortfolioCoreException("", "", $"Не удалось установить идентификатор дериватива по значению: {value}");

            AsString = value.ToUpper();
        }
    }
}