# Build context is the solution folder, not this one:
#   docker build -f YouScanDashboard.Server/Dockerfile -t youscan-dashboard .
# docker compose (see compose.yaml) does that for you.

# The SPA is built by Node alone. No MSBuild, no Visual Studio SDK, no Node in the .NET image.
FROM node:22-alpine AS client
WORKDIR /client
# Dependencies are a layer of their own: editing the sources does not re-download them.
COPY youscandashboard.client/package.json youscandashboard.client/package-lock.json ./
RUN npm ci
COPY youscandashboard.client/ ./
RUN npm run build

# The API. BuildSpa=false drops the client project reference: the SPA is already built above.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS server
WORKDIR /src
COPY YouScanDashboard.Server/YouScanDashboard.Server.csproj YouScanDashboard.Server/
RUN dotnet restore YouScanDashboard.Server/YouScanDashboard.Server.csproj -p:BuildSpa=false
COPY YouScanDashboard.Server/ YouScanDashboard.Server/
RUN dotnet publish YouScanDashboard.Server/YouScanDashboard.Server.csproj \
    -c Release -o /app/publish -p:BuildSpa=false -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=server /app/publish ./
COPY --from=client /client/dist ./wwwroot
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "YouScanDashboard.Server.dll"]