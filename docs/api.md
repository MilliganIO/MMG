# API Reference

> Auto-maintained by the Developer agent. Do not edit manually.
> Updated after every feature that adds or changes an endpoint.
> Format defined in `.claude/skills/update-docs.md`.

---

## GET /api/messages

Search messages using the `GetMessageDetailsByParameters_TOTAL` stored procedure.

**Auth:** JWT Bearer required; `role` claim required.

**Query Parameters (all optional except `onboardingStatus`):**

| Param | Type | Description |
|---|---|---|
| `onboardingStatus` | `string` | Required. Pass `App.OnboardingStatus` ("P"). |
| `startDate` | `DateTime?` | Filter start date. |
| `endDate` | `DateTime?` | Filter end date. |
| `programId` | `int?` | CDC Program ID. |
| `messageType` | `string?` | Message type code (e.g., "HL7"). |
| `status` | `string?` | Message status string. |
| `categoryId` | `int?` | Category ID. |
| `jurisdictionId` | `int?` | Jurisdiction ID. |
| `eventId` | `int?` | Event ID. |
| `controlId` | `string?` | Message control ID. |
| `transactionId` | `int?` | Transaction ID. |
| `caseId` | `string?` | Case ID. |
| `uploadFileName` | `string?` | Upload file name. |

**Response `200 OK`:**
```json
{
  "messages": [
    {
      "msgTransactionId": 12345,
      "messageControlId": "CTL-001",
      "caseId": "CASE-999",
      "jurisdictionDescription": "Georgia",
      "errorsCount": 0,
      "warningsCount": 2,
      "categoryDescription": "Lab Report",
      "programName": "MVPS",
      "receivedDate": "2026-04-30T14:22:00"
    }
  ],
  "total": 1
}
```

**Response `400 Bad Request`:** `user_role_id` claim missing from JWT.  
**Response `401 Unauthorized`:** Unauthenticated.  
**Response `403 Forbidden`:** No role claim.

---

## GET /api/messages/{msgTransactionId}/ErrorsAndWarnings

Returns the validation errors and warnings for a given message transaction, sourced from `[dashboard].[validation_error_extended_vw]` (CDE_ENV database). Added by FEAT-043.

**Auth:** Required (JWT Bearer token)  
**Role:** Any authenticated role

**Path parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `msgTransactionId` | int | Yes | MVPS transaction ID |

**Response `200 OK`:**
```json
[
  {
    "errorId": 101,
    "severity": "Error",
    "errorDescription": "OBX segment missing required field",
    "validationType": "Structural",
    "dataElementId1": "OBX-3",
    "dataElementValue1": null,
    "dataElementLoc1": "Segment",
    "dataElementId2": null,
    "dataElementValue2": null,
    "dataElementLoc2": null
  },
  {
    "errorId": 102,
    "severity": "Warning",
    "errorDescription": "Unexpected value in PID segment",
    "validationType": "Business",
    "dataElementId1": "PID-5",
    "dataElementValue1": "TEST",
    "dataElementLoc1": "Field",
    "dataElementId2": null,
    "dataElementValue2": null,
    "dataElementLoc2": null
  }
]
```

Returns `[]` if no errors/warnings exist for the given transaction.

**Error responses:**

| Status | Condition |
|---|---|
| `400 Bad Request` | `msgTransactionId` is ≤ 0 |
| `401 Unauthorized` | No valid JWT |

**Portal note:** The call is skipped entirely when both `ErrorsCount` and `WarningsCount` on the `MessageDetailDto` are zero.

---

## GET /alive

Liveness check — confirms the API process is alive and responsive. No database connectivity tested.

**Auth:** None

