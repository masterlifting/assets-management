namespace AM.Services.Portfolio.Core.Services.EntityStateService.Steps.Parsing.Reports.Bcs.Models;

public static class BcsReportStructure
{
    internal static readonly string[] Points = {
        "1.1.1. �������� �������� ������� �� ����������� ������� (���� ���������) � ������� ��������",
        "1.2. �����:",
        "�����/������ (�������� �����):",
        "2.1. ������:",
        "2.3. ������������� ������",
        "3. ������:"
    };
    internal static readonly string[] SkippedActions = {
        "��������",
        "����� \"��������\"",
        "�����:",
        "�������� ����� ����������",
        "�������/�������",
        "�������/������� (����)",
        "�������/������� (����)"
    };
    internal static readonly Dictionary<string, Common.Contracts.Entities.Enums.Exchanges> ExchangeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "�������(�������� �����)", Common.Contracts.Entities.Enums.Exchanges.Moex },
        { "����", Common.Contracts.Entities.Enums.Exchanges.Moex },
        { "���", Common.Contracts.Entities.Enums.Exchanges.Spbex },
        { "�������(FORTS)", Common.Contracts.Entities.Enums.Exchanges.Moex },
        { "�������.", Common.Contracts.Entities.Enums.Exchanges.Moex }
    };
    internal static readonly Dictionary<string, (string Income, string Expense)> ExchangeCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        {  "USDRUB_TOD", ("USD", "RUB") },
        {  "USDRUB_TOM", ("USD","RUB") },
        {  "EURRUB_TOM", ("EUR", "RUB") },
        {  "EURRUB_TOD", ("EUR", "RUB") },
        {  "EURUSD_TOD", ("EUR", "USD")},
        {  "EURUSD_TOM", ("EUR", "USD") }
    };
}