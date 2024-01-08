--EVENTS
SELECT
    "e"."Description",
    "e"."DateTime" as "Date",
    "et"."Name" as "Event",
    "e"."Value",
    "a"."Name" as "Asset"
FROM "Events" as "e"
JOIN "EventTypes" as "et" ON "e"."TypeId" = "et"."Id"
JOIN "Derivatives" as "d" ON "e"."DerivativeId" = "d"."Id"
JOIN "Assets" as "a" ON "d"."AssetId" = "a"."Id"
JOIN "Accounts" as "ac" ON "e"."AccountId" = "ac"."Id"
JOIN "Holders" as "h" ON "e"."HolderId" = "h"."Id"
JOIN "Exchanges" as "ex" ON "e"."ExchangeId" = "ex"."Id"
ORDER BY "e"."DateTime" DESC
LIMIT 100;

--DEALS
SELECT
    "d"."Description", 
    "d"."DateTime" as "Date",
    "ai"."Name" as "Income Asset",
    "i"."Value" as "Income Value",
    "ae"."Name" as "Expense Asset",
    "e"."Value" as "Expense Value" 
FROM "Deals" as "d"
JOIN "Incomes" as "i" ON "d"."Id" = "i"."DealId"
JOIN "Expenses" as "e" ON "d"."Id" = "e"."DealId"
JOIN "Holders" as "hi" ON "i"."HolderId" = "hi"."Id"
JOIN "Holders" as "he" ON "e"."HolderId" = "he"."Id"
JOIN "Exchanges" as "exi" ON "i"."ExchangeId" = "exi"."Id"
JOIN "Exchanges" as "exe" ON "e"."ExchangeId" = "exe"."Id"
JOIN "Accounts" as "aci" ON "i"."AccountId" = "aci"."Id"
JOIN "Accounts" as "ace" ON "e"."AccountId" = "ace"."Id"
JOIN "Derivatives" as "dei" ON "i"."DerivativeId" = "dei"."Id"
JOIN "Derivatives" as "dee" ON "e"."DerivativeId" = "dee"."Id"
JOIN "Assets" as "ai" ON "dei"."AssetId" = "ai"."Id"
JOIN "Assets" as "ae" ON "dee"."AssetId" = "ae"."Id"
ORDER BY "d"."DateTime" DESC
LIMIT 10;

SELECT COUNT(*) FROM "Deals";
SELECT COUNT(*) FROM "Events";

SELECT
    "a"."Id" as "AssetId",
    "a"."Name" as "Asset",
    "d"."Ticker",
    "d"."Code"
FROM "Assets" as "a"
JOIN "Derivatives" as "d" ON "a"."Id" = "d"."AssetId"
ORDER BY "d"."Ticker";

SELECT * FROM "ProcessStatuses";

DELETE FROM "Events";
DELETE FROM "Deals";
-- DELETE FROM "Derivatives";
-- DELETE FROM "Assets";