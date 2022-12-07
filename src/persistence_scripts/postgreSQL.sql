SELECT 
	"rd"."Id"
	, "rd"."UpdateTime"
	, "rd"."Name"
	, "step"."Name" as "step"
	, "state"."Name" as "state"
	, "rd"."Info"
	, "rd"."Attempt"
	, "rd"."Json" 
FROM "ReportData" as "rd"
JOIN "Steps" as "step" on "rd"."StepId" = "step"."Id"
JOIN "States" as "state" on "rd"."StateId" = "state"."Id"
WHERE "StateId" = 1

SELECT 
	"rd"."Info"
FROM "ReportData" as "rd"
JOIN "Steps" as "step" on "rd"."StepId" = "step"."Id"
JOIN "States" as "state" on "rd"."StateId" = "state"."Id"
WHERE "rd"."Info" IS NOT NULL AND "StateId" != 4

DELETE FROM "ReportData"

SELECT * FROM "States"
 
UPDATE "ReportData" SET "StateId" = 2, "Attempt" = 0, "UpdateTime" = NOW(), "Info" = NULL
WHERE "Id" IN ( SELECT "Id" FROM "ReportData" WHERE "StateId" = 1 FOR UPDATE SKIP LOCKED LIMIT 1)
RETURNING "Id";