**Response `200 OK`:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.001",
  "entries": {
    "self": { "status": "Healthy", "duration": "00:00:00.000", "description": null, "tags": ["live"] }
  }
}
```

---

## GET /health/dashboard | /health/cde-env | /health/cde-person | /health/cde-report

Per-database readiness checks. Each endpoint probes one configured database.

| Endpoint | Database |
|---|---|
| `/health/dashboard` | CDEDashboard (main app DB) |
| `/health/cde-env` | CDE_ENV |
| `/health/cde-person` | CDE_Person |
| `/health/cde-report` | CDE_REPORT |

**Auth:** None

**Response `200 OK` (database reachable):**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.012",
  "entries": {
    "database-dashboard": { "status": "Healthy", "duration": "00:00:00.011", "description": null, "tags": ["database-dashboard"] }
  }
}
```

**Response `503 Service Unavailable` (database unreachable):**
```json
{
  "status": "Unhealthy",
  "totalDuration": "00:00:05.001",
  "entries": {
    "database-dashboard": { "status": "Unhealthy", "duration": "00:00:05.000", "description": "Cannot connect to database.", "tags": ["database-dashboard"] }
  }
}
```
Response bodies never contain connection strings, credentials, or server paths.

---

## GET /api/Dashboard/Alerts

Returns active alerts for the authenticated user, filtered by audience matching.

**Auth:** Required (JWT Bearer token or `auth_token` cookie). Any authenticated user.

**Response `200 OK`:**
```json
[
  { "alertId": 1, "title": "Maintenance Tonight", "description": "System down 10-midnight.", "type": "Outage" },
  { "alertId": 2, "title": "Tip of the Week", "description": "Use keyboard shortcuts.", "type": "Tip" }
]
```
Empty array `[]` when no alerts match (not 404).

**Filtering rules:**
- `AllUsers = true` → all users
- `ProgramId = -1` → `user_type = Internal`
- `JurisdictionId = -1` → `user_type = External`
- `ProgramId = X` → user with `program_id = X` JWT claim
- `JurisdictionId = X` → user with `jurisdiction_id = X` JWT claim
- Role `MVPS Support Manager` or `MVPS User Support Manager` → bypass all filters, see all active alerts

**Sort:** Outage → Warning → Info → Tip, then `StartDate` descending within each type.

**Response `401`:** Unauthenticated request.

---

## POST /auth/token

Authenticates a user and returns a signed JWT.

**Request headers:**

| Header | Required | Description |
|---|---|---|
| `X-Logon-User` | Yes | The account identifier (e.g., `DOMAIN\username` or external user ID) |

**Response:** `200 OK` with `{ "token": "<jwt>" }` on success, `401 Unauthorized` on failure.

**JWT Claims:**

| Claim | Type | Always Present | Notes |
|---|---|---|---|
| `account_identifier` | string | Yes | |
| `first_name` | string | Yes | |
| `last_name` | string | Yes | |
| `email_address` | string | Yes | |
| `is_active` | bool (string) | Yes | |
| `user_type` | string | Yes | `"Internal"` or `"External"` |
| `user_role_id` | int (string) | Per role | One claim per current role |
| `role` | string | Per role | One claim per current role |
| `jurisdiction_id` | int (string) | External users only | Added by FEAT-002 |
| `jurisdiction_description` | string | External users only | Added by FEAT-002 |
| `program_id` | int (string) | Internal non-admin users only | One claim per active program; added by FEAT-003. Omitted for admin roles (FEAT-004). |
| `program_name` | string | Internal non-admin users only | One claim per active program; added by FEAT-003. Omitted for admin roles (FEAT-004). |

**Admin role bypass (FEAT-004):** Internal users whose current role has `IsAdminRole = true` AND `RoleGroup = null` are exempt from the program assignment requirement. They receive a valid JWT with the standard claims but no `program_id` / `program_name` claims. The `role` claim identifies the admin role for UI authorization checks.

**Failure reasons (401):**

| Reason | Description |
|---|---|
| `IdentifierNullOrEmpty` | `X-Logon-User` header missing or blank |
| `UserNotFound` | No user with that account identifier in the database |
| `UserInactive` | User exists but `IsActive = false` |
| `JurisdictionNotAssigned` | External user has no `UserRoleAssignment` with a non-null `JurisdictionId` |
| `ProgramNotAssigned` | Internal user has no `UserRoleAssignment` rows with a non-null `ProgramId` pointing to an active `DimPrograms` record |

