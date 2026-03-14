# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/MoodTracker.API/MoodTracker.API.csproj", "src/MoodTracker.API/"]
COPY ["src/MoodTracker.Application/MoodTracker.Application.csproj", "src/MoodTracker.Application/"]
COPY ["src/MoodTracker.Contracts/MoodTracker.Contracts.csproj", "src/MoodTracker.Contracts/"]
COPY ["src/MoodTracker.Infrastructure/MoodTracker.Infrastructure.csproj", "src/MoodTracker.Infrastructure/"]
COPY ["src/MoodTracker.Domain/MoodTracker.Domain.csproj", "src/MoodTracker.Domain/"]
COPY ["src/Directory.Build.props", "src/"]
COPY ["src/Directory.Packages.props", "src/"]

RUN dotnet restore "src/MoodTracker.API/MoodTracker.API.csproj"

COPY . .
WORKDIR "/src/src/MoodTracker.API"
RUN dotnet publish "MoodTracker.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MoodTracker.API.dll"]
