# Architecture

> Maintained by the Developer agent. Updated after every feature that changes structure.
> Last updated: `2026-03-21`

---

## Overview

Claude Learning is a full stack C# application built on clean architecture principles, orchestrated by .NET Aspire.

---

## Layer Diagram

```
┌─────────────────────────────────────────┐
│              Frontend (Portal)          │  Blazor Server with FluentUI-Blazor
├─────────────────────────────────────────┤
│              API Layer                  │  ASP.NET Core Minimal API
├─────────────────────────────────────────┤
│              Core Layer                 │  Domain entities, interfaces, services
├─────────────────────────────────────────┤
│           Infrastructure Layer          │  EF Core, repositories, external services
├─────────────────────────────────────────┤
│              Databases                  │  SQL Server (CDE_Dashboard + CDE_ENV)
└─────────────────────────────────────────┘
```

**Dependency rule:** Outer layers depend on inner layers. `Core` has zero references to `Api` or `Infrastructure`.

---

## Projects

| Project | Responsibility |
|---|---|
| `Api` | Minimal API endpoint handlers, route groups, JWT auth |
| `Core` | Domain entities, service interfaces, DTOs, business logic |
| `Infrastructure` | EF Core DbContexts, repositories |
| `Portal` | Blazor Server frontend (FluentUI-Blazor), middleware, API clients |
| `Claude-Learning.AppHost` | .NET Aspire orchestration (Api + Portal) |
| `Claude-Learning.ServiceDefaults` | Shared Aspire defaults |
| `UnitTests` | xUnit v3 + Moq + FluentAssertions |
| `IntegrationTests` | xUnit integration tests with WebApplicationFactory |
| `E2ETests` | Playwright end-to-end tests |

---

## Databases

| Database | DbContext | Connection String Key | Purpose |
|---|---|---|---|
| `CDE_Dashboard` | `AppDbContext` | `CDEDashboard` | Primary database — users, roles, jurisdictions, programs, lookups |
| `CDE_ENV` | `CdeEnvDbContext` | `CDE_ENV` | MMWR week lookup data (`[netss].[MMWRWeekLookup]`), read-only |

---

## Domain Entities

> Updated as entities are added.

| Entity | Table | Description | Added In |
|---|---|---|---|
| `User` | `Users` | Application user | FEAT-001 |
| `Role` | `Roles` | User roles with `RoleGroup` | FEAT-001, FEAT-004 |
| `UserRole` | `UserRoles` | User-role assignments with `IsCurrentRole` | FEAT-001 |
| `Jurisdiction` | `DimJurisdictions` | Jurisdiction lookup | FEAT-002 |
| `UserRoleAssignment` | `UserRoleAssignment` | Links roles to jurisdictions/programs | FEAT-002 |
| `CdcProgram` | `DimPrograms` | CDC program lookup | FEAT-003 |
| `Workflow` | `Workflow` | Workflow definition | FEAT-013 |
| `Status` | `Status` | Status definition | FEAT-013 |
| `WorkflowStatus` | `WorkflowStatus` | Workflow-status junction | FEAT-013 |
| `ClassificationStatus` | `ClassificationStatus` | Classification status lookup | FEAT-014 |
| `ProcessingStatus` | `ProcessingStatus` | Processing status lookup | FEAT-015 |
| `BatchFileStatus` | `BatchFileStatus` | Batch file status lookup | FEAT-016 |
| `CdcProfile` | `CdcProfile` | CDC profile lookup | FEAT-017 |
| `UserColumnPreference` | `UserColumnPreferences` | Per-user column visibility/order | FEAT-008 |

---

## Services

| Interface | Implementation | Registered As | Added In |
|---|---|---|---|
| `IAuthService` | `AuthService` | Scoped | FEAT-001 |
| `ITokenService` | `TokenService` | Scoped | FEAT-001 |
| `IUserRepository` | `UserRepository` | Scoped | FEAT-001 |
| `ILookupRepository` | `LookupRepository` | Scoped | FEAT-007 |
| `IMmwrRepository` | `MmwrRepository` | Scoped | FEAT-018 |
| `IColumnPreferenceRepository` | `ColumnPreferenceRepository` | Scoped | FEAT-008 |
| `IColumnPreferenceService` | `ColumnPreferenceService` | Scoped | FEAT-008 |
| `ICurrentUserService` | `CurrentUserService` | Scoped | FEAT-009 |
| `IApiClient` | `ApiClient` | HttpClient (Portal) | FEAT-001 |
| `ILookupApiClient` | `LookupApiClient` | Scoped (Portal) | FEAT-007 |

---

## API Routes

> See `docs/api.md` for full endpoint documentation.