---

## POST /auth/switch-role

Switches the authenticated user's active role and regenerates the JWT. Added by FEAT-020.

**Auth:** Required (Bearer token via `auth_token` cookie)

**Request body:**

```json
{
  "userRoleId": 2
}
```

| Field | Type | Required | Description |
|---|---|---|---|
| `userRoleId` | int | Yes | The `UserRoleId` to switch to (must belong to the authenticated user) |

**Response:** `200 OK` — new `auth_token` cookie set with regenerated JWT reflecting the new active role.

**Error responses:**

| Status | Condition |
|---|---|
| `400 Bad Request` | `userRoleId` is 0 or negative |
| `401 Unauthorized` | No valid JWT / no `account_identifier` claim |
| `404 Not Found` | `userRoleId` does not belong to the authenticated user |

---

## GET /auth/role-assignments

Returns the authenticated user's role assignments with role name, jurisdiction, and program details. Added by FEAT-020.

**Auth:** Required (Bearer token via `auth_token` cookie)

**Response:** `200 OK`

```json
[
  {
    "userRoleId": 1,
    "roleName": "Admin",
    "isCurrentRole": true,
    "jurisdictionDescription": null,
    "programName": null
  },
  {
    "userRoleId": 2,
    "roleName": "Viewer",
    "isCurrentRole": false,
    "jurisdictionDescription": null,
    "programName": "Immunization"
  }
]
```

**Error responses:**

| Status | Condition |
|---|---|
| `401 Unauthorized` | No valid JWT / no `account_identifier` claim |

---

## GET /auth/current-role-access

Returns the authenticated user's current role with its access scope (programs, jurisdiction, or admin). Added by FEAT-022.

**Auth:** Required (Bearer token via `auth_token` cookie)

**Response:** `200 OK`

```json
{
  "userRoleId": 42,
  "roleName": "CDC Program Data Manager",
  "roleDescription": "Manages data for assigned CDC programs",
  "roleGroup": "Program",
  "isAdminRole": false,
  "jurisdictionDescription": null,
  "programNames": ["HIV/AIDS", "Tuberculosis"],
  "hasAllEventCodes": false,
  "eventCodeNames": ["Hepatitis A (10110)", "Measles (10140)"]
}
```

| Field | Type | Description |
|---|---|---|
| `userRoleId` | int | The current role's UserRoleId |
| `roleName` | string | Role name |
| `roleDescription` | string | Role description |
| `roleGroup` | string? | `"Program"`, `"Jurisdiction"`, or `null` (admin) |
| `isAdminRole` | bool | Whether this is an admin role |
| `jurisdictionDescription` | string? | Jurisdiction name (jurisdiction-scoped roles only) |
| `programNames` | string[] | Assigned program names, sorted alphabetically (program-scoped roles only) |
| `hasAllEventCodes` | bool | `true` if admin role or `EventCodeId=-1`; UI shows "All Event Codes" |
| `eventCodeNames` | string[] | Assigned event codes as `"Description (Code)"`, sorted alphabetically |

**Error responses:**

| Status | Condition |
|---|---|
| `401 Unauthorized` | No valid JWT / no `account_identifier` claim |
| `404 Not Found` | No current role found for the authenticated user |

---

## GET /auth/user-settings

Returns the authenticated user's assigned settings. Only includes settings where both `Setting.IsActive` and `UserSetting.IsActive` are `true`. Added by FEAT-024.

**Auth:** Required (Bearer token via `auth_token` cookie)

**Response:** `200 OK`

```json
[
  {
    "settingId": 1,
    "settingName": "Email Notifications",
    "settingDescription": "Receive email notifications for status changes",
    "canUserChange": true
  }
]
```

Returns `[]` if the user has no active settings.

**Error responses:**

| Status | Condition |
|---|---|
| `401 Unauthorized` | No valid JWT / no `user_id` claim |

---

## GET /auth/user-notifications

Returns the authenticated user's eligible notifications with toggle state. Filters by active notifications whose category is either unlinked to a setting or linked to one the user has active. Includes `HasMinimumRecipient` enforcement for jurisdiction users. Added by FEAT-025.

