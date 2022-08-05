using System;

namespace AM.Services.Common.Contracts.Models.Entity.Interfaces;

public interface IDateIdentity : IPeriod
{
    DateOnly Date { get; set; }
}