| Method | Route | Auth | Handler File | Added In |
|---|---|---|---|---|
| POST | `/auth/token` | No | `AuthEndpoints.cs` | FEAT-001 |
| GET | `/api/Lookups/Programs` | Yes | `LookupEndpoints.cs` | FEAT-007 |
| GET | `/api/Lookups/MessageTypes` | Yes | `LookupEndpoints.cs` | FEAT-007 |
| GET | `/api/Lookups/MessageStatuses` | Yes | `LookupEndpoints.cs` | FEAT-007 |
| GET | `/api/Lookups/Categories` | Yes | `LookupEndpoints.cs` | FEAT-007 |
| GET | `/api/Lookups/Jurisdictions` | Yes | `LookupEndpoints.cs` | FEAT-007 |
| GET | `/api/Lookups/EventCodes` | Yes | `LookupEndpoints.cs` | FEAT-007 |
| GET | `/api/Lookups/Workflows/{id}/Statuses` | Yes | `LookupEndpoints.cs` | FEAT-013 |
| GET | `/api/Lookups/ClassificationStatuses` | Yes | `LookupEndpoints.cs` | FEAT-014 |
| GET | `/api/Lookups/ProcessingStatuses` | Yes | `LookupEndpoints.cs` | FEAT-015 |
| GET | `/api/Lookups/BatchFileStatuses` | Yes | `LookupEndpoints.cs` | FEAT-016 |
| GET | `/api/Lookups/CdcProfiles` | Yes | `LookupEndpoints.cs` | FEAT-017 |
| GET | `/api/Lookups/Mmwrs` | Yes | `LookupEndpoints.cs` | FEAT-018 |
| GET | `/column-preferences` | Yes | `ColumnPreferenceEndpoints.cs` | FEAT-008 |
| PUT | `/column-preferences` | Yes | `ColumnPreferenceEndpoints.cs` | FEAT-008 |
| DELETE | `/column-preferences` | Yes | `ColumnPreferenceEndpoints.cs` | FEAT-008 |

---

## FilterPanel Controls

The FilterPanel component (`FilterPanel.razor`) provides the following filter controls on the Messages page:

| Control | Type | Config Flag | Added In |
|---|---|---|---|
| Program | `FilterField` (dropdown) | `IncludeCdcPrograms` | FEAT-007 |
| Message Type | `FilterField` (dropdown) | `IncludeMessageTypes` | FEAT-007 |
| Message Status | `FilterField` (dropdown, string) | `IncludeMessageStatuses` | FEAT-007 |
| Category | `FilterField` (dropdown) | `IncludeCategories` | FEAT-007 |
| Jurisdiction | `FilterField` (dropdown) | `IncludeJurisdictions` | FEAT-007 |
| Event Code | `FilterField` (dropdown) | `IncludeEventCodes` | FEAT-007 |
| Message Set | Inline `FluentSelect` | `IncludeMessageSets` | FEAT-012 |
| Classification Status | `FilterField` (dropdown) | `IncludeClassificationStatuses` | FEAT-014 |
| Processing Status | `FilterField` (dropdown) | `IncludeProcessingStatuses` | FEAT-015 |
| Batch File Status | `FilterField` (dropdown) | `IncludeBatchFileStatuses` | FEAT-016 |
| Profile | `FilterField` (dropdown) | `IncludeCdcProfiles` | FEAT-017 |
| Jurisdiction Status | `FilterField` (dropdown) | `IncludeWorkflowStatuses` | FEAT-013 |
| Program Status | `FilterField` (dropdown) | `IncludeWorkflowStatuses` | FEAT-013 |
| MMWR Year | Inline `FluentSelect` | `IncludeMmwr` | FEAT-018 |
| Start MMWR Week | Inline `FluentSelect` | `IncludeMmwr` | FEAT-018 |
| End MMWR Week | Inline `FluentSelect` | `IncludeMmwr` | FEAT-018 |
| Start Date | `FluentDatePicker` | `IncludeDateRange` | FEAT-019 |
| End Date | `FluentDatePicker` | `IncludeDateRange` | FEAT-019 |

---

## Key Packages

| Package | Purpose |
|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | ORM (EF Core 10) |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT Bearer auth |
| `Microsoft.FluentUI.AspNetCore.Components` | FluentUI-Blazor UI library |
| `Scalar` | API docs UI (built into .NET 10 OpenAPI) |
| `xUnit` v3 | Testing framework |
| `Moq` | Mocking framework |
| `FluentAssertions` | Test assertions |
| `Microsoft.Playwright` | E2E browser testing |

---

## Configuration

Environment-specific settings use `appsettings.{Environment}.json`. Secrets are managed via:
- Local: `dotnet user-secrets`
- Production: Environment Variables

Required configuration:
```
ConnectionStrings__CDEDashboard   # Primary database connection string
ConnectionStrings__CDE_ENV        # MMWR lookup database connection string
Jwt__Secret                       # JWT signing key (32+ chars, user-secrets only)
Jwt__Issuer                       # JWT issuer
Jwt__Audience                     # JWT audience
Jwt__ExpiryMinutes                # Token expiry (default 60)
```
