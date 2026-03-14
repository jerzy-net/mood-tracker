# Backend Architecture & Skeleton

This document outlines the proposed backend solution structure for MoodTracker. It follows Clean Architecture and prepares infrastructure for EF Core with PostgreSQL, Serilog, Swagger, HealthChecks, CORS, and a global error middleware. Authentication is intentionally deferred.

## Solution Layout
- `MoodTracker.sln` — solution root referencing all projects
- `src/Directory.Build.props` — shared settings (nullable, implicit usings, treat warnings as errors)
- `src/Directory.Packages.props` — centralized NuGet versions (ASP.NET Core, EF Core, MediatR, FluentValidation, Serilog)
- `src/MoodTracker.Domain/`
- `src/MoodTracker.Application/`
- `src/MoodTracker.Infrastructure/`
- `src/MoodTracker.Contracts/`
- `src/MoodTracker.API/`
- `docker-compose.yml` — API + PostgreSQL
- `backend/Dockerfile` — ASP.NET Core runtime image for the API

## Projects and Files

### MoodTracker.Domain
- `MoodTracker.Domain.csproj` — class library definition
- `Entities/MoodEntry.cs` — aggregate root for a mood submission
- `Entities/User.cs` — user identity placeholder for future auth (no auth implemented yet)
- `ValueObjects/MoodScore.cs` — enforces allowed range [-5, +5]
- `ValueObjects/RecordedAt.cs` — UTC-only timestamp value object
- `Abstractions/AuditableEntity.cs` — created/updated timestamps
- `Events/MoodEntryCreated.cs` — domain event raised on creation

### MoodTracker.Application
- `MoodTracker.Application.csproj` — class library definition
- `Abstractions/IMoodEntryRepository.cs` — persistence port for mood entries
- `Abstractions/IUnitOfWork.cs` — transactional boundary
- `Abstractions/IDateTimeProvider.cs` — UTC clock
- `Behaviors/ValidationBehavior.cs` — FluentValidation pipeline for MediatR
- `Features/Moods/CreateMoodEntry/CreateMoodEntryCommand.cs` — MediatR request model
- `Features/Moods/CreateMoodEntry/CreateMoodEntryHandler.cs` — orchestrates creation logic and average calculation
- `Features/Moods/CreateMoodEntry/CreateMoodEntryValidator.cs` — validation rules
- `Features/Moods/Queries/GetMoodHistory/GetMoodHistoryQuery.cs` — personal history request model
- `Features/Moods/Queries/GetMoodHistory/GetMoodHistoryHandler.cs` — raw/daily average query handler
- `Features/Health/HealthCheckQuery.cs` — simple query for `/health`
- `Exceptions/DuplicateMoodEntryException.cs` — conflict signaling
- `DependencyInjection.cs` — registers MediatR, FluentValidation, pipeline behaviors

### MoodTracker.Contracts
- `MoodTracker.Contracts.csproj` — class library definition
- `Moods/Requests/CreateMoodEntryRequest.cs` — API request DTO for mood creation
- `Moods/Responses/CreateMoodEntryResponse.cs` — API response with comparison message
- `Moods/Responses/MoodHistoryResponse.cs` — paged history payload
- `Common/PagedRequest.cs` — pagination envelope
- `Common/PagedResponse.cs` — pagination metadata
- `Health/HealthCheckResponse.cs` — health endpoint DTO

### MoodTracker.Infrastructure
- `MoodTracker.Infrastructure.csproj` — class library definition
- `DependencyInjection.cs` — registers DbContext, repositories, Serilog, HealthChecks, CORS
- `Persistence/MoodTrackerDbContext.cs` — EF Core context
- `Persistence/Configurations/MoodEntryConfiguration.cs` — EF mappings including unique index on `(UserId, RecordedAtUtc)`
- `Persistence/Configurations/UserConfiguration.cs` — EF mappings for users (no auth flows yet)
- `Persistence/Repositories/MoodEntryRepository.cs` — EF implementation of `IMoodEntryRepository`
- `Persistence/UnitOfWork.cs` — EF-backed `IUnitOfWork`
- `Services/DateTimeProvider.cs` — UTC clock implementation
- `Logging/SerilogConfiguration.cs` — Serilog sinks, enrichers, request logging
- `Migrations/` — EF Core migration files
- `appsettings.Infrastructure.json` — optional defaults for connection strings and Serilog

### MoodTracker.API
- `MoodTracker.API.csproj` — ASP.NET Core web project
- `Program.cs` — composition root: configuration for Swagger, Serilog, EF Core, HealthChecks, CORS, API versioning (`/api/v1`), and middleware pipeline
- `Controllers/MoodsController.cs` — endpoints for create + history using Contracts DTOs
- `Controllers/HealthController.cs` — `/health` endpoint wired to HealthChecks
- `Middleware/ExceptionHandlingMiddleware.cs` — global error handling + problem details
- `Filters/ValidationFilter.cs` — translates validation failures to consistent responses
- `Extensions/ServiceCollectionExtensions.cs` — API-layer setup helpers (Swagger, CORS policy)
- `appsettings.json` — base config including PostgreSQL connection string placeholder and Serilog sinks
- `appsettings.Development.json` — development overrides
- `Properties/launchSettings.json` — local profiles

## Infrastructure Setup Notes
- **EF Core + PostgreSQL:** `MoodTrackerDbContext` configured with Npgsql provider; migrations live in Infrastructure. Connection string pulled from configuration and exposed via `appsettings*.json` and `docker-compose.yml`.
- **Serilog:** Configured in Infrastructure for console output and request logging; bootstrap logging enabled in `Program.cs`.
- **Swagger/OpenAPI:** Enabled in API via Swashbuckle; grouped under `v1` with docs at `/swagger`.
- **HealthChecks:** Registered with Npgsql check for database connectivity; exposed at `/health`.
- **CORS:** Single policy allowing configured frontend origin(s); registered in Infrastructure and applied in API.
- **Global Error Middleware:** `ExceptionHandlingMiddleware` wraps the pipeline to produce ProblemDetails responses and map known exceptions (e.g., duplicates) to proper status codes.
- **API Versioning:** Route template `/api/v1/...` applied to controllers; Swagger doc matches version.
- **Docker:** `backend/Dockerfile` builds the API; `docker-compose.yml` runs API + PostgreSQL (with exposed port, volume, and healthcheck).

## Open Questions
- Preferred database name, user, and password for the PostgreSQL container
- Allowed CORS origins for the frontend
- Desired Serilog sinks beyond console (e.g., Seq, file)
