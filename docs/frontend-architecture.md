# MoodTracker Frontend Architecture (v1)

## Tech Stack

- React 18+
- TypeScript (strict mode enabled)
- Vite
- React Router v6+
- Axios
- Recharts
- Context API
- Nginx (Docker production serving)

---

# Architectural Principles

- Feature-oriented structure
- Separation of concerns
- No business logic inside UI components
- DTO ≠ ViewModel
- Centralized API handling
- Strict typing (no `any`)
- Server-side pagination
- Predictable state flow

---

# Project Structure

```
src/
  app/
    App.tsx
    router.tsx
    providers/
      AuthProvider.tsx
      ToastProvider.tsx
  layouts/
    PublicLayout.tsx
    AppLayout.tsx
  features/
    auth/
      api/
        authApi.ts
      hooks/
        useLogin.ts
        useRegister.ts
      pages/
        LoginPage.tsx
        RegisterPage.tsx
      types.ts
    mood/
      api/
        moodApi.ts
      hooks/
        useCreateMood.ts
        useMoodHistory.ts
      components/
        MoodSlider.tsx
        ResultCard.tsx
        MoodChart.tsx
        MoodTable.tsx
      pages/
        DashboardPage.tsx
        HistoryPage.tsx
      types.ts
      mappers.ts
  components/
    ProtectedRoute.tsx
    ErrorBoundary.tsx
    Pagination.tsx
  api/
    axiosInstance.ts
  config/
    env.ts
  types/
    api.ts
  utils/
    date.ts
```
---

# Routing

## Public Routes

- `/`
- `/login`
- `/register`

## Protected Routes

- `/history`

Access control:
- `/history` wrapped in `ProtectedRoute`
- Unauthorized access → redirect to `/login`

After successful login/registration:
- Redirect to `/`

---

# Layouts

## PublicLayout

Used for:
- `/`
- `/login`
- `/register`

## AppLayout

Used for:
- `/history`

Contains:
- Navigation bar (authenticated version)
- Logout button

---

# State Management

## Auth State

Managed via `AuthProvider` (Context API).

Responsibilities:
- Store JWT in `localStorage`
- Expose `user`, `isAuthenticated`
- Provide `login`, `logout`
- React to `401` responses (auto logout)

Important:
- No client-side JWT expiration decoding
- Expiration handled by backend (401 response)

---

## Local State

Used for:

- Mood submission form
- Chart mode
- Pagination state
- Loading indicators

No global state management library required.

---

# API Layer

## Axios Instance

Centralized configuration:

- Base URL from `VITE_API_BASE_URL`
- Attach `Authorization: Bearer <token>`
- Global `401` interceptor → logout + redirect
- Map errors to typed `ApiError`

---

## API Modules

Each feature owns its API module.

Components never call axios directly.

---

# DTO & Type Rules

## API DTOs

Defined in feature `types.ts`.

## View Models

Mapped via `mappers.ts`.

Reason:
- Prevent API schema leakage into UI
- Enable backend evolution without UI breakage

---

# History Page Architecture

## Data Loading

Server-side pagination required.

API contract:

GET /api/v1/mood?page=1&pageSize=20

Response:

{
  items: MoodEntryDto[],
  totalCount: number
}

Pagination component driven by `totalCount`.

---

## Chart Modes

export type ChartMode = 'raw' | 'daily-average';

Behavior:

- `raw` → render all entries
- `daily-average` → group client-side by UTC date

Grouping logic isolated in `utils/date.ts`.

---

# Error Handling Flow

API → Feature Hook → ToastProvider

Handled statuses:

- 400 → validation message
- 401 → auto logout
- 409 → duplicate entry message
- 500 → generic fallback

No 429 handling (rate limiting not implemented in backend).

---

# Error Boundary

Global `ErrorBoundary` wraps application root.

Purpose:
- Catch unexpected runtime errors
- Display fallback UI
- Prevent full app crash

---

# Build & Deployment

Production build:

npm run build

Output:

dist/

Used in Docker multi-stage build:

Stage 1:
- Node build

Stage 2:
- Nginx serves static assets

No SSR required.

---

# Environment Configuration

Defined in:

config/env.ts

Responsibilities:
- Read `import.meta.env`
- Validate required variables
- Export typed config

Required:

- `VITE_API_BASE_URL`

---

# Performance Considerations

- Memoized heavy chart components
- Avoid unnecessary re-renders
- Pagination limits data load size
- No unnecessary global state

---

# Security Considerations

- JWT stored in localStorage (acceptable for educational scope)
- No refresh tokens
- No silent renew
- All protected API calls require Authorization header

---

# Testing Strategy (Optional v1)

- Component tests: React Testing Library
- Hook tests for:
  - useLogin
  - useCreateMood
- Utility tests for daily-average grouping logic