**Auth:** Required (Bearer token via `auth_token` cookie)

**Response:** `200 OK`

```json
[
  {
    "notificationId": 1,
    "title": "Daily Summary Report",
    "description": "Receive a daily summary of message activity",
    "frequency": "Daily",
    "scheduledTime": "08:00",
    "categoryDescription": "Reports",
    "isEnabled": true,
    "isToggleDisabled": false,
    "disabledReason": null
  }
]
```

| Field | Type | Description |
|---|---|---|
| `notificationId` | int | Notification ID |
| `title` | string | Notification title |
| `description` | string? | Optional description |
| `frequency` | string | e.g., "Daily", "Weekly" |
| `scheduledTime` | string? | e.g., "08:00" |
| `categoryDescription` | string | Category name |
| `isEnabled` | bool | Whether the user has this notification active |
| `isToggleDisabled` | bool | `true` if toggle is locked (HasMinimumRecipient) |
| `disabledReason` | string? | Warning message when toggle is disabled |

Returns `[]` if no eligible notifications exist.

**Error responses:**

| Status | Condition |
|---|---|
| `401 Unauthorized` | No valid JWT / no `user_id` claim |

---

## PUT /auth/user-notifications

Toggles a notification on or off for the authenticated user. Upserts a `UserNotification` row. Added by FEAT-025.

**Auth:** Required (Bearer token via `auth_token` cookie)

**Request Body:**

```json
{
  "notificationId": 1,
  "isActive": false
}
```

**Response:** `200 OK` on success.

**Error responses:**

| Status | Condition |
|---|---|
| `400 Bad Request` | `NotificationId` is missing or ≤ 0 |
| `401 Unauthorized` | No valid JWT / no `user_id` or `account_identifier` claim |

---

## GET /api/admin/users/my-permissions

Returns the authenticated user's permission scope for the Edit-User page, based on their **current role only** (`IsCurrentRole = true`). Added by FEAT-030, updated by FEAT-030-v2.

**Auth:** Required (Bearer token)
**Policy:** `AdminDataManager`

**Response:** `200 OK`

```json
{
  "isAdmin": false,
  "currentRoleGroup": "Program",
  "programIds": [10],
  "jurisdictionIds": [],
  "eventCodeIdsByProgramId": { "10": [1, 2] },
  "eventCodeIdsByJurisdictionId": {}
}
```

| Field | Type | Description |
|---|---|---|
| `isAdmin` | bool | `true` if current role is admin (`IsAdminRole=true` AND `RoleGroup=null`) — all scoping disabled |
| `currentRoleGroup` | string? | `"Program"`, `"Jurisdiction"`, or `null` (admin). Determines which roles can be assigned. |
| `programIds` | int[] | Program IDs from current role's assignments |
| `jurisdictionIds` | int[] | Jurisdiction IDs from current role's assignments |
| `eventCodeIdsByProgramId` | dict | Event code IDs grouped by program ID. `-1` means all event codes. |
| `eventCodeIdsByJurisdictionId` | dict | Event code IDs grouped by jurisdiction ID. `-1` means all event codes. |

**Error responses:**

| Status | Condition |
|---|---|
| `401 Unauthorized` | No valid JWT / no `account_identifier` claim |

---

> **Note:** All lookup endpoints require authentication (added by FEAT-023) and return a unified response format. Integer-keyed lookups return `LookupItem` (`{ "value": int, "text": "string" }`). String-keyed lookups (MessageStatuses) return `LookupStringItem` (`{ "value": "string", "text": "string" }`).

## GET /api/filters/Workflows/{id}/Statuses

Returns active workflow statuses for the given workflow, ordered by `SortOrder`. Added by FEAT-013.

**Path parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `id` | int | Yes | The Workflow ID |

**Response:** `200 OK`

```json
[
  { "value": 1, "text": "Received" },
  { "value": 2, "text": "In Review" }
]
```

Returns `[]` if no active statuses exist for the workflow.

---

## GET /api/filters/ClassificationStatuses

