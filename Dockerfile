FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["TaskTrack.API/TaskTrack.API.csproj", "TaskTrack.API/"]
COPY ["TaskTrack.Repo/TaskTrack.Repo.csproj", "TaskTrack.Repo/"]
COPY ["TaskTrack.Service/TaskTrack.Service.csproj", "TaskTrack.Service/"]
RUN dotnet restore "TaskTrack.API/TaskTrack.API.csproj"

COPY . .
RUN dotnet publish "TaskTrack.API/TaskTrack.API.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
USER $APP_UID
COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "exec dotnet TaskTrack.API.dll --urls http://0.0.0.0:${PORT:-8080}"]