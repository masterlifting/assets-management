FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY . .

WORKDIR /src/AM.Portfolio.Worker

RUN dotnet restore
RUN dotnet build --no-restore -c Release
RUN dotnet publish --no-restore --no-build -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AM.Portfolio.Worker.dll"]