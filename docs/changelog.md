# Changelog

> Auto-maintained by the Developer agent. Do not edit manually.
> New entries are prepended at the top after every completed feature.
> Format defined in `.claude/skills/update-docs.md`.

---

## [FEAT-049] Errors & Warnings Grouped Bar Chart — 2026-05-28

**Status:** ✅ Done

**Summary:** The Dashboard's `ErrorsAndWarningsChart` card now renders real per-`ReceivedDate` errors/warnings counts instead of hard-coded fake data. New endpoint `GET /api/Dashboard/ErrorsAndWarnings` reuses the existing `GetMessageStatusesByAll` stored procedure consumed as a multi-row projection (`ReceivedDate, Errors, Warnings`) — FEAT-010's aggregate projection is unchanged. The service zero-fills missing dates and labels by span (`ddd` for ≤7 days, `MM/dd` for ≤31, `MMM` otherwise); buckets sharing a label sum Errors and Warnings. Time-range menu (3 Days / 1 Week / 1 Month / 3 Months / 6 Months) defaults to **3 Days**. The bucketing helpers were extracted from `MessageVolumeService` into a shared `DateBucketing` static class so both dashboard charts share the same logic.

**New files:**
- `specs/features/errors-and-warnings-chart.md`
- `src/Core/Services/DateBucketing.cs`
- `src/Core/DTOs/ErrorsWarningsDto.cs`
- `src/Core/Interfaces/IErrorsWarningsRepository.cs`, `IErrorsWarningsService.cs`
- `src/Core/Services/ErrorsWarningsService.cs`
- `src/Infrastructure/Models/SpErrorsWarningsResult.cs`
- `src/Infrastructure/Repositories/ErrorsWarningsRepository.cs`
- `tests/UnitTests/DateBucketingTests.cs` (8 tests)
- `tests/UnitTests/ErrorsWarningsServiceTests.cs` (7 tests)
- `tests/IntegrationTests/FakeErrorsWarningsRepository.cs`
- `tests/IntegrationTests/DashboardErrorsAndWarningsEndpointTests.cs` (4 tests)

**Modified files:**
- `src/Api/Program.cs` — registered `IErrorsWarningsRepository` + `IErrorsWarningsService`
- `src/Api/Endpoints/DashboardEndpoints.cs` — added `GET /api/Dashboard/ErrorsAndWarnings`
- `src/Core/Services/MessageVolumeService.cs` — delegate to `DateBucketing` helpers (behavior-identical)
- `src/Portal/Services/Interfaces/IDashboardApiClient.cs` — added `GetErrorsAndWarningsAsync`
- `src/Portal/Services/DashboardApiClient.cs` — implemented `GetErrorsAndWarningsAsync`
- `src/Portal/Components/Pages/Dashboard/Components/ErrorsAndWarningsChart.razor.cs` — replaced `LoadFakeData()` with `LoadDataAsync()` API call
- `specs/_index.md` — added FEAT-049 row

---

## [FEAT-048] Dashboard Message Volume Map — 2026-05-26

**Status:** ✅ Done

**Summary:** The Dashboard's stubbed `MessagesMap` card is now wired to live data via the `GetMessagesForMap` stored procedure. New endpoint `GET /api/Dashboard/Map` accepts `startDate`, `endDate`, `onboardingStatus`, and optional `messageType`; the user role comes from the `user_role_id` JWT claim. Response is `MessageMapResultDto` containing the full SP row set (id, code, alt code, description, volume). The Portal projects rows into an `IReadOnlyDictionary<string,double>` keyed by trimmed/uppercased `JurisdictionAltCode` and binds it to the existing `UsChoroplethMap`. The card adds an options menu (3 Days, 1 Week, 1 Month, 3 Months, 6 Months) mirroring `MessagesVolume`, defaulting to 3 Days. A sequence-token cancellation guard discards stale responses when the user switches ranges quickly.

**New files:**
- `specs/features/dashboard-message-volume-map.md`
- `src/Core/DTOs/MessageMapResultDto.cs`
- `src/Core/Interfaces/IMessageMapRepository.cs`, `IMessageMapService.cs`
- `src/Core/Services/MessageMapService.cs`
- `src/Infrastructure/Models/SpMessageMapResult.cs`
- `src/Infrastructure/Repositories/MessageMapRepository.cs`
- `tests/UnitTests/MessageMapServiceTests.cs` (4 tests)
- `tests/IntegrationTests/FakeMessageMapRepository.cs`
- `tests/IntegrationTests/DashboardMapEndpointTests.cs` (4 tests)

**Modified files:**
- `src/Api/Program.cs` — registered `IMessageMapRepository` + `IMessageMapService`
- `src/Api/Endpoints/DashboardEndpoints.cs` — added `GET /api/Dashboard/Map`
- `src/Portal/Services/Interfaces/IDashboardApiClient.cs` — added `GetMessageMapAsync`
- `src/Portal/Services/DashboardApiClient.cs` — implemented `GetMessageMapAsync`
- `src/Portal/Components/Pages/Dashboard/Components/MessagesMap.razor` + `.razor.cs` — replaced sample data with real API fetch + range menu

**Tests:** 4 unit + 4 integration added; full suite 323 unit + 132 integration passing.

---

## [FEAT-047] Jurisdiction Data Sources — 2026-05-25

**Status:** ✅ Done

**Summary:** New `/jurisdictions/datasources` list page renders the result set from `dbo.GetDatasources` (`CDE_Dashboard`) in a `FluentCustomizableTable` with all 24 returned columns visible by default and the standard column-customizer/Export/Filter toolbar. Read access is open to any authenticated user with a role claim; the SP's `@UserRoleID` parameter is taken from the JWT `user_role_id` claim. Filters from the existing shared `FiltersPanel` are wired for Jurisdiction, Program, Event Code, Category, Profile, and Message Type (6 of 7 SP params). MVPS administrators (`MVPS Support Manager`, `MVPS User Support Manager`) see a "+ New Data Source" button inside the table toolbar that opens a slide-out side panel for creating a record via `POST /api/datasources` — enforced by a new `MvpsAdmin` named authorization policy. Create writes through a new `Datasource` entity (EF Core) mapped to a `Datasources` table; the response payload re-projects through the same SP so the row in the UI matches the GET shape. Onboarding statuses are `O` (Onboarding) and `P` (Production). Sidebar gets a new "Data Sources" link next to "Members".

**New files:**
- `specs/features/jurisdiction-datasources.md`
- `src/Core/DTOs/DatasourceDto.cs`, `DatasourceSearchParams.cs`, `CreateDatasourceRequest.cs`
- `src/Core/Entities/Datasource.cs`
- `src/Core/Interfaces/IDatasourceRepository.cs`, `IDatasourceService.cs`
- `src/Core/Services/DatasourceService.cs`
- `src/Infrastructure/Models/SpDatasourceResult.cs`
- `src/Infrastructure/Repositories/DatasourceRepository.cs`
- `src/Api/Endpoints/DatasourceEndpoints.cs`
- `src/Portal/Services/Interfaces/IDatasourceApiClient.cs`
- `src/Portal/Services/DatasourceApiClient.cs`
- `src/Portal/Components/Pages/Jurisdictions/Datasources/DatasourceCreatePanel.razor` + `.razor.cs`
- `tests/UnitTests/DatasourceServiceTests.cs` (7 tests, all passing)

**Modified files:**
- `src/Infrastructure/Data/AppDbContext.cs` — added `DbSet<Datasource>` + `OnModelCreating` entity config (physical table `JurisdictionProfile`, `OnboardingStatus char(1)`, `SendingApplication max 100`, `CreatedBy/UpdatedBy max 20`)
- `src/Api/Program.cs` — added DI for `IDatasourceRepository`/`IDatasourceService`, new `MvpsAdmin` policy, and `app.MapDatasourceEndpoints()`
- `src/Portal/Program.cs` — added `IDatasourceApiClient` DI registration
- `src/Portal/Components/Layout/Bars/SideBar.razor` — added top-level "Data Sources" nav link under Jurisdictions area
- `src/Portal/Components/Pages/Jurisdictions/Datasources/Datasources.razor` + `.razor.cs` — replaced empty scaffold with full page (24-column table + create panel)

**Deferred to follow-up:**
- `OnboardingStatus` UI filter in `FiltersPanel` (API already accepts the query param)
- EF migration generation against the live `CDE_Dashboard` — entity config is in place; DBAs to run `dotnet ef migrations add` against the appropriate branch
- Integration tests + Playwright E2E

---

## [FEAT-046] Members Dashboard Widget — 2026-05-23

**Status:** ✅ Done

**Summary:** New Members widget on the dashboard (`/`) gives External users an at-a-glance view of their jurisdiction's members alongside the existing TransactionTable in a `grid-row-halves` row. Internal users see no change — TransactionTable remains full-width. Widget uses the shared `FluentCustomizableTable` + `ColumnsPanel` pattern (column show/hide enabled; no filter panel) wrapped in the standard `table-page > table-content-layout > table-main > Card` scaffolding, matching the TransactionTable widget. Subtitle shows "{JurisdictionName} — Total Members: {N}". Seven columns: composite "Member" (LastName, FirstName (AccountIdentifier)), the four FEAT-045 boolean role flags as Yes/No, Status badge, and an Email column rendered as a stealth `FluentAnchor` opening `mailto:{EmailAddress}` in the default client. The widget reuses the existing `GET /api/members` endpoint from FEAT-045 — no API work needed; the endpoint already jurisdiction-scopes for External callers. One additive field (`AccountIdentifier`) was added to `MemberListItemDto` and projected in `MemberListRepository`. Dashboard layout switching is driven by a tiny `Home.razor.cs` reading the `user_type` claim from the cascading `AuthenticationState`. Subtitle reads `jurisdiction_description` claim (with a fallback to the first member row's `Jurisdiction` if the claim is empty).

**New files:**
- `specs/features/dashboard-members-widget.md`
- (filled the existing stubs at `src/Portal/Components/Pages/Dashboard/Components/Members.razor(.cs/.css)`)

