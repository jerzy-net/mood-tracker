# Copilot Instructions — MoodTracker

## General Principles

- Follow simplified Clean Architecture.
- Keep the API layer thin.
- Business logic must reside in the Application layer.
- Infrastructure must not contain business rules.
- Use async/await everywhere.
- All timestamps must be stored and processed in UTC.
- Prefer clarity over cleverness.

---

# Backend Rules

## Architecture

Project structure:

- Domain
- Application
- Infrastructure
- API

### Domain
- Contains only entities and core invariants.
- No EF Core, no external dependencies.

### Application
- Contains services, DTOs, validation.
- All business logic must be implemented here.
- Must be fully unit-testable.

### Infrastructure
- EF Core DbContext
- JWT generation
- Password hashing
- No business logic.

### API
- Controllers must remain thin.
- No business logic in controllers.
- Only mapping and HTTP concerns.

---

## Coding Standards

- Use constructor injection.
- Avoid static state.
- Use explicit DTOs (never expose EF entities).
- Validate input using FluentValidation.
- Return proper HTTP status codes.
- Use meaningful method and variable names.

---

## Authentication

- JWT access tokens only (no refresh tokens).
- Use BCrypt for password hashing.
- Email must be unique.

---

## Mood Logic

- Mood value must be in range [-5, +5].
- Only one mood entry per (UserId, RecordedAtUtc).
- Daily average is calculated by `RecordedAtUtc.Date` in UTC.

---

## Testing

- Write unit tests for all Application services.
- Use xUnit + Moq + FluentAssertions.
- Do not test controllers or infrastructure.

---

# Frontend Rules (React + TypeScript)

## General

- Use functional components only.
- Use TypeScript strictly (no `any`).
- Use explicit interfaces for DTOs.
- Do not duplicate backend models; use dedicated frontend types.
- Keep components small and composable.

---

## Project Structure

Use feature-based structure:

- src/
  - api/
  - components/
  - pages/
  - layouts/
  - hooks/
  - types/
  - utils/

---

## Routing

- Use React Router.
- Protect authenticated routes.
- Redirect unauthenticated users to login.

---

## State Management

- Use React hooks (useState, useEffect, useMemo).
- Avoid global state libraries.
- Keep state local where possible.

---

## API Integration

- Centralize API calls in `api/` folder.
- Use a single axios instance (or fetch wrapper).
- Attach JWT token via interceptor.
- Handle 401 globally.

---

## Forms

- Use controlled components.
- Validate input before sending to backend.
- Do not trust client-side validation alone.

---

## Charts

- Use Recharts (or Chart.js).
- Support two modes:
  - Raw entries
  - Daily average aggregation

---

## UI Principles

- Separate layout from pages.
- No business logic inside UI components.
- Keep presentation and data-fetching concerns separated.

---

## Constraints

- Do not introduce Redux or complex state libraries.
- Do not over-engineer abstractions.
- Keep components readable and maintainable.