Returns active classification statuses, ordered by `DisplaySequence` descending. Added by FEAT-014.

**Response:** `200 OK`

```json
[
  { "value": 1, "text": "Status Name" },
  { "value": 2, "text": "Another Status" }
]
```

Returns `[]` if no active classification statuses exist.

---

## GET /api/filters/ProcessingStatuses

Returns active processing statuses, ordered by `Name` alphabetically. Added by FEAT-015.

**Response:** `200 OK`

```json
[
  { "value": 1, "text": "Status Name" },
  { "value": 2, "text": "Another Status" }
]
```

Returns `[]` if no active processing statuses exist.

---

## GET /api/filters/BatchFileStatuses

Returns active batch file statuses, ordered by `DisplayName` alphabetically. Added by FEAT-016.

**Response:** `200 OK`

```json
[
  { "value": 1, "text": "Status Name" },
  { "value": 2, "text": "Another Status" }
]
```

Returns `[]` if no active batch file statuses exist.

---

## GET /api/filters/CdcProfiles

Returns all CDC profiles, ordered by `DisplayName` alphabetically. Added by FEAT-017.

**Response:** `200 OK`

```json
[
  { "value": 1, "text": "Profile Display Name" },
  { "value": 2, "text": "Another Profile" }
]
```

Returns `[]` if no profiles exist.

---

## GET /api/filters/Mmwrs

Returns MMWR year data (years 2018 through current year) with start and end week ranges. Data sourced from `[netss].[MMWRWeekLookup]` in the `CDE_ENV` database. Added by FEAT-018.

**Response:** `200 OK`

```json
[
  { "year": 2026, "startWeek": 1, "endWeek": 12 },
  { "year": 2025, "startWeek": 1, "endWeek": 52 },
  { "year": 2024, "startWeek": 1, "endWeek": 52 }
]
```

Returns `[]` if no MMWR data exists.

**Notes:**
- Uses a custom DTO `MmwrYearDto(int Year, int StartWeek, int EndWeek)` — not `LookupItem`
- Endpoint injects `IMmwrRepository` (not `IFilterRepository`) since data comes from a separate database
- Results ordered by year descending

---

## GET /api/Persons

Searches for an active person in the `PERSON_VW` view (CDE_Person database) by AccountIdentifier. Returns the person only if they are active (CdcStartDate ≤ today AND CDCSeparationDate > today). Added by FEAT-027.

**Auth:** Required (Bearer token)
**Policy:** `AdminDataManager` (MVPS Support Manager, MVPS User Support Manager, Jurisdiction Data Manager, CDC Program Data Manager)

**Query parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `accountIdentifier` | string | Yes | The 4-character user ID to search |

**Response:** `200 OK`

```json
{
  "accountIdentifier": "ABCD",
  "firstName": "Jane",
  "lastName": "Doe",
  "primaryEmailAddress": "jane.doe@cdc.gov"
}
```

**Error responses:**

| Status | Condition |
|---|---|
| `400 Bad Request` | Missing or blank `accountIdentifier` |
| `401 Unauthorized` | No valid JWT |
| `403 Forbidden` | Role not in AdminDataManager policy |
| `404 Not Found` | No active person found for the given AccountIdentifier |

---

## GET /api/SamsUsers

Searches for a SAMS user by executing stored procedure `uspExtGetUserActivity` (no parameters) against the `CDE_REPORT` database. Results are filtered in-memory by AccountIdentifier (matching `UserAccountNbr` as string) and ActivityName (matching the `SamsActivityName` appsetting). Added by FEAT-028.

**Auth:** Required (Bearer token)
**Policy:** `AdminDataManager` (MVPS Support Manager, MVPS User Support Manager, Jurisdiction Data Manager, CDC Program Data Manager)

**Query parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `accountIdentifier` | string | Yes | The user account number to search (compared to UserAccountNbr as string) |

**Response:** `200 OK`

```json
{
  "accountIdentifier": "12345",
  "email": "jane@example.com",
  "givenName": "Jane",
  "surName": "Doe",
  "activityName": "MVPS Production",
  "currentSamsStatus": "Active"
}
```