**Modified files:**
- `src/Core/DTOs/MemberListItemDto.cs` — added `AccountIdentifier`
- `src/Infrastructure/Repositories/MemberListRepository.cs` — projected `AccountIdentifier`
- `src/Portal/Components/Pages/Home.razor` — conditional grid-row-halves layout for External users; removed commented-out hint
- `src/Portal/Components/Pages/Home.razor.cs` — reads `user_type` claim to drive layout switch
- `src/Portal/Components/Pages/Dashboard/Components/Members.razor` — `table-page > table-content-layout > table-main > Card` scaffolding wrapping `<FluentCustomizableTable>` with 7 `TableColumn<MemberListItemDto>` entries; `<ColumnsPanel>` sibling for column show/hide
- `src/Portal/Components/Pages/Dashboard/Components/Members.razor.cs` — injects `IMemberApiClient`, reads `jurisdiction_description` claim from cascading `AuthenticationState` for the card subtitle (falls back to the first row's `Jurisdiction` if claim is missing); manages `ActiveTablePanel` for the columns side panel; `TableToolbarConfig` enables Columns + Refresh, disables Filter + Export
- `src/Portal/Components/Pages/Dashboard/Components/Members.razor.css` — placeholder (status-badge styles live globally in `app.css`)
- `specs/_index.md` — appended FEAT-046 row

---

## [FEAT-045] Jurisdiction Members Browse Page — 2026-05-23

**Status:** ✅ Done

**Summary:** New `/jurisdictions/members` page lets jurisdiction users and admins browse External users with jurisdiction assignments. The four role-based boolean columns (IsJurisdictionUser, IsJurisdictionManager, IsStateTerritorialEPI, IsSTDManager) are derived at query time via case-insensitive `Contains` matches on `Role.Name`, with substrings centralized in `Core.Constants.MemberRoleSubstrings` — they are display-only, not filterable. New `MemberBrowser` authorization policy allows access for the four AdminDataManager roles OR any user with `user_type=External`. Server-side scoping: MVPS admins and CDC Program Data Manager see all members; Jurisdiction Data Manager and external users are scoped to their `jurisdiction_id` claim regardless of any client-supplied `jurisdictionId` query param. Sidebar shows the "Members" link conditionally using the same eligibility check. Filter panel reuses the shared `FiltersPanel` with `IncludeRoles=true` so members can be narrowed by a single role selection.

**New files:**
- `specs/features/jurisdiction-members-browse.md`
- `src/Core/DTOs/MemberListItemDto.cs`
- `src/Core/DTOs/MemberSearchParams.cs`
- `src/Core/DTOs/MemberDataScope.cs`
- `src/Core/Constants/MemberRoleSubstrings.cs`
- `src/Core/Interfaces/IMemberListRepository.cs`
- `src/Infrastructure/Repositories/MemberListRepository.cs`
- `src/Api/Endpoints/MemberEndpoints.cs`
- `src/Portal/Services/Interfaces/IMemberApiClient.cs`
- `src/Portal/Services/MemberApiClient.cs`
- `src/Portal/Components/Pages/Jurisdictions/Members.razor`
- `src/Portal/Components/Pages/Jurisdictions/Members.razor.cs`

**Modified files:**
- `src/Api/Program.cs` — registered `IMemberListRepository`, added `MemberBrowser` policy (roles OR `user_type=External`), mapped endpoint
- `src/Portal/Program.cs` — registered `IMemberApiClient`
- `src/Portal/Components/Layout/Bars/SideBar.razor(.cs)` — added "Members" nav link gated by `_showJurisdictionMembers` (admin roles OR External user_type)
- No changes to shared filter components — reused the existing `IncludeRoles` autocomplete

**Fix (2026-05-23):** The four role-derived boolean columns were showing "No" for actual role holders because the original case-insensitive `Contains` rule required contiguous substrings — e.g., `"Jurisdiction Data Manager".Contains("jurisdiction manager")` is `false` because of the "Data" word in between. Switched to **exact-match (case-insensitive)** against the four canonical role names. Renamed `Core.Constants.MemberRoleSubstrings` → `MemberRoleNames`; helper is now `EqualsIgnoreCase(roleName, target)`. Canonical names: `Jurisdiction User`, `Jurisdiction Data Manager`, `State/Territorial EPI`, `STD Manager`. Files touched: `src/Core/Constants/MemberRoleNames.cs` (new, replaces deleted `MemberRoleSubstrings.cs`), `src/Infrastructure/Repositories/MemberListRepository.cs` (flag-derivation lines), spec §5 table.

**Enhancement (2026-05-23):** Restricted the Roles filter autocomplete on the Members page to `Role.RoleGroup = "Jurisdiction"`. Added optional `roleGroup` parameter to the roles lookup chain: `IFilterRepository.GetActiveRolesAsync(string? roleGroup, …)` → `GET /api/filters/Roles?roleGroup=…` → `IFilterApiClient.GetRolesAsync(string? roleGroup)` → `FiltersPanel.LoadRolesAsync` reads new `SearchConfig.RoleGroupFilter`. Members page sets `RoleGroupFilter = "Jurisdiction"`; all other pages (AdminUsers, etc.) are unaffected since the default is `null` (returns all active roles). Files touched: `src/Core/Interfaces/IFilterRepository.cs`, `src/Infrastructure/Repositories/FilterRepository.cs`, `src/Api/Endpoints/FilterEndpoints.cs`, `src/Portal/Services/Interfaces/IFilterApiClient.cs`, `src/Portal/Services/FilterApiClient.cs`, `src/Portal/Components/UI/SearchControls/Models/SearchConfig.cs`, `src/Portal/Components/UI/Panels/FiltersPanel/FiltersPanel.razor.cs`, `src/Portal/Components/Pages/Jurisdictions/Members.razor.cs`.

---

## [FEAT-044] Case Listings — 2026-05-19

**Status:** ✅ Done

**Summary:** Wired the Cases Listing page (`/cases/listing`) to the real `GetCaseListing_Total` stored procedure in CDE_Dashboard (AppDbContext), replacing 200-row fake data with live API results. Added `GET /api/cases` endpoint accepting 15 filter parameters. Extended the filter panel with ClassificationStatus, ProcessingStatus, EventCode, and CaseId filters (all flags already existed in `SearchConfig` and `FiltersPanel`). The `@ShowAll` parameter is hardcoded to 0; `@UseMmwr` is derived from which date mode is active; `@OnboardingStatus` always passes `App.OnboardingStatus` ("P"). The Portal API client maps flat `CaseListingItemDto` rows to the existing `CdcCase` Portal model, so the table columns and CasesPanel side drawer are unchanged.

**New files:**
- `specs/features/case-listings.md`
- `src/Core/DTOs/CaseListingItemDto.cs`
- `src/Core/DTOs/CaseListingResultDto.cs`
- `src/Core/DTOs/CaseListingSearchParams.cs`
- `src/Core/Interfaces/ICaseListingRepository.cs`
- `src/Core/Interfaces/ICaseListingService.cs`
- `src/Core/Services/CaseListingService.cs`
- `src/Infrastructure/Models/SpCaseListingResult.cs`
- `src/Infrastructure/Repositories/CaseListingRepository.cs`
- `src/Portal/Services/Interfaces/ICaseListingApiClient.cs`
- `src/Portal/Services/CaseListingApiClient.cs`

**Modified files:**
- `src/Api/Endpoints/CaseEndpoints.cs` — added `GET /api/cases` listing endpoint
- `src/Api/Program.cs` — registered `ICaseListingRepository`, `ICaseListingService`
- `src/Portal/Program.cs` — registered `ICaseListingApiClient`
- `src/Portal/Components/Pages/Cases/Listing.razor.cs` — replaced fake data with real API call, implemented `ApplyCurrentFiltersAsync`, enabled ClassificationStatuses/ProcessingStatuses/EventCodes/CaseId filter flags

---

## [FEAT-043] Message Errors & Warnings Detail View — 2026-05-04

**Status:** ✅ Done

**Summary:** Fills the previously empty "Errors & Warnings" section in `MessageDetailPanel` with live validation records fetched from `[dashboard].[validation_error_extended_vw]` (CDE_ENV database). When a user opens a message with a non-zero error or warning count, the panel calls `GET /api/Messages/{msgTransactionId}/ErrorsAndWarnings` and renders results in two sub-sections (Errors, then Warnings), each showing the count in the sub-heading. Each card displays the error description, validation type, and up to two data element fields (omitted when null). The API call is skipped entirely when both counts are zero. A `FluentMessageBar` (Error intent) is shown on failure. Also fixed a pre-existing build break: `MessageDetailDto` had gained new fields (`IsMessageMalformed`, `IsLegacyCaseInactivated`, `CaseProcessingStatus`, `CaseClassificationStatus`) in the merged PR but the constructor calls in `MessageEndpointsTests` and `MessageSearchServiceTests` were not updated; and `MessagesQueryStringFilterTests` referenced `Messages` under the wrong namespace after the component was moved into a subfolder.

**New files:**
- `src/Core/DTOs/ValidationErrorDto.cs`
- `src/Core/Interfaces/IValidationErrorRepository.cs`
- `src/Infrastructure/Models/ValidationErrorResult.cs`
- `src/Infrastructure/Repositories/ValidationErrorRepository.cs`
- `tests/IntegrationTests/FakeValidationErrorRepository.cs`
- `tests/IntegrationTests/MessageErrorsAndWarningsEndpointsTests.cs`
- `tests/UnitTests/ValidationErrorRepositoryTests.cs`

**Modified files:**
- `src/Api/Endpoints/MessageEndpoints.cs` — added `GET /{msgTransactionId}/ErrorsAndWarnings`
- `src/Api/Program.cs` — registered `IValidationErrorRepository`
- `src/Portal/Services/Interfaces/IMessageApiClient.cs` — added `GetErrorsAndWarningsAsync`
- `src/Portal/Services/MessageApiClient.cs` — implemented `GetErrorsAndWarningsAsync`
- `src/Portal/Components/UI/Panels/MessageDetailPanel/MessageDetailPanel.razor` — rendered E&W section
- `src/Portal/Components/UI/Panels/MessageDetailPanel/MessageDetailPanel.razor.cs` — added state, fetch logic, LINQ filters
- `tests/IntegrationTests/MessageEndpointsTests.cs` — fixed `MessageDetailDto` constructor (missing new fields)
- `tests/UnitTests/MessageSearchServiceTests.cs` — fixed `MessageDetailDto` constructor (missing new fields)
- `tests/UnitTests/MessagesQueryStringFilterTests.cs` — fixed namespace after Messages component moved to subfolder
- `specs/_index.md` — registered FEAT-043
- `docs/api.md` — documented new endpoint
- `docs/changelog.md` — this entry

---

## [FEAT-041] Search Messages — 2026-05-01

**Status:** ✅ Done

**Summary:** Replaced fake data on the Messages page (`/messages`) with live results from the stored procedure `GetMessageDetailsByParameters_TOTAL` (CDE_Dashboard database). The FilterPanel drives the search via 12 optional parameters (date range, program, message type, status, category, jurisdiction, event code, control/transaction/case ID, upload file name). `UserRoleId` is always sourced from the JWT claim; `ShowAll` is hardcoded to 0; `OnboardingStatus` comes from `App.OnboardingStatus`. The `@Total OUTPUT` parameter is returned and displayed as the Card subtitle count. Page loads in empty state — the SP is only called after the user clicks Apply. This is the first stored procedure call in the project to use an OUTPUT parameter.

**New files:**
- `src/Core/DTOs/MessageDetailDto.cs`
- `src/Core/DTOs/MessageSearchResultDto.cs`
- `src/Core/Interfaces/IMessageSearchRepository.cs`
- `src/Core/Interfaces/IMessageSearchService.cs`
- `src/Core/Services/MessageSearchService.cs`
- `src/Infrastructure/Models/SpMessageDetailResult.cs`
- `src/Infrastructure/Repositories/MessageSearchRepository.cs`
- `src/Api/Endpoints/MessageEndpoints.cs`
- `src/Portal/Services/Interfaces/IMessageApiClient.cs`
- `src/Portal/Services/MessageApiClient.cs`
- `tests/UnitTests/MessageSearchServiceTests.cs`
- `tests/IntegrationTests/FakeMessageSearchRepository.cs`
- `tests/IntegrationTests/MessageEndpointsTests.cs`

**Modified files:**
- `src/Portal/Components/Pages/Messages.razor` — replaced fake data, wired `MessageDetailDto`, added empty-state prompt and error bar
- `src/Portal/Components/Pages/Messages.razor.cs` — removed fake data; added `IMessageApiClient` injection, `ApplyFilters` API call, `_total`/`_hasSearched`/`_errorMessage` state
- `src/Api/Program.cs` — registered `IMessageSearchRepository`, `IMessageSearchService`, mapped `MessageEndpoints`
- `src/Portal/Program.cs` — registered `IMessageApiClient`

**New migration:** none

**Tests:** 3 unit + 4 integration added (290 unit, 120 integration total)

**Spec:** `specs/features/search-messages.md`

---

## [FEAT-040] API Health Check Endpoints — 2026-04-28

**Status:** ✅ Done

**Summary:** Added five unauthenticated health check endpoints available in all environments. `GET /alive` confirms the API process is responsive (liveness). `GET /health/dashboard`, `/health/cde-env`, `/health/cde-person`, and `/health/cde-report` each probe the corresponding database using `CanConnectAsync` (readiness). All endpoints return standard ASP.NET Core health check JSON; 200 = Healthy, 503 = Unhealthy/Degraded. Exception messages are suppressed — responses only emit a sanitized description string so connection strings cannot leak.

**New files:**
- `src/Api/HealthChecks/DbContextHealthCheck.cs` — generic `IHealthCheck` wrapping `DbContext.Database.CanConnectAsync`
- `src/Api/Endpoints/HealthEndpoints.cs` — `MapHealthEndpoints` extension + JSON response writer
- `tests/IntegrationTests/HealthEndpointsTests.cs` — 9 integration tests

**Modified files:**
- `src/Api/Program.cs` — registered `AddHealthChecks()` with 5 named checks; called `app.MapHealthEndpoints()`

---

## [FEAT-039] Dashboard Active Alerts — 2026-04-25

**Status:** ✅ Done

**Summary:** Implemented the user-facing side of the alert system. Authenticated users now see active, audience-matched alerts on the dashboard inside the `Alerts.razor` component. Alerts are filtered server-side from JWT claims (`user_type`, `program_id`, `jurisdiction_id`); AlertAdministrator-role users bypass audience filtering and see all active alerts. Results are sorted Outage → Warning → Info → Tip, then newest StartDate first. Users can dismiss individual alerts; dismissed IDs are stored in a 30-day browser cookie (`dismissed_alerts`) and restored on reload. Clearing cookies resets dismissals. The component renders nothing when no alerts apply.

**New files:**
- `src/Core/DTOs/ActiveAlertDto.cs` — slim user-facing DTO (AlertId, Title, Description, Type)
- `src/Core/Interfaces/IActiveAlertRepository.cs` — read-only alert repository interface
- `src/Core/Interfaces/IActiveAlertService.cs` — service interface
- `src/Core/Services/ActiveAlertService.cs` — audience filtering + sort logic
- `src/Infrastructure/Repositories/ActiveAlertRepository.cs` — EF query for date-active alerts
- `src/Portal/Services/Interfaces/IActiveAlertApiClient.cs`
- `src/Portal/Services/ActiveAlertApiClient.cs`
- `tests/UnitTests/ActiveAlertServiceTests.cs` — 14 tests
- `tests/IntegrationTests/FakeActiveAlertRepository.cs`
- `tests/IntegrationTests/DashboardAlertEndpointsTests.cs` — 4 tests

**Modified files:**
- `src/Api/Endpoints/DashboardEndpoints.cs` — added `GET /api/Dashboard/Alerts`
- `src/Api/Program.cs` — registered IActiveAlertRepository, IActiveAlertService
- `src/Portal/Program.cs` — registered IActiveAlertApiClient
- `src/Portal/Components/Pages/Dashboard/Components/Alerts.razor` — implemented
- `src/Portal/Components/Pages/Dashboard/Components/Alerts.razor.cs` — implemented
- `src/Portal/wwwroot/js/tab-session.js` — added `window.cookieHelper` for dismiss cookie

**New migration:** none — uses existing `Alert` table from FEAT-038

**Spec:** `specs/features/dashboard-active-alerts.md`

---

## [FEAT-038] Alert Management — 2026-04-17

**Status:** ✅ Done

**Summary:** Added a full CRUD admin page for managing system-wide alerts at `/administration/alert-builder/alerts`. Admins can create, edit, and delete alerts with a title, description, severity type (Tip/Info/Warning/Outage), start/end date+time, and a targeted audience (All Users, All Jurisdiction Users, All Program Users, a specific program, or a specific jurisdiction). The audience dropdown is populated in parallel with the alert list via existing filter endpoints. Create/Edit uses an inline right-side panel with client-side validation. Delete requires confirmation via a modal dialog. Protected by a new `AlertAdministrator` policy (MVPS Support Manager + MVPS User Support Manager only).

**New files:**
- `src/Core/Entities/Alert.cs` — Alert entity (maps to existing `Alert` table in CDE_Dashboard)
- `src/Core/DTOs/AlertListItemDto.cs` — DTO for list/create/update responses
- `src/Core/DTOs/SaveAlertRequest.cs` — request DTO for create/update
- `src/Core/Interfaces/IAlertRepository.cs` — repository interface
- `src/Core/Interfaces/IAlertService.cs` — service interface
- `src/Core/Services/AlertService.cs` — validation + delegation logic
- `src/Infrastructure/Repositories/AlertRepository.cs` — EF Core CRUD implementation
- `src/Api/Endpoints/AlertEndpoints.cs` — 4 endpoints (GET/POST/PUT/DELETE)
- `src/Portal/Services/Interfaces/IAlertApiClient.cs` — Portal HTTP client interface
- `src/Portal/Services/AlertApiClient.cs` — Portal HTTP client implementation
- `src/Portal/Components/Pages/Administration/Alert-Builder/Alerts.razor` — list page
- `src/Portal/Components/Pages/Administration/Alert-Builder/Alerts.razor.cs` — list page code-behind
- `src/Portal/Components/Pages/Administration/Alert-Builder/Components/AlertFormPanel.razor` — create/edit panel
- `src/Portal/Components/Pages/Administration/Alert-Builder/Components/AlertFormPanel.razor.cs` — panel code-behind
- `tests/UnitTests/AlertServiceTests.cs` — 8 unit tests
- `tests/IntegrationTests/FakeAlertRepository.cs` — in-memory fake repository
- `tests/IntegrationTests/AlertEndpointsTests.cs` — 9 integration tests

**Modified files:**
- `src/Infrastructure/Data/AppDbContext.cs` — added `DbSet<Alert> Alerts` + `OnModelCreating` config
- `src/Api/Program.cs` — registered `IAlertRepository`, `IAlertService`, `AlertAdministrator` policy, mapped `AlertEndpoints`
- `src/Portal/Program.cs` — registered `IAlertApiClient`
- `specs/features/alert-management.md` — new spec
- `specs/_index.md` — added FEAT-038 entry

---

## [FEAT-037] Activity View & Download Actions — 2026-04-09

**Status:** ✅ Done

**Summary:** Added per-item action buttons to ActivityPopover and HistoryPanel. Report (StratificationCache) items get a "View" button that navigates to `/reports/stratifications/{stratificationId}`. Export (ExportRequest) items get a "Download" button that streams the CSV file from the server file system via `GET /api/Activity/download/{exportRequestId}` and triggers a browser file download via JS interop. Also fixed three display bugs: (1) `activity-item-badge--report` CSS class was missing — Report items now share the green success style; (2) status is now a colored badge (info/success/danger/warning by value) instead of plain text; (3) status badge and action button share a flex row (status left, button right). Changes applied identically to both `ActivityPopover` and `HistoryPanel`.

**New files:**
- `src/Portal/Models/ExportDownloadResult.cs` — download result record
- `specs/features/activity-view-download.md` — FEAT-037 spec

**Modified files:**
- `src/Core/Interfaces/IActivityRepository.cs` — added `GetExportFileNameAsync`
- `src/Core/Interfaces/IActivityService.cs` — added `GetExportDownloadInfoAsync`
- `src/Core/Services/ActivityService.cs` — implemented `GetExportDownloadInfoAsync`
- `src/Infrastructure/Repositories/ActivityRepository.cs` — implemented `GetExportFileNameAsync`
- `src/Api/Endpoints/ActivityEndpoints.cs` — added `GET /api/Activity/download/{exportRequestId:int}`
- `src/Api/appsettings.json` — added `Export:RootPath` config key
- `src/Portal/Services/Interfaces/IActivityApiClient.cs` — added `DownloadExportAsync`
- `src/Portal/Services/ActivityApiClient.cs` — implemented `DownloadExportAsync`
- `src/Portal/wwwroot/js/tab-session.js` — added `triggerFileDownload` JS helper
- `src/Portal/Components/Layout/Bars/Components/ActivityPopover.razor` — new item template
- `src/Portal/Components/Layout/Bars/Components/ActivityPopover.razor.cs` — NavigationManager, IJSRuntime, handlers
- `src/Portal/Components/Layout/Bars/Components/ActivityPopover.razor.css` — report badge, status badge, actions row
- `src/Portal/Components/UI/Panels/HistoryPanel/HistoryPanel.razor` — new item template
- `src/Portal/Components/UI/Panels/HistoryPanel/HistoryPanel.razor.cs` — NavigationManager, IJSRuntime, handlers
- `src/Portal/Components/UI/Panels/HistoryPanel/HistoryPanel.razor.css` — full activity item styles + status badge + actions row

**Config required:**
- Set `Export:RootPath` in `Api/appsettings.json` (or user-secrets) to the directory where CSV export files are stored

---

## [FEAT-036] Activity Button & Panel — 2026-04-08

**Status:** ✅ Done

**Summary:** Wired up the TopBar Activity button stub. Clicking it opens a `Popover` (bottom-anchored, 360px) that fetches and displays the top 10 recent activity items via `GET /api/Activity?limit=10`. Each row shows a type badge (Export/Report), description, status, and date. A "View all Activity" button at the popover footer closes the popover and opens the repurposed `HistoryPanel` as a 500px right-side panel via `DialogService.ShowPanelAsync`, loading the full list (`limit=200`). Empty state, loading spinner, and error message are all handled. `HistoryPanel` was repurposed from a Case History stub to implement `IDialogContentComponent<string>`.

**New files:**
- `src/Portal/Services/Interfaces/IActivityApiClient.cs`
- `src/Portal/Services/ActivityApiClient.cs` — `GET /api/Activity?limit=` wrapper

**Modified files:**
- `src/Portal/Components/Layout/Bars/TopBar.razor` — added `Popover` with activity list + "View all Activity" button
- `src/Portal/Components/Layout/Bars/TopBar.razor.cs` — injected `IActivityApiClient`/`IDialogService`; added toggle, load, and open-panel methods
- `src/Portal/Components/UI/Panels/HistoryPanel/HistoryPanel.razor` — repurposed from Case History stub to full activity list
- `src/Portal/Components/UI/Panels/HistoryPanel/HistoryPanel.razor.cs` — now implements `IDialogContentComponent<string>`
- `src/Portal/Program.cs` — registered `IActivityApiClient → ActivityApiClient`
- `specs/_index.md` — marked FEAT-036 Done
- `docs/changelog.md` — this entry

---

## [FEAT-035] Recent Activity Feed — 2026-04-08

**Status:** ✅ Done

**Summary:** Added `GET /api/Activity?limit=10` endpoint that returns a combined, time-ordered list of the current user's recent Stratification Reports (`StratificationCache`) and Export Requests (`ExportRequest`), scoped to the requesting user's `UserRoleId` JWT claim. Results are sorted by `CreatedDate` descending. The `limit` query parameter controls how many items are returned (default 10, max 200). Each item carries a `type` discriminator (`"Export"` or `"Report"`).

**New files:**
- `src/Core/DTOs/ActivityItemDto.cs` — combined response DTO with `type` discriminator
- `src/Core/Interfaces/IActivityRepository.cs`
- `src/Core/Interfaces/IActivityService.cs`
- `src/Core/Services/ActivityService.cs` — thin pass-through with logging
- `src/Infrastructure/Entities/ExportRequest.cs` — EF entity (no migration)
- `src/Infrastructure/Entities/StratificationCache.cs` — EF entity (no migration)
- `src/Infrastructure/Repositories/ActivityRepository.cs` — queries both tables, merges, sorts, trims to limit; maps `GenerationStatus` int to label
- `src/Api/Endpoints/ActivityEndpoints.cs` — `GET /api/Activity`, default limit 10, max 200, 400 on limit ≤ 0
- `tests/UnitTests/ActivityServiceTests.cs` — 3 unit tests
- `tests/IntegrationTests/FakeActivityRepository.cs`
- `tests/IntegrationTests/ActivityEndpointTests.cs` — 5 integration tests

**Modified files:**
- `src/Infrastructure/Data/AppDbContext.cs` — added `DbSet<ExportRequest>` and `DbSet<StratificationCache>` with entity configs
- `src/Api/Program.cs` — registered `IActivityRepository`/`IActivityService`, mapped `ActivityEndpoints`
- `specs/_index.md` — marked FEAT-035 Done
- `docs/api.md` — added `GET /api/Activity` entry
- `docs/changelog.md` — this entry

**Tests:** 3 unit + 5 integration (total: 280 unit + 97 integration, all passing)

---

## [FEAT-034] Dashboard Transaction Table — 2026-04-02

**Status:** ✅ Done

**Summary:** Wired the `TransactionTable.razor` dashboard component to live data by calling two stored procedures in CDE_Dashboard. Internal users see results grouped by CDC Program (`GetMessageTransactions`); External users see results grouped by Category (`GetMessageTransactionsByCategory`) with their jurisdiction automatically sourced from the JWT. The filter panel supports date range, message type, and jurisdiction (Internal) or category (External). Default date range is the last 3 days.

**New files:**
- `src/Core/DTOs/MessageTransactionDto.cs` — unified API response DTO (GroupName, GroupId, Processed, Errors, Received, NotProcessed)
- `src/Core/Interfaces/IMessageTransactionRepository.cs` — two SP method signatures
- `src/Core/Interfaces/IMessageTransactionService.cs` — service interface
- `src/Core/Services/MessageTransactionService.cs` — routes Internal→GetTransactions, External→GetTransactionsByCategory
- `src/Infrastructure/Models/SpMessageTransactionResult.cs` — SP result POCO for GetMessageTransactions
- `src/Infrastructure/Models/SpMessageTransactionCategoryResult.cs` — SP result POCO for GetMessageTransactionsByCategory
- `src/Infrastructure/Repositories/MessageTransactionRepository.cs` — `SqlQueryRaw` + `SqlParameter`; resolves `messageTypeId` → Name for SP param
- `specs/features/dashboard-transaction-table.md` — feature spec
- `tests/UnitTests/MessageTransactionServiceTests.cs` — 7 unit tests (SP routing, jurisdiction handling, messageTypeId passthrough)
- `tests/IntegrationTests/FakeMessageTransactionRepository.cs` — fake repository
- `tests/IntegrationTests/DashboardTransactionEndpointTests.cs` — 5 integration tests (401, Internal/External JWT, optional filters, empty result)

**Modified files:**
- `src/Api/Endpoints/DashboardEndpoints.cs` — added `GET /api/Dashboard/Transactions`
- `src/Api/Program.cs` — DI registrations for transaction repository + service
- `src/Portal/Services/Interfaces/IDashboardApiClient.cs` — added `GetTransactionsAsync`
- `src/Portal/Services/DashboardApiClient.cs` — implemented `GetTransactionsAsync`
- `src/Portal/Components/Pages/Dashboard/Components/TransactionTable.razor` — removed inline @code block
- `src/Portal/Components/Pages/Dashboard/Components/TransactionTable.razor.cs` — rewired to real API; dynamic grouping label; auth-state-driven SearchConfig; default date range last 3 days

---

## [FEAT-033] Message Volume Area Chart — 2026-04-01

**Status:** ✅ Done

**Summary:** Wired the `MessagesVolume.razor` dashboard component to live data by calling the `GetMessageVolume` stored procedure. Replaces hardcoded fake data with real message volume retrieved via `GET /api/Dashboard/Volumes`. Users can select time ranges (3 Days, 1 Week, 1 Month, 3 Months, 6 Months) via FluentMenu; the area chart updates with server-formatted labels.

**New files:**
- `src/Core/DTOs/MessageVolumeDto.cs` — `MessageVolumePointDto` and `MessageVolumeResultDto` records
- `src/Core/Interfaces/IMessageVolumeRepository.cs` — repository interface
- `src/Core/Interfaces/IMessageVolumeService.cs` — service interface
- `src/Core/Services/MessageVolumeService.cs` — repository delegation + server-side label formatting
- `src/Infrastructure/Models/SpMessageVolumeResult.cs` — SP result POCO (ReceivedDate, Volume)
- `src/Infrastructure/Repositories/MessageVolumeRepository.cs` — `SqlQueryRaw` with `SqlParameter`
- `tests/UnitTests/MessageVolumeServiceTests.cs` — 7 unit tests
- `tests/IntegrationTests/FakeMessageVolumeRepository.cs` — fake repository for integration tests
- `tests/IntegrationTests/DashboardVolumeEndpointTests.cs` — 4 integration tests

**Modified files:**
- `src/Api/Endpoints/DashboardEndpoints.cs` — added `GET /Volumes` endpoint
- `src/Api/Program.cs` — DI registrations for volume repository + service
- `src/Portal/Services/Interfaces/IDashboardApiClient.cs` — added `GetMessageVolumeAsync`
- `src/Portal/Services/DashboardApiClient.cs` — implemented `GetMessageVolumeAsync`
- `src/Portal/Components/Pages/Dashboard/Components/MessagesVolume.razor.cs` — replaced fake data with API call

**Tests:** 7 unit + 4 integration, all passing

---

## [FEAT-032] Dashboard 24-Hour Message Status Snapshot — 2026-04-01

**Status:** 🟡 Pending Review

**Summary:** Wired the `StatusSummary.razor` dashboard component to live data by calling the `GetMessageStatusesByAll` stored procedure. Displays Incoming, Completed, Errored, and In Process counts for the last 24 hours with percent change trends vs the prior 24-hour period. Two SP calls execute in parallel for current and prior periods.

**New files:**
- `src/Core/DTOs/MessageStatusSnapshotDto.cs` — `StatusMetricDto` and `MessageStatusSnapshotDto` records
- `src/Core/Interfaces/IMessageStatusRepository.cs` — repository interface
- `src/Core/Interfaces/IMessageStatusService.cs` — service interface
- `src/Core/Services/MessageStatusService.cs` — parallel SP calls, percent change, InProcess clamping
- `src/Infrastructure/Models/SpMessageStatusResult.cs` — SP result POCO
- `src/Infrastructure/Repositories/MessageStatusRepository.cs` — `SqlQueryRaw` with `SqlParameter`
- `src/Api/Endpoints/DashboardEndpoints.cs` — `GET /api/Dashboard/Status`
- `src/Portal/Services/Interfaces/IDashboardApiClient.cs` — Portal API client interface
- `src/Portal/Services/DashboardApiClient.cs` — Portal API client implementation

**Modified files:**
- `src/Api/Program.cs` — DI registrations + endpoint mapping
- `src/Portal/Program.cs` — `IDashboardApiClient` registration
- `src/Portal/Components/Pages/Dashboard/Components/StatusSummary.razor` — loading state
- `src/Portal/Components/Pages/Dashboard/Components/StatusSummary.razor.cs` — replaced hardcoded data with API call

**Tests:** 14 unit + 4 integration (total 236 unit + 77 integration all passing)

---

## [FEAT-030-v2] Edit User Permission Scoping v2 — Current Role Only — 2026-03-30

**Status:** ✅ Done

**Summary:** Changed Edit-User page permission scoping from union-of-all-roles to current-role-only (`IsCurrentRole = true`). Non-admin users can now only assign items within their current role's scope. Role assignment is based on the current role's `RoleGroup` (Program roles assign Program roles, Jurisdiction roles assign Jurisdiction roles). Admin bypass only applies when the current role is an admin role. Added `NavigationLock` to warn users about unsaved changes when navigating away or switching roles mid-edit. Updated `AdminUserPermissionsDto` to replace `RoleIds` with `CurrentRoleGroup`.

**Files modified:**
- `src/Core/DTOs/AdminUserPermissionsDto.cs` — replaced `RoleIds` with `CurrentRoleGroup`
- `src/Infrastructure/Repositories/AdminUserRepository.cs` — `GetPermissionsAsync` queries only `IsCurrentRole` UserRole
- `src/Api/Endpoints/AdminUserEndpoints.cs` — validation checks RoleGroup match; injects `IFilterRepository`
- `src/Portal/Components/Pages/Administration/UserManagement/Edit-User.razor` — added `NavigationLock`
- `src/Portal/Components/Pages/Administration/UserManagement/Edit-User.razor.cs` — `FilteredRoles` uses `CurrentRoleGroup`; added `OnBeforeInternalNavigation`, `SwitchRoleWithWarningAsync`, `HasUnsavedChanges`
- `tests/IntegrationTests/AdminUserCreateEndpointsTests.cs` — updated `FakeAdminUserRepository`

**Spec:** `specs/features/edit-user-permission-scoping-v2.md`

---

## [FEAT-030] Edit User Permission Scoping — 2026-03-30

**Status:** ✅ Done

**Summary:** Scoped the Edit-User page so non-admin users (Jurisdiction Data Manager, CDC Program Data Manager) can only assign roles, programs, jurisdictions, and event codes they themselves hold. Admin roles (MVPS Support Manager, MVPS User Support Manager) remain unrestricted. Permissions are derived from the union of all the logged-in user's roles. A new `GET /api/admin/users/my-permissions` endpoint returns the user's allowed IDs. Backend validation on `POST /api/admin/users/{userId}/role-assignments` returns 403 if a non-admin attempts to assign items outside their scope. EventCodeId=-1 (Check All) grants permission for all event codes under that program/jurisdiction.

**Files added:**
- `src/Core/DTOs/AdminUserPermissionsDto.cs`
- `specs/features/edit-user-permission-scoping.md`

**Files modified:**
- `src/Core/Interfaces/IAdminUserRepository.cs` — added `GetPermissionsAsync`
- `src/Infrastructure/Repositories/AdminUserRepository.cs` — implemented `GetPermissionsAsync`
- `src/Api/Endpoints/AdminUserEndpoints.cs` — added `HandleGetMyPermissions` endpoint, added `ValidateAssignmentsAgainstPermissions` backend validation
- `src/Portal/Services/Interfaces/IAdminUserApiClient.cs` — added `GetMyPermissionsAsync`
- `src/Portal/Services/AdminUserApiClient.cs` — implemented `GetMyPermissionsAsync`
- `src/Portal/Components/Pages/Administration/UserManagement/Edit-User.razor.cs` — fetches permissions, `FilteredRoles`/`FilteredPrograms`/`FilteredJurisdictions`/`FilterEventCodesByPermissions`
- `src/Portal/Components/Pages/Administration/UserManagement/Edit-User.razor` — uses filtered collections
- `tests/IntegrationTests/AdminUserCreateEndpointsTests.cs` — updated `FakeAdminUserRepository`
- `specs/_index.md`

---

## [FEAT-029] Add User & Edit User Role Assignments — 2026-03-30

**Status:** ✅ Done

**Summary:** Wired up the AddUserDialog "Continue" button to create a new User record via `POST /api/admin/users`, then navigate to the Edit-User page. Implemented the full 3-column role assignment UI on `/administration/user-management/edit-user/{userId}` with: Column 1 (Roles) filtered by UserType (Internal sees admin+program roles, External sees jurisdiction roles); Column 2 (Programs/Jurisdiction) dynamically driven by selected role's RoleGroup; Column 3 (Event Codes) with per-program filtering, Check All (stores EventCodeId=-1), and HasAllEvents auto-assignment. Save creates UserRole + UserRoleAssignment records with delete-then-insert transaction. Client-side validation enforces all business rules including single-jurisdiction constraint for external users.

**Files added:**
- `src/Core/DTOs/CreateUserRequest.cs`
- `src/Core/DTOs/CreateUserResponse.cs`
- `src/Core/DTOs/UserDetailDto.cs`
- `src/Core/DTOs/RoleDto.cs`
- `src/Core/DTOs/SaveRoleAssignmentsRequest.cs`
- `src/Core/Interfaces/IAdminUserRepository.cs`
- `src/Infrastructure/Repositories/AdminUserRepository.cs`
- `src/Portal/Components/Pages/Administration/UserManagement/Components/AddUserDialog.razor.cs`
- `tests/UnitTests/AdminUserApiClientTests.cs`
- `tests/IntegrationTests/AdminUserCreateEndpointsTests.cs`

**Files modified:**
- `src/Core/Interfaces/IFilterRepository.cs` — added `GetEventCodesByProgramAsync`, `GetActiveRoleDetailsAsync`
- `src/Infrastructure/Repositories/FilterRepository.cs` — implemented new methods
- `src/Api/Endpoints/AdminUserEndpoints.cs` — added POST create user, GET user by id, POST save role-assignments
- `src/Api/Endpoints/FilterEndpoints.cs` — added GET event codes by program, GET role details
- `src/Api/Program.cs` — registered `IAdminUserRepository`
- `src/Portal/Services/Interfaces/IAdminUserApiClient.cs` — added create, get, save methods
- `src/Portal/Services/AdminUserApiClient.cs` — implemented new methods
- `src/Portal/Services/Interfaces/IFilterApiClient.cs` — added event codes by program, role details
- `src/Portal/Services/FilterApiClient.cs` — implemented new methods
- `src/Portal/Components/Pages/Administration/UserManagement/Components/AddUserDialog.razor` — wired Continue to create user + navigate
- `src/Portal/Components/Pages/Administration/UserManagement/Users.razor` — passed UserType to AddUserDialog
- `src/Portal/Components/Pages/Administration/UserManagement/Edit-User.razor` — full 3-column role assignment UI
- `src/Portal/Components/Pages/Administration/UserManagement/Edit-User.razor.cs` — state management, validation, save logic
- `src/Portal/Components/Pages/Administration/UserManagement/Edit-User.razor.css` — styling for role assignment cards

**Tests:** 7 unit + 7 integration; total 219 unit + 73 integration all passing

---

## [FEAT-028] Search SAMS User Source — 2026-03-27

**Status:** ✅ Done

**Summary:** Added the ability for admin users to discover external SAMS users from the `CDE_REPORT` database when searching by AccountIdentifier on the Admin Users page. When the User table search returns zero results and an AccountIdentifier was provided, the system now searches both the internal person source (FEAT-027) and SAMS user source in parallel. The SAMS search executes stored procedure `uspExtGetUserActivity` (no parameters) and filters results in-memory by `UserAccountNbr` (as string) matching AccountIdentifier and `ActivityName` matching the `SamsActivityName` appsetting. If found, a message bar appears with an Add button that opens a confirmation modal displaying 6 fields: GivenName, SurName, Email, UserAccountNbr, ActivityName, and CurrentSamsStatus. Both person and SAMS user message bars can appear simultaneously.

**Files added:**
- `src/Core/DTOs/SamsUserDto.cs` — record DTO for SAMS user data (6 fields)
- `src/Core/Interfaces/ISamsUserRepository.cs` — repository interface
- `src/Infrastructure/Data/CdeReportDbContext.cs` — minimal DbContext for CDE_REPORT database
- `src/Infrastructure/Models/SpSamsUserResult.cs` — POCO for SqlQueryRaw mapping of SP results
- `src/Infrastructure/Repositories/SamsUserRepository.cs` — executes `uspExtGetUserActivity` SP, filters in-memory
- `src/Api/Endpoints/SamsUserEndpoints.cs` — `GET /api/SamsUsers` endpoint (AdminDataManager policy)
- `src/Portal/Services/Interfaces/ISamsUserApiClient.cs` — Portal API client interface
- `src/Portal/Services/SamsUserApiClient.cs` — Portal API client implementation
- `tests/UnitTests/SamsUserApiClientTests.cs` — 4 unit tests
- `tests/IntegrationTests/SamsUserEndpointsTests.cs` — 5 integration tests + FakeSamsUserRepository
- `specs/features/search-sams-user.md` — FEAT-028 spec

**Files modified:**
- `src/Api/Program.cs` — registered CdeReportDbContext, ISamsUserRepository, MapSamsUserEndpoints()
- `src/Api/appsettings.Development.json` — added CDE_REPORT connection string + SamsActivityName setting
- `src/Portal/Program.cs` — registered ISamsUserApiClient
- `src/Portal/Components/Pages/Administration/UserManagement/Users.razor` — added SAMS user message bar + modal (6 fields)
- `src/Portal/Components/Pages/Administration/UserManagement/Users.razor.cs` — injected ISamsUserApiClient, parallel search via Task.WhenAll
- `tests/IntegrationTests/ApiWebApplicationFactory.cs` — added CDE_REPORT connection string + SamsActivityName config

**Tests:** 4 unit + 5 integration; total 125 unit + 66 integration all passing

---

## [FEAT-027] Search Internal Person Source — 2026-03-27

**Status:** ✅ Done

**Summary:** Added the ability for admin users to discover active internal personnel from the `CDE_Person` database when searching by AccountIdentifier on the Admin Users page. When the User table search returns zero results and an AccountIdentifier was provided, the system automatically queries the `PERSON_VW` view for an active person (CdcStartDate ≤ today AND CDCSeparationDate > today). If found, a message bar appears with "A new User was found." and an Add button that opens a confirmation modal displaying the person's details. The actual user creation is a future feature.

**Files added:**
- `src/Core/DTOs/PersonDto.cs` — record DTO for person data
- `src/Core/Interfaces/IPersonRepository.cs` — repository interface
- `src/Infrastructure/Data/CdePersonDbContext.cs` — minimal DbContext for CDE_Person database
- `src/Infrastructure/Models/PersonViewResult.cs` — POCO for SqlQueryRaw mapping
- `src/Infrastructure/Repositories/PersonRepository.cs` — queries PERSON_VW with active-person filter
- `src/Api/Endpoints/PersonEndpoints.cs` — `GET /api/Persons` endpoint (AdminDataManager policy)
- `src/Portal/Services/Interfaces/IPersonApiClient.cs` — Portal API client interface
- `src/Portal/Services/PersonApiClient.cs` — Portal API client implementation
- `tests/UnitTests/PersonApiClientTests.cs` — 4 unit tests
- `tests/IntegrationTests/PersonEndpointsTests.cs` — 5 integration tests + FakePersonRepository
- `specs/features/search-internal-person.md` — FEAT-027 spec

**Files modified:**
- `src/Api/Program.cs` — registered CdePersonDbContext, IPersonRepository, MapPersonEndpoints()
- `src/Api/appsettings.Development.json` — added CDE_Person connection string
- `src/Portal/Program.cs` — registered IPersonApiClient
- `src/Portal/Components/Pages/Administration/UserManagement/Users.razor` — added message bar and confirmation modal
- `src/Portal/Components/Pages/Administration/UserManagement/Users.razor.cs` — added person search logic, IPersonApiClient injection
- `tests/IntegrationTests/ApiWebApplicationFactory.cs` — added CDE_Person connection string

**Tests:** 4 new unit tests, 5 new integration tests; 121 unit + 61 integration all passing.

---

## [FEAT-026] Admin User Setting Search Filter — 2026-03-26

**Status:** ✅ Done

**Summary:** Added a "Settings" search filter to the Admin User Management page (`/administration/usermanagement/users`). Administrators can now filter the user list by a specific active setting assignment. The filter matches users who have an active `UserSetting` record (`IsActive=true`) for the selected setting. Most UI plumbing already existed (`SearchConfig.IncludeSettings`, `SearchSelection.SettingId`, FilterPanel dropdown, lookup endpoint); this feature wires `SettingId` through the API and repository layers.

**Files modified:**
- `src/Core/Entities/User.cs` — added `UserSettings` navigation property
- `src/Core/Entities/UserSetting.cs` — added `User` navigation property
- `src/Infrastructure/Data/AppDbContext.cs` — configured User↔UserSetting relationship
- `src/Core/DTOs/AdminUserSearchParams.cs` — added `int? SettingId` parameter
- `src/Infrastructure/Repositories/AdminUserListRepository.cs` — added SettingId filter in `ApplySearchFilters`
- `src/Api/Endpoints/AdminUserEndpoints.cs` — added `settingId` query parameter
- `src/Portal/Services/AdminUserApiClient.cs` — added `settingId` to URL builder
- `src/Portal/Components/Pages/Administration/UserManagement/Users.razor.cs` — enabled `IncludeSettings`, mapped `SettingId`

**Files added:**
- `specs/features/admin-user-setting-search.md` — FEAT-026 spec

**Tests:** 1 new integration test; 117 unit + 56 integration all passing.

---

## [FEAT-025] User Notifications View — 2026-03-26

**Status:** ✅ Done

**Summary:** Replaced the ProfilePanel "Notifications coming soon" placeholder with a functional notifications tab. Displays eligible notifications as a flat list with title, description, frequency, and scheduled time. Users can toggle notifications on/off via `FluentSwitch`. Eligibility is filtered by active notifications linked to the user's active settings (or unlinked to any setting). Jurisdiction users face a `HasMinimumRecipient` constraint — toggle is disabled with a warning when they are the last active recipient in their jurisdiction. Program and admin users bypass this check.

**Files added:**
- `specs/features/user-notifications-view.md` — FEAT-025 spec
- `src/Core/Entities/Notification.cs` — entity (maps to existing `Notification` table)
- `src/Core/Entities/NotificationCategory.cs` — entity (maps to existing `NotificationCategory` table)
- `src/Core/Entities/UserNotification.cs` — entity (maps to existing `UserNotification` table)
- `src/Core/DTOs/UserNotificationDto.cs` — DTO record
- `src/Core/DTOs/ToggleNotificationRequest.cs` — request record
- `src/Core/Interfaces/IUserNotificationRepository.cs` — repository interface
- `src/Core/Interfaces/IUserNotificationService.cs` — service interface
- `src/Core/Services/UserNotificationService.cs` — service with validation
- `src/Infrastructure/Repositories/UserNotificationRepository.cs` — EF Core repository with HasMinimumRecipient jurisdiction check
- `tests/UnitTests/UserNotificationServiceTests.cs` — 7 unit tests
- `tests/IntegrationTests/UserNotificationEndpointTests.cs` — 5 integration tests
- `tests/IntegrationTests/FakeUserNotificationRepository.cs` — test fake

**Files modified:**
- `src/Infrastructure/Data/AppDbContext.cs` — added 3 DbSets + entity configs (Notification, NotificationCategory, UserNotification)
- `src/Api/Endpoints/AuthEndpoints.cs` — added `GET /auth/user-notifications`, `PUT /auth/user-notifications`
- `src/Api/Program.cs` — DI registrations for `IUserNotificationRepository`, `IUserNotificationService`
- `src/Portal/Services/IApiAuthClient.cs` — added `GetUserNotificationsAsync`, `ToggleNotificationAsync`
- `src/Portal/Services/ApiAuthClient.cs` — implementations
- `src/Portal/Components/Layout/Bars/Components/ProfilePanel.razor` — notifications tab UI
- `src/Portal/Components/Layout/Bars/Components/ProfilePanel.razor.cs` — notification loading, toggle handler, ViewModel
- `tests/IntegrationTests/ApiWebApplicationFactory.cs` — registered fake notification repository
- `docs/changelog.md` — this entry
- `docs/api.md` — GET/PUT `/auth/user-notifications`
- `specs/_index.md` — FEAT-025 row

---

## [FEAT-024] User Settings View — 2026-03-25

**Status:** 🔵 In Review

**Summary:** Populated the ProfilePanel "Settings" tab with the authenticated user's assigned settings. Read-only view displaying setting Name and Description, ordered alphabetically. Only shows settings where both `Setting.IsActive` and `UserSetting.IsActive` are `true`. Shows "No settings assigned." when no settings match.

**Files added:**
- `specs/features/user-settings-view.md` — FEAT-024 spec
- `src/Core/Entities/UserSetting.cs` — new entity (maps to existing `UserSetting` table)
- `src/Core/DTOs/UserSettingDto.cs` — DTO record
- `src/Core/Interfaces/IUserSettingRepository.cs` — repository interface
- `src/Core/Interfaces/IUserSettingService.cs` — service interface
- `src/Core/Services/UserSettingService.cs` — service implementation
- `src/Infrastructure/Repositories/UserSettingRepository.cs` — EF Core repository
- `tests/UnitTests/UserSettingServiceTests.cs` — 2 unit tests
- `tests/IntegrationTests/UserSettingEndpointTests.cs` — 2 integration tests
- `tests/IntegrationTests/FakeUserSettingRepository.cs` — test fake

**Files modified:**
- `src/Infrastructure/Data/AppDbContext.cs` — added `UserSettings` DbSet + entity config
- `src/Api/Endpoints/AuthEndpoints.cs` — added `GET /auth/user-settings`
- `src/Api/Program.cs` — DI registrations for `IUserSettingRepository`, `IUserSettingService`
- `src/Portal/Services/IApiAuthClient.cs` — added `GetUserSettingsAsync` method
- `src/Portal/Services/ApiAuthClient.cs` — implemented `GetUserSettingsAsync`
- `src/Portal/Components/Layout/Bars/Components/ProfilePanel.razor` — replaced Settings tab placeholder
- `src/Portal/Components/Layout/Bars/Components/ProfilePanel.razor.cs` — parallel settings load
- `tests/IntegrationTests/ApiWebApplicationFactory.cs` — registered `FakeUserSettingRepository`
- `docs/api.md`, `docs/changelog.md`

---

## Rename: `/api/Lookups` → `/api/filters` — 2026-03-25

**Summary:** Renamed the lookup endpoint group from `/api/Lookups` to `/api/filters` and renamed all associated classes/interfaces/files: `LookupEndpoints` → `FilterEndpoints`, `ILookupRepository` → `IFilterRepository`, `LookupRepository` → `FilterRepository`, `ILookupApiClient` → `IFilterApiClient`, `LookupApiClient` → `FilterApiClient`. All Portal API client URL strings updated accordingly. No functional changes.

**Files renamed:**
- `src/Api/Endpoints/LookupEndpoints.cs` → `FilterEndpoints.cs`
- `src/Core/Interfaces/ILookupRepository.cs` → `IFilterRepository.cs`
- `src/Infrastructure/Repositories/LookupRepository.cs` → `FilterRepository.cs`
- `src/Portal/Services/Interfaces/ILookupApiClient.cs` → `IFilterApiClient.cs`
- `src/Portal/Services/LookupApiClient.cs` → `FilterApiClient.cs`

**Files modified:**
- `src/Api/Program.cs`, `src/Portal/Program.cs` — DI registrations
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs`, `FilterMMWRDate.razor.cs` — injected client references
- `tests/IntegrationTests/AuthorizationPolicyTests.cs`, `tests/UnitTests/FilterPanelTests.cs` — updated URLs and mocks
- `docs/api.md`, `docs/changelog.md` — documentation

---

## [FEAT-023] Role-Based Authorization Policies — 2026-03-24

**Status:** 🔵 In Review

**Summary:** Added ASP.NET Core authorization policies using a 2-tier model. Default policy requires authentication + at least one `ClaimTypes.Role` claim for all protected endpoints. Named "AdminDataManager" policy restricts `/api/admin/*` to the 4 admin/data-manager roles (MVPS Support Manager, MVPS User Support Manager, Jurisdiction Data Manager, CDC Program Data Manager). Lookup endpoints (`/api/filters/*`) now require authentication (were previously open). `AdminUserEndpoints.DetermineScope()` retained for data-level scoping after the policy gate.

**Files added:**
- `specs/features/role-based-authorization.md` — FEAT-023 spec
- `tests/IntegrationTests/AuthorizationPolicyTests.cs` — 6 integration tests

**Files modified:**
- `src/Api/Program.cs` — replaced `AddAuthorization()` with `AddAuthorizationBuilder()` + default and "AdminDataManager" policies
- `src/Api/Endpoints/AdminUserEndpoints.cs` — `.RequireAuthorization("AdminDataManager")`
- `src/Api/Endpoints/FilterEndpoints.cs` (formerly `LookupEndpoints.cs`) — added `.RequireAuthorization()` to group

---

## [FEAT-022] Current Role Access View — 2026-03-24

**Status:** 🔵 In Review

**Summary:** Replaced the ProfilePanel "Access & Permissions" tab's all-roles data grid with a focused current-role-only view. Shows the role name and description, then conditionally displays access scope based on `RoleGroup`: admin roles (`RoleGroup=null`) show "All Programs", "All Jurisdictions", and "All Event Codes"; program-scoped roles show FluentBadge chips for each assigned program; jurisdiction-scoped roles show the jurisdiction name. Event codes display for all role types: admin roles always show "All Event Codes", `EventCodeId=-1` shows "All Event Codes", otherwise FluentBadge chips in `"Description (Code)"` format. New `GET /auth/current-role-access` endpoint returns `CurrentRoleAccessDto` for the user's current role. Existing `GET /auth/role-assignments` endpoint is unchanged.

**Files added:**
- `specs/features/current-role-access-view.md` — FEAT-022 spec
- `src/Core/DTOs/CurrentRoleAccessDto.cs` — DTO with role name, description, RoleGroup, programs list, jurisdiction
- `src/Core/Interfaces/ICurrentRoleAccessRepository.cs` — repository interface
- `src/Core/Interfaces/ICurrentRoleAccessService.cs` — service interface
- `src/Core/Services/CurrentRoleAccessService.cs` — thin pass-through service with logging
- `src/Infrastructure/Repositories/CurrentRoleAccessRepository.cs` — EF query filtering to `IsCurrentRole=true`
- `tests/UnitTests/CurrentRoleAccessServiceTests.cs` — 7 unit tests
- `tests/IntegrationTests/CurrentRoleAccessEndpointTests.cs` — 5 integration tests
- `tests/IntegrationTests/FakeCurrentRoleAccessRepository.cs` — in-memory fake for integration tests

**Files modified:**
- `src/Api/Endpoints/AuthEndpoints.cs` — added `GET /auth/current-role-access` endpoint
- `src/Api/Program.cs` — DI registration for `ICurrentRoleAccessRepository` + `ICurrentRoleAccessService`
- `src/Portal/Services/IApiAuthClient.cs` — added `GetCurrentRoleAccessAsync`
- `src/Portal/Services/ApiAuthClient.cs` — implemented `GetCurrentRoleAccessAsync`
- `src/Portal/Components/Layout/Bars/Components/ProfilePanel.razor` — replaced FluentDataGrid with conditional RoleGroup display
- `src/Portal/Components/Layout/Bars/Components/ProfilePanel.razor.cs` — fetches `CurrentRoleAccessDto`
- `tests/IntegrationTests/ApiWebApplicationFactory.cs` — registered `FakeCurrentRoleAccessRepository`
- `docs/api.md` — added `GET /auth/current-role-access` documentation

---

## [FEAT-020] User Profile View — 2026-03-22

**Status:** 🔵 In Review

**Summary:** Wired the TopBar `UserProfile.razor` component to real JWT claims from the `auth_token` cookie. Users see their actual name (`FirstName, LastName (AccountIdentifier)`), email address, initials, and roles. Role switcher uses `FluentMenuButton` to switch the active role via `POST /auth/switch-role`. FooterTemplate has an "Access & Permissions" link that closes the profile menu and opens a slide-in `IDialogService` panel with `FluentTabs` (Access & Permissions, Settings, Notifications). Access & Permissions tab fetches `UserRoleAssignment` data from a new `GET /auth/role-assignments` endpoint. Settings and Notifications tabs are placeholders. Profile menu uses `@bind-Open` for programmatic close.

**Files added:**
- `specs/features/user-profile-view.md` — FEAT-020 spec
- `src/Core/DTOs/SwitchRoleRequest.cs` — request record for role switching
- `src/Core/DTOs/UserRoleAssignmentDto.cs` — DTO for role assignment data
- `src/Core/Interfaces/IRoleSwitchService.cs` — role switch service interface
- `src/Core/Interfaces/IUserRoleAssignmentRepository.cs` — role assignment query interface
- `src/Core/Services/RoleSwitchService.cs` — orchestrates role switch + token regeneration
- `src/Infrastructure/Repositories/UserRoleAssignmentRepository.cs` — queries UserRoles with assignments
- `src/Portal/Components/Layout/Bars/Components/UserProfile.razor.cs` — code-behind with JWT claim parsing, role switching, dialog service
- `src/Portal/Components/Layout/Bars/Components/ProfilePanel.razor` + `.razor.cs` — slide-in panel with FluentTabs (Access & Permissions, Settings, Notifications)
- `tests/UnitTests/RoleSwitchServiceTests.cs` — 7 unit tests
- `tests/IntegrationTests/SwitchRoleEndpointTests.cs` — 5 integration tests
- `tests/IntegrationTests/RoleAssignmentsEndpointTests.cs` — 3 integration tests
- `tests/IntegrationTests/FakeUserRoleAssignmentRepository.cs` — test fake

**Files modified:**
- `src/Core/Interfaces/IUserRepository.cs` — added `SetCurrentRoleAsync`
- `src/Infrastructure/Repositories/UserRepository.cs` — implemented `SetCurrentRoleAsync`
- `src/Api/Endpoints/AuthEndpoints.cs` — added `POST /auth/switch-role` and `GET /auth/role-assignments`
- `src/Api/Program.cs` — registered `IRoleSwitchService`, `IUserRoleAssignmentRepository`
- `src/Portal/Services/IApiAuthClient.cs` — added `SwitchRoleAsync`, `GetRoleAssignmentsAsync`
- `src/Portal/Services/ApiAuthClient.cs` — implemented both methods
- `src/Portal/Components/Layout/Bars/Components/UserProfile.razor` — replaced hardcoded placeholder with JWT-driven profile menu
- `tests/IntegrationTests/FakeUserRepository.cs` — added `SetCurrentRoleAsync`
- `tests/IntegrationTests/ApiWebApplicationFactory.cs` — added `CDE_ENV` connection string, `ConfigureTestServices` for JWT validation, `FakeUserRoleAssignmentRepository` registration

**Tests:** 86 unit + 23 integration, all passing (no regressions)

**Tests:** 86 unit + 20 integration, all passing (no regressions)

---

## [FEAT-019] Date Range Filter — 2026-03-21

**Status:** 🔵 In Review

**Summary:** Added Start Date and End Date filter controls to the Messages page FilterPanel using `FluentDatePicker`. Dates default to 3 days ago (Start) and today (End). Validation enforced via picker Min/Max constraints and explicit handler checks — Start Date cannot be after End Date and vice versa, with a `FluentMessageBar` error message shown to the user on invalid selection. Both dates are bounded between 1/1/2018 and today. Reset restores defaults.

**Files modified:**
- `src/Portal/Components/UI/SearchControls/Models/SearchSelection.cs` — added `DateTime? StartDate`, `DateTime? EndDate`
- `src/Portal/Components/UI/SearchControls/Models/SearchConfig.cs` — added `bool IncludeDateRange = true`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs` — `_startDate`/`_endDate` fields, `StartDateMax`/`EndDateMin` computed properties, `OnStartDateChanged`/`OnEndDateChanged` handlers with cross-field validation, `ApplyDateRangeDefaults()`, `_dateValidationMessage`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor` — two `FluentDatePicker` controls with Min/Max constraints, `FluentMessageBar` for validation errors

**Files added:**
- `specs/features/date-range-filter.md` — FEAT-019 spec

**Tests:** All 95 existing tests pass (no regressions)

---

## [FEAT-018] MMWR Year/Week Search Filter — 2026-03-21

**Status:** 🔵 In Review

**Summary:** Added MMWR year and week range filtering to the Messages page FilterPanel. Data is sourced from `[netss].[MMWRWeekLookup]` in a second database (`CDE_ENV`), accessed via a new `CdeEnvDbContext`. A new `GET /api/Lookups/Mmwrs` endpoint returns MMWR years (2018–current) with start/end week ranges. Three `FluentSelect` controls (no "All" option) default to the current year with StartWeek=1 and EndWeek=max. When the year changes, weeks auto-populate. A guard flag prevents `FluentSelect` re-render from overwriting the EndWeek default.

**Files added:**
- `src/Core/DTOs/MmwrYearDto.cs` — `record MmwrYearDto(int Year, int StartWeek, int EndWeek)`
- `src/Core/Interfaces/IMmwrRepository.cs` — repository interface
- `src/Infrastructure/Data/CdeEnvDbContext.cs` — minimal DbContext for CDE_ENV (no DbSets, SqlQueryRaw only)
- `src/Infrastructure/Models/SpMmwrYearResult.cs` — SQL result model matching column names
- `src/Infrastructure/Repositories/MmwrRepository.cs` — SqlQueryRaw implementation
- `specs/features/mmwr-year-week-filter.md` — FEAT-018 spec

**Files modified:**
- `src/Api/appsettings.Development.json` — added `CDE_ENV` connection string
- `src/Api/Program.cs` — registered `CdeEnvDbContext` + `IMmwrRepository`
- `src/Api/Endpoints/LookupEndpoints.cs` — added `GET /Mmwrs` endpoint (injects `IMmwrRepository`)
- `src/Portal/Components/UI/SearchControls/Models/SearchSelection.cs` — added `MmwrYear`, `MmwrStartWeek`, `MmwrEndWeek`
- `src/Portal/Components/UI/SearchControls/Models/SearchConfig.cs` — added `IncludeMmwr`
- `src/Portal/Components/UI/SearchControls/Models/SearchValues.cs` — added `MmwrYears`
- `src/Portal/Services/Interfaces/ILookupApiClient.cs` — added `GetMmwrYearsAsync`
- `src/Portal/Services/LookupApiClient.cs` — implemented `GetMmwrYearsAsync`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs` — MMWR fields, handlers, lazy-load, `MmwrWeekOptions`, `ApplyMmwrDefaults`, `_isMmwrYearChanging` guard
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor` — 3 inline `FluentSelect` MMWR controls

**Tests:** All 95 existing tests pass (no regressions)

---

## [REFACTOR] Extract FilterField Component — 2026-03-20

**Status:** ✅ Done

**Summary:** Extracted a reusable `FilterField.razor` component from `FilterPanel.razor` to eliminate
~170 lines of repeated dropdown markup. Each of the 11 identical dropdown blocks (label + FluentSelect +
foreach over items) was replaced with a single `<FilterField>` tag. The MessageSet dropdown (hardcoded
boolean options, no item list) remains inline. Debug `<span>` elements were also removed.

**Files added:**
- `src/Portal/Components/UI/SearchControls/FilterField.razor` — reusable dropdown markup
- `src/Portal/Components/UI/SearchControls/FilterField.razor.cs` — code-behind with `Items`/`StringItems`/`Value`/`ValueChanged` parameters

**Files modified:**
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor` — replaced 11 dropdown blocks with `<FilterField>` usages, removed 13 debug spans
- `docs/changelog.md` — this entry

**Tests:** No test changes needed — all existing tests pass

---

## [REFACTOR] Unified Lookup DTOs — 2026-03-20

**Status:** ✅ Done

**Summary:** Consolidated 11 individual lookup DTO records into two unified types:
`LookupItem(int Value, string Text)` for integer-keyed lookups (10 endpoints) and
`LookupStringItem(string Value, string Text)` for string-keyed lookups (MessageStatuses).
All API response JSON properties changed from varied names to `{ "value": ..., "text": "..." }`.

**Files added:**
- `src/Core/DTOs/LookupItem.cs`
- `src/Core/DTOs/LookupStringItem.cs`

**Files deleted:**
- `src/Core/DTOs/ProgramListItemDto.cs`
- `src/Core/DTOs/MessageTypeListItemDto.cs`
- `src/Core/DTOs/MessageStatusListItemDto.cs`
- `src/Core/DTOs/CategoryListItemDto.cs`
- `src/Core/DTOs/JurisdictionListItemDto.cs`
- `src/Core/DTOs/CdcEventCodeListItemDto.cs`
- `src/Core/DTOs/WorkflowStatusListItemDto.cs`
- `src/Core/DTOs/ClassificationStatusListItemDto.cs`
- `src/Core/DTOs/ProcessingStatusListItemDto.cs`
- `src/Core/DTOs/BatchFileStatusListItemDto.cs`
- `src/Core/DTOs/CdcProfileListItemDto.cs`

**Files modified:**
- `src/Core/Interfaces/ILookupRepository.cs`
- `src/Infrastructure/Repositories/LookupRepository.cs`
- `src/Portal/Services/Interfaces/ILookupApiClient.cs`
- `src/Portal/Services/LookupApiClient.cs`
- `src/Portal/Components/UI/SearchControls/Models/SearchValues.cs`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs`
- `tests/UnitTests/FilterPanelTests.cs`

**Tests:** 79 unit + 15 integration — all passing

---

## [FEAT-017] CdcProfile Filter — 2026-03-20

**Status:** 🔵 In Review

**Summary:** Added a CdcProfile dropdown filter to the FilterPanel. A new read-only entity maps to the existing `[CdcProfile]` table (no migration). A new `GET /api/Lookups/CdcProfiles` endpoint returns all profiles ordered by `DisplayName` alphabetically. No IsActive filtering — all profiles are returned. The dropdown defaults to "All" (null) and resets correctly. Filter enabled on all pages that use FilterPanel.

**Files added:**
- `src/Core/Entities/CdcProfile.cs` — entity mapping to `[CdcProfile]`
- `src/Core/DTOs/CdcProfileListItemDto.cs` — `record CdcProfileListItemDto(int ProfileId, string DisplayName)`
- `specs/features/cdcprofile-filter.md` — FEAT-017 spec

**Files modified:**
- `src/Core/Interfaces/ILookupRepository.cs` — added `GetCdcProfilesAsync`
- `src/Infrastructure/Repositories/LookupRepository.cs` — EF LINQ query with DisplayName ordering (no IsActive filter)
- `src/Infrastructure/Data/AppDbContext.cs` — new DbSet + fluent config
- `src/Api/Endpoints/LookupEndpoints.cs` — added `GET /CdcProfiles`
- `src/Portal/Services/Interfaces/ILookupApiClient.cs` — added `GetCdcProfilesAsync`
- `src/Portal/Services/LookupApiClient.cs` — implemented with error swallowing
- `src/Portal/Components/UI/SearchControls/Models/SearchConfig.cs` — added `IncludeCdcProfiles`
- `src/Portal/Components/UI/SearchControls/Models/SearchSelection.cs` — added `CdcProfileId`
- `src/Portal/Components/UI/SearchControls/Models/SearchValues.cs` — added `CdcProfiles`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor` — profile dropdown + debug span
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs` — load, sync, handler for CdcProfile
- `tests/UnitTests/FilterPanelTests.cs` — 5 new tests + harness updated

**Tests:** 79 unit + 15 integration + 1 E2E — all passing

---

## [FEAT-016] BatchFileStatus Filter — 2026-03-20

**Status:** ✅ Done

**Summary:** Added a BatchFileStatus dropdown filter to the FilterPanel. A new read-only entity maps to the existing `[BatchFileStatus]` table (no migration). A new `GET /api/Lookups/BatchFileStatuses` endpoint returns active statuses ordered by `DisplayName` alphabetically. The dropdown defaults to "All" (null) and resets correctly.

**Files added:**
- `src/Core/Entities/BatchFileStatus.cs` — entity mapping to `[BatchFileStatus]`
- `src/Core/DTOs/BatchFileStatusListItemDto.cs` — `record BatchFileStatusListItemDto(int Id, string DisplayName)`
- `specs/features/batch-file-status-filter.md` — FEAT-016 spec

**Files modified:**
- `src/Core/Interfaces/ILookupRepository.cs` — added `GetActiveBatchFileStatusesAsync`
- `src/Infrastructure/Repositories/LookupRepository.cs` — EF LINQ query with IsActive filter and DisplayName ordering
- `src/Infrastructure/Data/AppDbContext.cs` — new DbSet + fluent config
- `src/Api/Endpoints/LookupEndpoints.cs` — added `GET /BatchFileStatuses`
- `src/Portal/Services/Interfaces/ILookupApiClient.cs` — added `GetBatchFileStatusesAsync`
- `src/Portal/Services/LookupApiClient.cs` — implemented with error swallowing
- `src/Portal/Components/UI/SearchControls/Models/SearchConfig.cs` — added `IncludeBatchFileStatuses`
- `src/Portal/Components/UI/SearchControls/Models/SearchSelection.cs` — added `BatchFileStatusId`
- `src/Portal/Components/UI/SearchControls/Models/SearchValues.cs` — added `BatchFileStatuses`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor` — batch file status dropdown + debug span
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs` — load, change handler, sync logic
- `tests/UnitTests/FilterPanelTests.cs` — 5 new tests (default null, sync, reset, config disabled, config enabled)

**Tests:** 5 unit tests added; total 74 unit + 15 integration all passing

---

## [FEAT-015] ProcessingStatus Filter — 2026-03-20

**Status:** ✅ Done

**Summary:** Added a ProcessingStatus dropdown filter to the FilterPanel. A new read-only entity maps to the existing `[ProcessingStatus]` table (no migration). A new `GET /api/Lookups/ProcessingStatuses` endpoint returns active statuses ordered by `Name` alphabetically. The dropdown defaults to "All" (null) and resets correctly.

**Files added:**
- `src/Core/Entities/ProcessingStatus.cs` — entity mapping to `[ProcessingStatus]`
- `src/Core/DTOs/ProcessingStatusListItemDto.cs` — `record ProcessingStatusListItemDto(int Id, string Name)`
- `specs/features/processing-status-filter.md` — FEAT-015 spec

**Files modified:**
- `src/Core/Interfaces/ILookupRepository.cs` — added `GetActiveProcessingStatusesAsync`
- `src/Infrastructure/Repositories/LookupRepository.cs` — EF LINQ query with IsActive filter and Name ordering
- `src/Infrastructure/Data/AppDbContext.cs` — new DbSet + fluent config
- `src/Api/Endpoints/LookupEndpoints.cs` — added `GET /ProcessingStatuses`
- `src/Portal/Services/Interfaces/ILookupApiClient.cs` — added `GetProcessingStatusesAsync`
- `src/Portal/Services/LookupApiClient.cs` — implemented with error swallowing
- `src/Portal/Components/UI/SearchControls/Models/SearchConfig.cs` — added `IncludeProcessingStatuses`
- `src/Portal/Components/UI/SearchControls/Models/SearchSelection.cs` — added `ProcessingStatusId`
- `src/Portal/Components/UI/SearchControls/Models/SearchValues.cs` — added `ProcessingStatuses`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor` — processing status dropdown + debug span
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs` — backing field, handler, load/sync/reset
- `tests/UnitTests/FilterPanelTests.cs` — 5 new tests + updated harness
- `specs/_index.md` — FEAT-015 marked Done
- `docs/api.md` — new endpoint documented
- `docs/changelog.md` — this entry

---

## [FEAT-014] ClassificationStatus Filter — 2026-03-20

**Status:** ✅ Done

**Summary:** Added a ClassificationStatus dropdown filter to the FilterPanel. A new read-only entity maps to the existing `[ClassificationStatus]` table (no migration). A new `GET /api/Lookups/ClassificationStatuses` endpoint returns active statuses ordered by `DisplaySequence` descending. The dropdown defaults to "All" (null) and resets correctly.

**Files added:**
- `src/Core/Entities/ClassificationStatus.cs` — entity mapping to `[ClassificationStatus]`
- `src/Core/DTOs/ClassificationStatusListItemDto.cs` — `record ClassificationStatusListItemDto(int Id, string Name)`
- `specs/features/classification-status-filter.md` — FEAT-014 spec

**Files modified:**
- `src/Core/Interfaces/ILookupRepository.cs` — added `GetActiveClassificationStatusesAsync`
- `src/Infrastructure/Repositories/LookupRepository.cs` — EF LINQ query with IsActive filter and DisplaySequence descending
- `src/Infrastructure/Data/AppDbContext.cs` — new DbSet + fluent config
- `src/Api/Endpoints/LookupEndpoints.cs` — added `GET /ClassificationStatuses`
- `src/Portal/Services/Interfaces/ILookupApiClient.cs` — added `GetClassificationStatusesAsync`
- `src/Portal/Services/LookupApiClient.cs` — implemented with error swallowing
- `src/Portal/Components/UI/SearchControls/Models/SearchConfig.cs` — added `IncludeClassificationStatuses`
- `src/Portal/Components/UI/SearchControls/Models/SearchSelection.cs` — added `ClassificationStatusId`
- `src/Portal/Components/UI/SearchControls/Models/SearchValues.cs` — added `ClassificationStatuses`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor` — classification status dropdown + debug span
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs` — backing field, handler, load/sync/reset
- `tests/UnitTests/FilterPanelTests.cs` — 5 new tests + updated harness
- `specs/_index.md` — FEAT-014 marked Done
- `docs/api.md` — new endpoint documented
- `docs/changelog.md` — this entry

---

## [FEAT-013] Workflow Status Filter — 2026-03-20

**Status:** ✅ Done

**Summary:** Added workflow status filtering to the FilterPanel. Three read-only entities (`Workflow`, `Status`, `WorkflowStatus`) map to existing database tables (no migration). A new `GET /api/Lookups/Workflows/{id}/Statuses` endpoint returns active statuses for a workflow, ordered by `SortOrder`. Two independent dropdowns — "Jurisdiction Workflow Status" and "Program Workflow Status" — are populated from a single API call. Both default to "All" (null) and reset independently.

**Files added:**
- `src/Core/Entities/Workflow.cs` — entity mapping to `[Workflow]`
- `src/Core/Entities/Status.cs` — entity mapping to `[Status]`
- `src/Core/Entities/WorkflowStatus.cs` — entity mapping to `[WorkflowStatus]` with nav props
- `src/Core/DTOs/WorkflowStatusListItemDto.cs` — `record WorkflowStatusListItemDto(int Id, string Name)`

**Files modified:**
- `src/Core/Interfaces/ILookupRepository.cs` — added `GetWorkflowStatusesAsync(int workflowId, ...)`
- `src/Infrastructure/Repositories/LookupRepository.cs` — EF LINQ join query with IsActive filters and SortOrder ordering
- `src/Infrastructure/Data/AppDbContext.cs` — 3 new DbSets + fluent config (table names, keys, FKs)
- `src/Api/Endpoints/LookupEndpoints.cs` — added `GET /Workflows/{id:int}/Statuses`
- `src/Portal/Services/Interfaces/ILookupApiClient.cs` — added `GetWorkflowStatusesAsync(int workflowId)`
- `src/Portal/Services/LookupApiClient.cs` — implemented with error swallowing
- `src/Portal/Components/UI/SearchControls/Models/SearchConfig.cs` — added `IncludeWorkflowStatuses`, `WorkflowId`
- `src/Portal/Components/UI/SearchControls/Models/SearchSelection.cs` — added `JurisdictionWorkflowStatus`, `ProgramWorkflowStatus`
- `src/Portal/Components/UI/SearchControls/Models/SearchValues.cs` — added `WorkflowStatuses`
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor` — two workflow status dropdowns + debug spans
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs` — backing fields, handlers, load/sync/reset
- `tests/UnitTests/FilterPanelTests.cs` — 7 new tests + updated harness
- `specs/_index.md` — FEAT-013 marked Done
- `docs/api.md` — new endpoint documented
- `docs/changelog.md` — this entry

---

## [FILTER] Undetermined Program Option for Admin Roles — 2026-03-19

**Status:** ✅ Done

**Summary:** Admin users (identified by absence of `role_group` claim in JWT, meaning `RoleGroup` is null on the Role entity) now see an "Undetermined" option (ProgramId = -1, ProgramName = "Undetermined") prepended to the Programs dropdown in the filter panel. This is a synthetic UI-only entry — not stored in the database or fetched from the API. Non-admin users (including jurisdiction roles and other role groups) do not see this option.

**Files modified:**
- `src/Portal/Components/UI/SearchControls/FilterPanel.razor.cs` — added `_isAdminRole` field; detect admin via absent `role_group` claim; prepend "Undetermined" entry in `LoadProgramsAsync` when admin
- `docs/changelog.md` — this entry

**Files added:**
- `tests/UnitTests/FilterPanelTests.cs` — 9 unit tests covering admin/non-admin/jurisdiction role detection and Undetermined program prepend logic

---

## [FEAT-011] Windows Authentication Support — 2026-03-18

**Status:** ✅ Done

**Summary:** Fixed Windows (Negotiate) authentication so it works when running via Aspire AppHost. Three root causes: (1) `UseStatusCodePagesWithReExecute` was swallowing the 401 Negotiate challenge — an inline middleware now disables `IStatusCodePagesFeature` for 401 responses so the `WWW-Authenticate: Negotiate` header reaches the browser; (2) the JWT middleware was positioned after `UseAuthorization`, so it couldn't hydrate `context.User` from the cookie before auth checks — moved it before `UseAuthorization` and added `RequireAuthorization()` to Blazor routes so the framework handles the Negotiate challenge with proper HTTP/2 downgrade; (3) `ApiClient` read the cookie as `"AuthToken"` instead of `"auth_token"` — fixed the name mismatch. Also added `CustomAuthenticationStateProvider` so Blazor components can access auth state, and the middleware now strips the `DOMAIN\` prefix from Windows identity names.

**Files added:**
- `src/Portal/Services/CustomAuthenticationStateProvider.cs` — Blazor `AuthenticationStateProvider` that reads JWT claims from the `auth_token` cookie
- `specs/features/windows-authentication.md` — FEAT-011 spec

**Files modified:**
- `src/Portal/Middleware/JwtAuthenticationMiddleware.cs` — hydrates `context.User` from JWT cookie; strips `DOMAIN\` prefix from Windows identity; defers to authorization pipeline for Negotiate challenge
- `src/Portal/Program.cs` — moved middleware before `UseAuthorization()`; added inline middleware to bypass status code pages for 401s; registered `CustomAuthenticationStateProvider` + `AddCascadingAuthenticationState()`; added `RequireAuthorization()` to `MapRazorComponents`
- `src/Portal/Services/ApiClient.cs` — fixed cookie name from `"AuthToken"` to `"auth_token"`
- `src/Portal/Components/_Imports.razor` — added `@using Microsoft.AspNetCore.Components.Authorization`
- `src/Portal/Components/Pages/AccessDenied.razor` — added `[AllowAnonymous]`
- `src/Portal/Components/Pages/LoggedOut.razor` — added `[AllowAnonymous]`
- `src/Portal/Components/Pages/NotFound.razor` — added `[AllowAnonymous]`
- `src/Portal/Components/Pages/Error.razor` — added `[AllowAnonymous]`
- `specs/_index.md` — FEAT-011 added
- `docs/changelog.md` — this entry

---

## [REFACTOR] `IApiAuthClient` — Use `IApiClient` + `ApiResponse<T>` — 2026-03-14

**Status:** ✅ Done

**Summary:** `ApiAuthClient` now delegates HTTP transport to `IApiClient` instead of holding a raw `HttpClient`. `GetTokenAsync` returns `ApiResponse<string>` instead of `string?`, giving callers structured success/error information. `JwtAuthenticationMiddleware` updated to consume the new return type. `IApiClient` / `ApiClient` are now the single HTTP abstraction for all Portal → API communication.

**Files added:**
- `tests/UnitTests/ApiAuthClientTests.cs` — 5 unit tests (valid token, 401, 500, exception, missing token in body)

**Files modified:**
- `src/Portal/Services/IApiAuthClient.cs` — `GetTokenAsync` return type changed from `Task<string?>` to `Task<ApiResponse<string>>`
- `src/Portal/Services/ApiAuthClient.cs` — injects `IApiClient` instead of `HttpClient`; returns `ApiResponse<string>` for all paths
- `src/Portal/Middleware/JwtAuthenticationMiddleware.cs` — checks `result.Success` and reads `result.Data` for the token string
- `src/Portal/Program.cs` — replaced `AddHttpClient<IApiAuthClient, ApiAuthClient>` with `AddHttpContextAccessor()`, `AddHttpClient<IApiClient, ApiClient>`, and `AddTransient<IApiAuthClient, ApiAuthClient>`
- `tests/UnitTests/UnitTests.csproj` — added `<ProjectReference>` to `Portal.csproj`
- `docs/changelog.md` — this entry

---

## [FEAT-005] Token Session Management — 2026-03-05

**Status:** ✅ Done

**Summary:** The `auth_token` JWT cookie is now a session cookie (no `Max-Age`), so the browser clears it automatically when all windows/tabs close, ensuring a fresh JWT is generated on each new app session. A `GET /auth/logout` endpoint deletes the cookie and redirects to a new `/logged-out` page. A Logout link is added to the nav menu.

**Files added:**
- `src/Portal/Endpoints/LogoutEndpoints.cs` — `GET /auth/logout` minimal API endpoint
- `src/Portal/Pages/LoggedOut.razor` — `/logged-out` confirmation page
- `specs/features/token-session-management.md` — FEAT-005 spec

**Files modified:**
- `src/Portal/Middleware/JwtAuthenticationMiddleware.cs` — removed `MaxAge`; added `/logged-out` to middleware skip list
- `src/Api/Endpoints/AuthEndpoints.cs` — removed `MaxAge` from cookie options
- `src/Portal/Program.cs` — added `using Portal.Endpoints`; registered `app.MapLogoutEndpoints()`
- `src/Portal/Components/Layout/NavMenu.razor` — added Logout nav link
- `docs/changelog.md` — this entry
- `specs/_index.md` — FEAT-005 marked Done

---

## [FEAT-004] Internal Admin Role Bypass — 2026-03-04

**Status:** ✅ Done

**Summary:** Internal users whose current role has `IsAdminRole = true` AND `RoleGroup = null` now bypass the program assignment requirement and receive a valid JWT with no `program_id` / `program_name` claims. Non-admin internal users are unchanged. External users are unaffected. The `Role` entity gained a nullable `RoleGroup` property mapping to an existing database column.

**Files added:**
- `specs/features/admin-role-bypass.md` — FEAT-004 spec

**Files modified:**
- `src/Core/Entities/Role.cs` — added `string? RoleGroup`
- `src/Infrastructure/Data/AppDbContext.cs` — added `HasMaxLength(20)` config for `RoleGroup`
- `src/Core/Services/AuthService.cs` — admin bypass check before program resolution
- `tests/UnitTests/AuthServiceTests.cs` — 3 new tests
- `tests/IntegrationTests/AuthEndpointsTests.cs` — 2 new tests
- `docs/changelog.md` — this entry
- `docs/api.md` — noted admin bypass behavior
- `specs/_index.md` — FEAT-004 marked Done

---

## [FEAT-003] Internal User Program Assignment — 2026-03-04

**Status:** ✅ Done

**Summary:** Internal users now have their CDC program assignments embedded in the JWT at authentication time. The system resolves all active programs (`IsActive = true`) from `UserRoleAssignment → DimPrograms` across `IsCurrentRole = true` roles, de-duplicates by `ProgramId`, and embeds one `program_id` + `program_name` claim pair per program. If no active programs are found, authentication fails with 401. External users are unaffected.

**Files added:**
- `src/Core/Entities/CdcProgram.cs` — entity mapping to `DimPrograms`
- `specs/features/internal-user-program-assignment.md` — FEAT-003 spec

**Files modified:**
- `src/Core/Entities/UserRoleAssignment.cs` — added `CdcProgram?` navigation property
- `src/Infrastructure/Data/AppDbContext.cs` — added `DbSet<CdcProgram>`, FK config from `UserRoleAssignment` to `CdcProgram`, and `DimPrograms` table mapping
- `src/Infrastructure/Repositories/UserRepository.cs` — extended eager loading to include `UserRoleAssignments.ThenInclude(CdcProgram)`
- `src/Core/Services/UserTokenData.cs` — added optional `Programs` field
- `src/Core/Services/AuthResult.cs` — added `ProgramNotAssigned` to `AuthFailureReason` enum
- `src/Core/Services/AuthService.cs` — program resolution logic for internal users (with de-duplication)
- `src/Api/Services/TokenService.cs` — added `program_id` and `program_name` claims when present
- `tests/UnitTests/AuthServiceTests.cs` — 5 new tests + updated internal user helpers
- `tests/UnitTests/TokenServiceTests.cs` — 2 new tests for program claim presence/absence
- `docs/changelog.md` — this entry
- `docs/api.md` — updated JWT claims table
- `specs/_index.md` — FEAT-003 marked Done

---

## [FEAT-002] User Jurisdiction Assignment — 2026-03-04

**Status:** ✅ Done

**Summary:** External users now have their jurisdiction embedded in the JWT at authentication time. The system resolves the jurisdiction from `UserRoleAssignment → DimJurisdictions` and adds `jurisdiction_id` and `jurisdiction_description` claims. If an external user has no jurisdiction assigned, authentication fails with 401. Internal users are unaffected.

**Files added:**
- `src/Core/Entities/Jurisdiction.cs` — entity mapping to `DimJurisdictions`
- `src/Core/Entities/UserRoleAssignment.cs` — entity mapping to `UserRoleAssignment`
- `specs/features/user-jurisdiction-assignment.md` — FEAT-002 spec

**Files modified:**
- `src/Core/Entities/UserRole.cs` — added `UserRoleAssignments` navigation property
- `src/Infrastructure/Data/AppDbContext.cs` — added `DbSet<UserRoleAssignment>`, `DbSet<Jurisdiction>`, and EF configuration
- `src/Infrastructure/Repositories/UserRepository.cs` — extended eager loading to include `UserRoleAssignments.ThenInclude(Jurisdiction)`
- `src/Core/Services/UserTokenData.cs` — added optional `JurisdictionId` and `JurisdictionDescription` fields
- `src/Core/Services/AuthResult.cs` — added `JurisdictionNotAssigned` to `AuthFailureReason` enum
- `src/Core/Services/AuthService.cs` — jurisdiction resolution logic for external users
- `src/Api/Services/TokenService.cs` — added `jurisdiction_id` and `jurisdiction_description` claims when present
- `tests/UnitTests/AuthServiceTests.cs` — 3 new tests + updated existing external user test
- `tests/UnitTests/TokenServiceTests.cs` — 2 new tests for jurisdiction claim presence/absence
- `specs/_index.md` — FEAT-002 added
- `docs/changelog.md` — this entry

---

## [FEAT-001] JWT Authentication — 2026-03-03

**Status:** Implementation complete, pending spec approval and integration testing.

**Summary:** Added JWT-based authentication supporting Internal (Windows identity) and External (`X-Logon-User` header) users. Authentication is transparent — users are identified automatically on first page load, a signed JWT is issued, and stored in a `HttpOnly` Secure `SameSite=Strict` cookie.

**Files added:**
- `src/Core/Entities/User.cs`, `Role.cs`, `UserRole.cs` — domain entities
- `src/Core/Interfaces/IUserRepository.cs`, `ITokenService.cs`, `IAuthService.cs` — contracts
- `src/Core/Services/AuthService.cs`, `JwtSettings.cs`, `UserTokenData.cs`, `AuthResult.cs` — business logic
- `src/Infrastructure/Data/AppDbContext.cs` — EF Core context mapping to existing tables
- `src/Infrastructure/Repositories/UserRepository.cs` — repository implementation
- `src/Api/Services/TokenService.cs` — JWT generation/validation
- `src/Api/Endpoints/AuthEndpoints.cs` — `POST /auth/token` minimal API endpoint
- `src/Portal/Services/ApiAuthClient.cs`, `IApiAuthClient.cs` — API HTTP client
- `src/Portal/Middleware/JwtAuthenticationMiddleware.cs` — auto-auth middleware
- `src/Portal/Components/Pages/AccessDenied.razor` — `/access-denied` page
- `tests/UnitTests/AuthServiceTests.cs` — 9 test scenarios for AuthService
- `tests/UnitTests/TokenServiceTests.cs` — 9 test scenarios for TokenService

**Files modified:**
- `src/Api/Program.cs` — JWT Bearer auth, DI registration, cookie token extraction
- `src/Api/appsettings.json` — `Jwt` section, `ConnectionStrings`
- `src/Portal/Program.cs` — JWT middleware, typed HttpClient registration
- `src/Portal/appsettings.json` — `ApiBaseUrl`
- `specs/_index.md` — FEAT-001 added

**Packages added:** `Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt`, `Microsoft.IdentityModel.Tokens` (Api); `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools` (Infrastructure); `System.IdentityModel.Tokens.Jwt` (Portal); `Moq`, `FluentAssertions` (UnitTests); `Microsoft.Extensions.Logging.Abstractions` (Core)

**Security notes:**
- JWT Secret must be set via `dotnet user-secrets` (`Jwt__Secret`) — never in `appsettings.json`
- `X-Logon-User` header trust is enforced at the network/gateway level, not in code

---