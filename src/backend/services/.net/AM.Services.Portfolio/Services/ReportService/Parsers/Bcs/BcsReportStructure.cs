using System;
using System.Collections.Generic;

namespace AM.Services.Portfolio.Services.ReportService.Parsers.Bcs;

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
    internal static readonly Dictionary<string, Common.Contracts.Enums.Exchanges> ExchangeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        { "�������(�������� �����)", Common.Contracts.Enums.Exchanges.MOEX },
        { "����", Common.Contracts.Enums.Exchanges.MOEX },
        { "���", Common.Contracts.Enums.Exchanges.SPBEX },
        { "�������(FORTS)", Common.Contracts.Enums.Exchanges.MOEX },
        { "�������.", Common.Contracts.Enums.Exchanges.MOEX }
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