**Error responses:**

| Status | Condition |
|---|---|
| `400 Bad Request` | Missing or blank `accountIdentifier` |
| `401 Unauthorized` | No valid JWT |
| `403 Forbidden` | Role not in AdminDataManager policy |
| `404 Not Found` | No matching SAMS user found for the given AccountIdentifier and configured ActivityName |

---

## GET /api/Dashboard/Status

Returns a 24-hour message status snapshot with trend indicators comparing the current and prior 24-hour periods.

**Auth:** Required (Bearer token)

**Query parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `onboardingStatus` | string | Yes | Onboarding status filter (e.g., `"P"`) |
| `messageType` | string | No | Optional message type filter (max 10 chars) |

**Response:** `200 OK` with `MessageStatusSnapshotDto` containing Incoming, Completed, Errored, and InProcess metrics, each with Count, PercentChange, and TrendDirection.

---

## GET /api/Dashboard/Volumes

Returns message volume data points for rendering an area chart over a specified date range.

**Auth:** Required (Bearer token)

**Query parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `startDate` | DateTime | Yes | Start of the date range (ISO 8601) |
| `endDate` | DateTime | Yes | End of the date range (ISO 8601) |
| `onboardingStatus` | string | Yes | Onboarding status filter (e.g., `"P"`) |
| `messageType` | string | No | Optional message type filter (max 10 chars) |

**Response:** `200 OK`

```json
{
  "dataPoints": [
    { "label": "Mon", "count": 42.0 },
    { "label": "Tue", "count": 58.0 }
  ]
}
```

Labels are formatted server-side based on the date range span:
- ≤ 7 days: day-of-week (`"ddd"` — Mon, Tue, etc.)
- ≤ 31 days: date (`"MM/dd"` — 03/15, 04/01, etc.)
- &gt; 31 days: month abbreviation (`"MMM"` — Jan, Feb, etc.)

**Error responses:**

| Status | Condition |
|---|---|
| `401 Unauthorized` | No valid JWT or missing `user_role_id` claim |
---

## GET /api/Dashboard/ErrorsAndWarnings

Returns per-`ReceivedDate` Errors and Warnings counts for the authenticated user's current role, bucketed and labeled by date span. Added by FEAT-049.

**Auth:** Required (Bearer token)

**Query parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `startDate` | DateTime | Yes | Start of the date range (ISO 8601) |
| `endDate` | DateTime | Yes | End of the date range (ISO 8601) |
| `onboardingStatus` | string | Yes | Onboarding status filter (e.g., `"P"`) |
| `messageType` | string | No | Optional message type filter (max 10 chars) |

**Response:** `200 OK`

```json
{
  "points": [
    { "label": "Mon", "errors": 4, "warnings": 9 },
    { "label": "Tue", "errors": 2, "warnings": 11 },
    { "label": "Wed", "errors": 0, "warnings": 0 }
  ]
}
```

Labels are formatted server-side based on the date range span (same rule as `/Volumes`):
- ≤ 7 days: day-of-week (`"ddd"`)
- ≤ 31 days: date (`"MM/dd"`)
- &gt; 31 days: month abbreviation (`"MMM"`)

Missing dates in the requested range are zero-filled so the x-axis remains contiguous. When multiple dates collapse to the same label (e.g., monthly buckets), Errors and Warnings are summed per label.

**Error responses:**

| Status | Condition |
|---|---|
| `401 Unauthorized` | No valid JWT or missing `user_role_id` claim |
---

## GET /api/admin/alerts

Returns all alerts ordered by `StartDate` descending. Added by FEAT-038.

**Auth:** Required (Bearer token)
**Policy:** `AlertAdministrator` (MVPS Support Manager, MVPS User Support Manager only)

**Response:** `200 OK` — `AlertListItemDto[]`

