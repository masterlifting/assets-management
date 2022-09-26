FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# COPY . ./
# RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
# COPY *.sln ./
# RUN dotnet restore
# RUN dotnet build --no-restore -c Release

# FROM nexus-ci.corp.dev.vtb/adsd-docker/ubi8/dotnet-60-runtime:6.0-3 AS base
# WORKDIR /app
# EXPOSE 8080

# FROM nexus-ci.corp.dev.vtb/adsd-docker/ubi8/dotnet-60:6.0-3 AS build
# WORKDIR /src
# USER 0
# ARG nexususer
# ARG nexuspass

# COPY --chown=1001:0 .docker/data/vtb-ca.pem /etc/ssl/certs/
# COPY ["src/Depint.Adapter.Cftb.Core/", "Depint.Adapter.Cftb.Core/"]
# COPY ["src/Depint.Adapter.Cftb.Host/", "Depint.Adapter.Cftb.Host/"]
# COPY ["src/Depint.Adapter.Cftb.Infrastructure/", "Depint.Adapter.Cftb.Infrastructure/"]

# WORKDIR "/src/Depint.Adapter.Cftb.Host"

# RUN dotnet nuget disable source nuget.org
# RUN dotnet nuget add source https://nexus-ci.corp.dev.vtb/repository/pfom-nuget-lib/index.json -n pfom-nuget-lib -u $nexususer -p $nexuspass --store-password-in-clear-text
# RUN dotnet nuget add source https://nexus-ci.corp.dev.vtb/repository/adsd-nugetv3/index.json -n asd-nuget-proxyv3 -u $nexususer -p $nexuspass --store-password-in-clear-text
# RUN dotnet nuget add source https://nexus-ci.corp.dev.vtb/repository/adsd-nuget-lib/index.json -n adsd-nuget-lib -u $nexususer -p $nexuspass --store-password-in-clear-text

# RUN dotnet restore  "Depint.Adapter.Cftb.Host.csproj"
# RUN dotnet build --no-restore -c Release

# FROM build AS publish
# RUN dotnet publish --no-restore --no-build -c Release -o /app/publish
# FROM base AS final
# ENV TZ=Europe/Moscow
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Depint.Adapter.Cftb.Host.dll"]