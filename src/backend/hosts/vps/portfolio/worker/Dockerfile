FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk

WORKDIR /
COPY . /

RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

WORKDIR /services/portfolio/worker

RUN dotnet restore "./AM.Services.Portfolio.Worker.csproj"
RUN dotnet build --no-restore -c Release "./AM.Services.Portfolio.Worker.csproj"

FROM sdk AS publish
RUN dotnet publish --no-restore --no-build -c Release -o /app/publish

WORKDIR /app
FROM runtime AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AM.Services.Portfolio.Worker.dll"]