```json
[
  {
    "alertId": 1,
    "title": "Planned Maintenance",
    "description": "System will be down for 4 hours.",
    "type": "Outage",
    "allUsers": true,
    "programId": null,
    "jurisdictionId": null,
    "startDate": "2026-04-17T10:00:00",
    "endDate": "2026-04-17T14:00:00",
    "createdBy": "admin_user",
    "createdDate": "2026-04-17T08:00:00",
    "updatedBy": null,
    "updatedDate": null
  }
]
```

**Error responses:**

| Status | Condition |
|---|---|
| `401 Unauthorized` | No valid JWT |
| `403 Forbidden` | Role not in AlertAdministrator policy |

---

## POST /api/admin/alerts

Creates a new alert. `CreatedBy` is set from the `account_identifier` JWT claim. Added by FEAT-038.

**Auth:** Required (Bearer token)
**Policy:** `AlertAdministrator`

**Request body:** `SaveAlertRequest` (Title, Description, Type, AllUsers, StartDate, EndDate, ProgramId?, JurisdictionId?)

**Response:** `201 Created` — `AlertListItemDto` of the newly created alert.

**Error responses:**

| Status | Condition |
|---|---|
| `400 Bad Request` | Validation error (e.g., invalid Type, EndDate ≤ StartDate) |
| `401 Unauthorized` | No valid JWT or missing `account_identifier` claim |
| `403 Forbidden` | Role not in AlertAdministrator policy |

---

## PUT /api/admin/alerts/{id}

Updates an existing alert. `UpdatedBy` is set from the `account_identifier` JWT claim. Added by FEAT-038.

**Auth:** Required (Bearer token)
**Policy:** `AlertAdministrator`

**Path parameters:** `id` (int) — AlertId

**Request body:** `SaveAlertRequest`

**Response:** `200 OK` — updated `AlertListItemDto`.

**Error responses:**

| Status | Condition |
|---|---|
| `400 Bad Request` | Validation error |
| `401 Unauthorized` | No valid JWT or missing `account_identifier` claim |
| `403 Forbidden` | Role not in AlertAdministrator policy |
| `404 Not Found` | Alert with given ID does not exist |

---

## DELETE /api/admin/alerts/{id}

Permanently deletes an alert. Added by FEAT-038.

**Auth:** Required (Bearer token)
**Policy:** `AlertAdministrator`

**Path parameters:** `id` (int) — AlertId

**Response:** `204 No Content`

**Error responses:**

| Status | Condition |
|---|---|
| `401 Unauthorized` | No valid JWT |
| `403 Forbidden` | Role not in AlertAdministrator policy |
| `404 Not Found` | Alert with given ID does not exist |

---

## GET /api/Activity

**Feature:** FEAT-035 Recent Activity Feed
**Auth:** Required (Bearer token or `auth_token` cookie)

Returns a combined, time-ordered list of the authenticated user's recent Stratification Reports and Export Requests, scoped to the requesting user's `user_role_id` JWT claim.

**Query parameters:**

| Parameter | Type | Required | Default | Description |
|---|---|---|---|---|
| `limit` | int | No | `10` | Number of items to return. Max: 200. Must be > 0. |

**Response:** `200 OK`

```json
[
  {
    "id": 42,
    "type": "Export",
    "fileName": "export_2026_01.csv",
    "stratificationType": null,
    "mmwrYear": 2026,
    "mmwrWeekFrom": 1,
    "mmwrWeekTo": 4,
    "status": "Completed",
    "createdBy": "jsmith",
    "createdDate": "2026-04-07T14:30:00"
  },
  {
    "id": 17,
    "type": "Report",
    "fileName": null,
    "stratificationType": "Weekly Summary",
    "mmwrYear": 2026,
    "mmwrWeekFrom": 1,
    "mmwrWeekTo": 4,
    "status": "Complete",
    "createdBy": "jsmith",
    "createdDate": "2026-04-06T09:15:00"
  }
]
```

Items are sorted by `createdDate` descending (newest first). `type` is `"Export"` or `"Report"`.

**Error responses:**

| Status | Condition |
|---|---|
| `400 Bad Request` | `limit` is ≤ 0 |
| `401 Unauthorized` | No valid JWT or missing `user_role_id` claim |
