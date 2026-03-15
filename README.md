# MoodTracker

MoodTracker is a fullstack web application for tracking emotional state on a scale from -5 to +5.

Users can:
- Register and authenticate
- Submit mood entries (authenticated or anonymously)
- Compare their mood against the daily global average (UTC)
- View their personal mood history and chart (authenticated only)

The project demonstrates clean architecture, testability, proper layering, containerization, and AI-assisted development workflow.

---

## Repository Layout
- `backend/` — .NET solution and Docker assets
- `frontend/` — placeholder for the future React app

---

## Tech Stack

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- JWT (HS256)
- BCrypt password hashing
- FluentValidation
- xUnit + Moq + FluentAssertions

### Frontend
- React + TypeScript (Vite)
- React Router
- Recharts (or Chart.js)

### Infrastructure
- Docker
- docker-compose
- Swagger / OpenAPI
- Health checks

---

## Architecture Overview

Simplified Clean Architecture:

Domain
Application
Infrastructure
API

- Domain: Entities and core business rules
- Application: Services, DTOs, validation
- Infrastructure: EF Core, JWT, hashing
- API: Controllers and middleware

Business logic resides in the Application layer.
API layer contains no business logic.

---

## Core Features

### Authentication
- User registration (email + password)
- BCrypt password hashing
- JWT access token authentication

### Mood Tracking
- Mood value from -5 to +5
- Optional custom timestamp (UTC)
- Daily average calculated by RecordedAtUtc
- Comparison message returned after submission

### History & Charts
- Authenticated users only
- Raw entries view
- Daily average aggregation mode
- Pagination support

---

## Running the Project

### Requirements
- Docker
- Docker Compose

### Start

docker-compose up --build

Services:
- Backend: http://localhost:5000
- Frontend: http://localhost:3000
- Swagger: http://localhost:5000/swagger

---

## Testing

Run backend tests:

dotnet test backend/MoodTracker.slnx

Unit tests cover:
- User registration
- Login
- Mood entry creation
- Daily average calculation
- Duplicate entry handling

---

## AI-Assisted Development

This project is built with extensive use of GitHub Copilot and AI tooling.  
A detailed prompt log and workflow documentation are available in: `docs/ai-workflow.md`

---

## Future Improvements

- Refresh tokens
- Role-based authorization
- Rate limiting
- CI/CD pipeline
- Production logging pipeline
