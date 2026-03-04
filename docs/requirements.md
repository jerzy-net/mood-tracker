# MoodTracker — Requirements Specification

## 1. Overview

MoodTracker is a fullstack web application that allows users to record their emotional state on a scale from -5 to +5.

The system supports both authenticated and anonymous mood submissions.

All timestamps are stored in UTC.

---

## 2. Functional Requirements

### 2.1 Authentication

- User registration with:
  - Email (unique)
  - Password
- Passwords must be hashed using BCrypt
- Login returns JWT access token
- No refresh tokens
- No email confirmation

---

### 2.2 Mood Submission

- Mood value must be in range [-5, +5]
- User may optionally specify RecordedAtUtc
- If not specified, current UTC timestamp is used
- After successful creation:
  - System calculates global daily average based on RecordedAtUtc.Date == Today (UTC)
  - Returns comparison message:
    - Above average
    - Below average
    - Equal to average

---

### 2.3 Data Constraints

- For authenticated users:
  - Only one entry per (UserId, RecordedAtUtc)
  - Enforced via unique index
  - Conflict results in HTTP 409

- For anonymous users:
  - No uniqueness constraints

---

### 2.4 History & Chart (Authenticated Only)

- Retrieve personal mood entries
- Support pagination (page, pageSize)
- Two aggregation modes:
  - Raw (all entries)
  - Daily average (grouped by date)

---

## 3. Non-Functional Requirements

- API versioning: /api/v1
- OpenAPI/Swagger documentation
- Centralized error handling middleware
- Input validation via FluentValidation
- Health endpoint /health
- EF Core migrations
- Dockerized deployment via docker-compose

---

## 4. Architecture

Simplified Clean Architecture:

- Domain
- Application
- Infrastructure
- API

Constraints:
- Business logic resides in Application layer
- DTOs must not expose EF entities
- API layer must remain thin
- Infrastructure must not contain business logic

---

## 5. Persistence

- PostgreSQL
- Entity Framework Core
- All timestamps stored as UTC
- Unique index for (UserId, RecordedAtUtc) when UserId is not null

---

## 6. Testing Requirements

Unit tests required for:

- RegisterUser
- LoginUser
- CreateMoodEntry
- Daily average calculation
- Duplicate entry detection

Frameworks:
- xUnit
- Moq
- FluentAssertions

Infrastructure and controller tests are out of scope.
