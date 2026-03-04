# MoodTracker UI Requirements (v1)

## Scope

Defines UI behavior and constraints for v1.

Out of scope:
- Dark theme
- Public global statistics page
- Timezone switching
- Real-time updates
- Email confirmation / password reset
- Rate limiting UX (not implemented in backend)

---

# Routing

## Public Routes
- `/`
- `/login`
- `/register`

## Authenticated Routes
- `/`
- `/history`

Behavior:
- `/` is accessible to both anonymous and authenticated users.
- `/history` requires authentication.
- Unauthenticated access to `/history` redirects to `/login`.

After successful login or registration:
- Redirect to `/`.

---

# Navigation

## Unauthenticated
- Logo (links to `/`)
- Login
- Register

## Authenticated
- Logo (links to `/`)
- History
- Logout

Logout behavior:
- Remove JWT from storage
- Redirect to `/`

---

# Authentication UX

## Token Storage

- JWT stored in `localStorage`
- Attached to API requests via centralized API client
- On `401` response:
  - Clear token
  - Redirect to `/login`

---

# Home / Dashboard

Accessible to all users.

## Mood Input

- Slider: integer range -5 to +5
- Large color-coded value display
- Optional custom datetime (default: current UTC)
- Submit button disabled while request is in progress

Color rules:
- `< 0` → red
- `0` → gray
- `> 0` → green

Datetime behavior:
- Backend stores UTC
- Frontend sends ISO string in UTC
- Displayed values use local browser time (`toLocaleString()`)

---

## Result Card (After Submission)

Displays:

- Submitted mood value
- Global daily average (UTC-based)
- Difference:  
  `difference = userValue - globalDailyAverage`

Message rules:

- `difference > 0.5` → "Your mood is better than today's average."
- `-0.5 ≤ difference ≤ 0.5` → "Your mood is close to today's average."
- `difference < -0.5` → "Your mood is below today's average."
- If no entries exist for today →  
  "Not enough data to calculate today's average."

Notes:
- Daily average is calculated globally across all users.
- Calculation is based on `RecordedAtUtc.Date == Today (UTC)`.

---

# History Page (Authenticated Only)

Sections:
- Chart controls
- Chart
- Entries table

If user has no entries:
- Show message: "No mood entries yet."

---

## Chart

- Y-axis fixed: -5 to +5
- X-axis: date/time (local display)

Modes:

1. Raw mode
   - All individual entries plotted

2. Daily Average mode
   - Entries grouped by date (UTC date)
   - Average calculated per user (not global)

Clarification:
- History page shows **user-specific data only**
- Daily average mode = average of that user's entries per day

---

## Entries Table

Columns:

- Date (displayed in local time)
- Mood value
- User daily average (for that date)
- Difference (Value - UserDailyAverage)

Pagination:
- 20 entries per page
- Simple numbered pagination (no infinite scroll)

---

# Authentication Pages

## Login

Fields:
- Email
- Password

Validation:
- Required fields
- Basic email format validation

On success:
- Store JWT
- Redirect to `/`

---

## Register

Fields:
- Email
- Password (minimum 8 characters)
- Confirm password

Validation:
- Required fields
- Password match check
- Minimum length enforcement

On success:
- Auto-login
- Redirect to `/`

---

# Error Handling

Centralized toast notification system.

Status handling:

- `400` → Validation error (show message)
- `401` → Redirect to login
- `409` → "Mood entry already exists for this timestamp."
- `500` → "Something went wrong. Please try again."

Loading states:
- Disable buttons during API calls
- Show spinner where appropriate

---

# UI Principles

- Use functional React components only
- No business logic inside UI components
- Separate layout from pages
- Centralized API client
- Strict TypeScript (no `any`)
- Keep components small and composable