# CLAUDE.md
> This file is the source of truth for how Claude operates in this project.
> Read this file fully before taking any action.

---

## 🏗️ Project Overview
<!-- UPDATE THIS SECTION as the project evolves -->
- **Project Name:** `MMG Explorer`
- **Description:** `This project is dedicated to generating information about message Mapping guides from the CDC. `
- **Status:** 🟡 In Development
- **Last Updated:** `6/10/2026`

---

## 📁 Project Structure

```
/
├── CLAUDE.md                        # ← You are here
├── .claude/
│   ├── agents/
│   │   ├── business-analyst.md      # BA agent instructions
│   │   └── developer.md             # Developer agent instructions
│   └── skills/
│       ├── write-spec.md            # How to write a spec
│       ├── implement-feature.md     # How to implement from a spec
│       └── update-docs.md           # How to update documentation
├── specs/
│   ├── _template.md                 # Spec template — copy this
│   ├── _index.md                    # Master feature index
│   └── features/
│       └── [feature-name].md        # One file per feature
├── docs/
│   ├── architecture.md              # System architecture
│   ├── api.md                       # API reference (auto-updated)
│   └── changelog.md                 # Feature changelog
├── src/
│   ├── [ProjectName].Api/           # ASP.NET Core Minimal API
│   │   ├── Endpoints/               # One file per feature group
│   │   │   └── [Feature]Endpoints.cs
│   │   ├── Middleware/
│   │   └── Program.cs
│   ├── [ProjectName].Core/          # Domain models, interfaces, business logic
│   │   ├── Entities/
│   │   ├── Interfaces/
│   │   └── Services/
│   ├── [ProjectName].Infrastructure/ # Data access, external services
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   └── Migrations/
│   │   └── Repositories/
│   └── [ProjectName].Web/           # Frontend Blazor Server with FluentUI-Blazor V5
│       ├── Pages/
│       └── Components/
└── tests/
    ├── [ProjectName].UnitTests/
    ├── [ProjectName].IntegrationTests/
    └── [ProjectName].E2ETests/          # Playwright end-to-end tests
```

---

## 🤖 Agents

Claude operates in two distinct agent modes. **Always specify which agent you want.**

| Agent | Trigger | Role |
|---|---|---|
| Business Analyst | `@ba` or "Act as BA" | Gathers requirements, writes specs |
| Developer | `@dev` or "Act as Developer" | Implements specs, writes code, updates docs |

Agent files live in `.claude/agents/`.

---

## 🔄 Spec-Driven Development Workflow

Every feature follows this lifecycle. **Do not skip steps.**

```
1. GATHER     →  @ba gathers requirements → produces spec file
2. REVIEW     →  Human reviews & approves spec
3. IMPLEMENT  →  @dev reads spec → writes code
4. DOCUMENT   →  @dev updates docs/changelog
5. CLOSE      →  Spec marked ✅ Done in _index.md
```

### Step-by-Step

**Step 1 — Requirements (BA Agent)**
```
@ba  I want to add [feature]. Ask me questions and produce a spec.
```
BA asks clarifying questions, then creates `specs/features/[feature-name].md`.

**Step 2 — Human Review**
Open the spec. Edit, approve, or reject. Add:
```
**STATUS: APPROVED** — [your name], [date]
```

**Step 3 — Implementation (Developer Agent)**
```
@dev  Implement the spec at specs/features/[feature-name].md
```
Dev reads the spec and writes code. If the spec is ambiguous, Dev STOPS and asks.

**Step 4 — Documentation**
Dev automatically updates:
- `docs/changelog.md`
- `docs/architecture.md` (if structure changed)
- `docs/api.md` (if endpoints changed)
- `specs/_index.md` (mark ✅ Done)

---

## 📋 Rules — All Agents Must Follow

1. **Spec before code.** No feature is coded without an approved spec.
2. **One spec per feature.** Keep specs atomic and focused.
3. **Don't invent requirements.** If unclear, ask the human.
4. **Update docs on every feature.** Documentation is never optional.
5. **Never modify an approved spec.** Create a `v2` spec for changes.
6. **Changelog entry required** for every completed feature.
7. **Reference specs in code** at the top of new classes/files:
   ```csharp
   // spec: specs/features/[feature-name].md
   ```

---

## 🧠 Tech Stack

- **Language:** C# 14 / .NET 10
- **Backend:** ASP.NET Core Web API — **Minimal APIs only** (no controllers)
- **Frontend:** Blazor Server with FluentUI-blazor v5 prerelease package
- **ORM:** Entity Framework Core 10
- **Database:** SQL Server
- **Auth:** ASP.NET Core Identity + JWT Bearer tokens
- **Testing:** xUnit + Moq + FluentAssertions + Playwright
- **Validation:** FluentValidation
- **Logging:** Serilog
- **API Docs:** Scalar (built-in .NET 10 OpenAPI support — no Swashbuckle needed)

---

## ✅ Coding Standards

### General
- Follow clean architecture: `Api` → `Core` → `Infrastructure` (dependencies point inward)
- `Core` project has zero infrastructure dependencies
- Use dependency injection for all services — register in `Program.cs`
- **Minimal APIs only** — no controllers, no `[ApiController]`, no MVC
- Organize endpoints using `IEndpointRouteBuilder` extension methods grouped by feature (e.g., `UserEndpoints.cs`, `OrderEndpoints.cs`)
- Register endpoint groups in `Program.cs` via `app.MapGroup()` / `app.MapUserEndpoints()`
- No business logic in endpoint handlers — delegate immediately to a service

### Naming
- Classes, methods, properties: `PascalCase`
- Local variables, parameters: `camelCase`
- Private fields: `_camelCase`
- Interfaces prefixed with `I`: `IUserService`
- File name matches class name exactly

### C# Specifics
- Use `record` types for DTOs and value objects
- Prefer `async/await` throughout — no `.Result` or `.Wait()`
- Use nullable reference types (`<Nullable>enable</Nullable>`)
- Use `IResult` / `Results<T>` for minimal API responses
- Guard clauses at the top of methods (fail fast)
- Never catch general `Exception` without logging and rethrowing

### EF Core
- Always use async methods: `ToListAsync()`, `FirstOrDefaultAsync()`, etc.
- Never call `SaveChanges()` — always `SaveChangesAsync()`
- Migrations live in `Infrastructure` project
- Seed data goes in `AppDbContext.OnModelCreating()`

### Testing
- Every service class has a corresponding `ServiceNameTests.cs`
- Tests named: `MethodName_Scenario_ExpectedResult`
- Use FluentAssertions: `result.Should().Be(expected)`
- Integration tests use `WebApplicationFactory<Program>`
- Minimum coverage target: business logic in `Core` = 80%
- Playwright E2E tests cover all critical user journeys (happy path + key error states)
- Playwright tests live in `[ProjectName].E2ETests/` and use the Page Object Model (POM) pattern
- E2E test naming: `[Feature]_[Scenario]_[ExpectedOutcome]` (e.g., `Login_WithInvalidCredentials_ShowsErrorMessage`)

### File Organization
- Max file length: 300 lines (split into partials or separate classes if longer)
- One class per file
- Group related endpoints in the same controller or route group

---

## 📌 Current Sprint / Active Features

| Feature | Spec | Status |
|---|---|---|
| — | — | — |

> See `specs/_index.md` for full history.