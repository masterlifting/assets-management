﻿using System.ComponentModel.DataAnnotations;

namespace CommonServices.Attributes
{
    public class NotZeroAttribute : ValidationAttribute
    {
        private readonly string property;
        public NotZeroAttribute(string property) => this.property = property;

        public override bool IsValid(object? value)
        {
            var stringValue = value?.ToString();

            var isParse = decimal.TryParse(stringValue, out var result);

            ErrorMessage = $"The '{property}' must be greater than 0";

            return isParse && result > 0;
        }
    }
}
