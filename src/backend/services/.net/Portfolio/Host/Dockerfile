FROM mcr.microsoft.com/dotnet/aspnet:6.0 as aspnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk

WORKDIR /
COPY . /

RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
COPY *.sln .

RUN dotnet restore "Assets Management.sln"
RUN dotnet build --no-restore -c Release "Assets Management.sln"

WORKDIR /services/.net/Portfolio/Host
FROM sdk AS publish
RUN dotnet publish --no-restore --no-build -c Release -o /app/publish

WORKDIR /app
FROM aspnet AS final
COPY --from=publish /app/publish .

#ENV TZ=Europe/Moscow
ENTRYPOINT ["dotnet", "AM.Services.Portfolio.Host.dll"]