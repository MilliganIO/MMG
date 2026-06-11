# Feature: Theme Switching (Dark/Light/System) + Company Brand Theme

**STATUS: APPROVED** — Josh Milligan, 2026-06-11 (approved via plan review)

## Overview
Let users switch MMG Explorer between Light, Dark, and System theme modes, and apply the company's brand colors as the application theme using FluentUI-Blazor V5's theming engine.

## Requirements
- [x] A theme switcher control offering three modes: **Light**, **Dark**, **System** (System follows the OS preference).
- [x] The switcher lives in a new application header bar (app title left, switcher right).
- [x] The selected mode persists across page reloads and browser sessions.
- [x] The FluentUI theme is generated from the company primary color so all components use the brand color ramp in both light and dark modes.
- [x] Company colors:
  - Primary: `#005778` (seeds the FluentUI theme)
  - Accent orange: `#FC4C02`
  - Accent teal: `#008E97`
- [x] All three colors are defined in exactly one place in C# (`BrandTheme`) and exposed as CSS custom properties for stylesheet use.

## Out of Scope
- Per-user theme storage on the server (preference is browser-local via localStorage).
- Navigation menu / additional header content (header hosts title + switcher only for now).
- Automated E2E tests — no test projects exist in the repo yet; verification is manual.

## Technical Notes
- FluentUI-Blazor **v5.0.0-rc.3** replaces v4's `<FluentDesignTheme>` with `IThemeService` (registered by the existing `AddFluentUIComponents()` call).
- `IThemeService.SetThemeAsync(string color, bool isExact)` generates the brand ramp preserving the current mode; `SetThemeAsync(ThemeMode)` changes mode preserving the brand color.
- Settings persist automatically to localStorage key `fluentui-blazor:theme-settings` and are re-applied by the library's JS module before Blazor starts (no flash of wrong theme).
- `IThemeService` uses JS interop — calls only from `OnAfterRenderAsync`, never during prerender.
- New files: `src/MmgExplorer/Theming/BrandTheme.cs`, `src/MmgExplorer/Components/Layout/ThemeSwitcher.razor`. Modified: `MainLayout.razor`, `wwwroot/app.css`.

## Acceptance Criteria
- [x] Header bar renders with brand styling; FluentUI components use the #005778-derived ramp instead of default Fluent blue.
- [x] Selecting Dark switches the whole app to dark mode; Light switches back; System follows the OS setting (verified live via emulated `prefers-color-scheme`).
- [x] After choosing a mode and reloading the page, the choice is still applied and the switcher reflects it.
- [x] `--brand-primary`, `--brand-accent-orange`, `--brand-accent-teal` CSS variables are available app-wide.

Verified manually in the running app on 2026-06-11.
