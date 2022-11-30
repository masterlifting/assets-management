FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /
COPY . /

RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

WORKDIR "/services/.net/Portfolio/Worker"

RUN dotnet restore  "AM.Services.Portfolio.Worker.csproj"
RUN dotnet build --no-restore -c Release

WORKDIR /app
FROM build AS publish
RUN dotnet publish --no-restore --no-build -c Release -o /app/publish

#ENV TZ=Europe/Moscow
WORKDIR /app
FROM base AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AM.Services.Portfolio.Worker.dll"]