# Architecture

> Maintained by the Developer agent. Updated after every feature that changes structure.
> Last updated: `2026-06-11`

> Note (2026-06-11): the previous contents of this file documented a different
> project ("Claude Learning" / CDE Dashboard) and were carried over as boilerplate.
> Replaced with the actual MMG Explorer architecture; see git history if needed.

---

## Overview

MMG Explorer generates information about Message Mapping Guides from the CDC. It is currently a single Blazor Server project orchestrated by .NET Aspire. The clean-architecture layers described in `CLAUDE.md` (Api / Core / Infrastructure) will be introduced when the first backend feature needs them.

---

## Projects

| Project | Path | Responsibility |
|---|---|---|
| `MmgExplorer` | `src/MmgExplorer/` | Blazor Server frontend (FluentUI-Blazor v5, interactive server render mode) |
| `Mmg.AppHost` | `Mmg.AppHost/` | .NET Aspire orchestration (runs `MmgExplorer` as `"web"`) |
| `Mmg.ServiceDefaults` | `Mmg.ServiceDefaults/` | Shared Aspire defaults (OpenTelemetry, resilience, service discovery) |

---

## Frontend (MmgExplorer)

```
src/MmgExplorer/
├── Components/
│   ├── App.razor                  # Root HTML document, asset includes
│   ├── Routes.razor               # Router
│   ├── Layout/
│   │   ├── MainLayout.razor       # FluentLayout: app header (title + ThemeSwitcher) + content
│   │   └── ThemeSwitcher.razor    # Light/Dark/System mode menu (FEAT-001)
│   └── Pages/                     # Home, Error, NotFound
├── Theming/
│   └── BrandTheme.cs              # Company brand color constants (FEAT-001)
└── wwwroot/
    └── app.css                    # Brand CSS variables, app-header design-token overrides
```

### Theming (FEAT-001)

- FluentUI-Blazor **v5** theming via `IThemeService` (registered by `AddFluentUIComponents()`).
- Brand ramp generated from `BrandTheme.Primary` (`#005778`, exact); applied on first visit only.
- Mode (Light/Dark/System) set via `SetThemeAsync(ThemeMode)`; persisted by the library in localStorage (`fluentui-blazor:theme-settings`) and reapplied before Blazor starts.
- Accent colors exposed as CSS variables: `--brand-primary`, `--brand-accent-orange`, `--brand-accent-teal`.
- Header contrast handled by remapping neutral foreground design tokens inside `.app-header`.

---

## Key Packages

| Package | Purpose |
|---|---|
| `Microsoft.FluentUI.AspNetCore.Components` v5.0.0-rc.3 (prerelease) | FluentUI-Blazor UI library |
| `Microsoft.FluentUI.AspNetCore.Components.Icons` | Fluent icon set |
| Aspire AppHost SDK | Orchestration |

---

## Planned (per CLAUDE.md, not yet present)

- `Api` (ASP.NET Core Minimal APIs), `Core`, `Infrastructure` (EF Core 10 + SQL Server) projects
- ASP.NET Core Identity + JWT auth, FluentValidation, Serilog, Scalar API docs
- Test projects: UnitTests (xUnit + Moq + FluentAssertions), IntegrationTests, E2ETests (Playwright)
