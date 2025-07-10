FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS aspnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk

WORKDIR /
COPY . /

RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

WORKDIR /services/portfolio/api

RUN dotnet restore "AM.Services.Portfolio.API.csproj"
RUN dotnet build --no-restore -c Release "AM.Services.Portfolio.API.csproj"

FROM sdk AS publish
RUN dotnet publish --no-restore --no-build -c Release -o /app/publish

WORKDIR /app
FROM aspnet AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AM.Services.Portfolio.API.dll"]