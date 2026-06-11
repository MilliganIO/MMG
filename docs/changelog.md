# Changelog

> Auto-maintained by the Developer agent. Do not edit manually.
> New entries are prepended at the top after every completed feature.

> Note (2026-06-11): the previous contents of this file documented a different
> project ("Claude Learning" / CDE Dashboard, FEAT-001–FEAT-049) and were carried
> over as boilerplate. They were removed; see git history if needed. Numbering
> restarts at FEAT-001 for MMG Explorer.

---

## [FEAT-001] Theme Switching (Dark/Light/System) + Company Brand Theme — 2026-06-11

**Status:** ✅ Done

**Summary:** Added a Light/Dark/System theme switcher and applied the company brand colors as the application theme using FluentUI-Blazor v5's `IThemeService`. The primary brand color `#005778` seeds the generated FluentUI color ramp (`SetThemeAsync(color, isExact: true)`); accents `#FC4C02` (orange) and `#008E97` (teal) are exposed as CSS custom properties. A new app header bar (brand background, orange accent underline) hosts the app title and the switcher — an icon menu button whose icon reflects the active mode. Mode choice persists automatically via FluentUI's localStorage settings (`fluentui-blazor:theme-settings`) and is reapplied before Blazor starts, so there is no flash of the wrong theme. System mode follows the OS preference live. The brand ramp is applied once on first visit only — re-applying on every load would wipe the stored mode (the JS settings merge overwrites `mode` with `undefined`).

**New files:**
- `specs/_template.md`, `specs/_index.md` — spec workflow scaffolding
- `specs/features/theme-switching.md` — feature spec
- `src/MmgExplorer/Theming/BrandTheme.cs` — brand hex constants (single C# source of truth)
- `src/MmgExplorer/Components/Layout/ThemeSwitcher.razor` — mode switcher (menu of Light/Dark/System with checkmark on active mode; plain `FluentMenuItem` + `OnClick` because `Role=Radio` handles clicks inside the web component and they never reach Blazor)

**Modified files:**
- `src/MmgExplorer/Components/Layout/MainLayout.razor` — wrapped in `FluentLayout` with Header (title + spacer + ThemeSwitcher) and Content areas
- `src/MmgExplorer/wwwroot/app.css` — `--brand-*` CSS variables; `.app-header` styling via Fluent design-token overrides for on-brand contrast
- `.claude/launch.json` — dev-server launch config for previewing

**Fix (2026-06-11):** In light mode the switcher menu items rendered white-on-white. The `.app-header` rule remapped neutral foreground tokens to `--colorNeutralForegroundOnBrand` for the whole header subtree, and the menu popover — a DOM descendant of the header even though it displays in the top layer — inherited them. Token remaps are now scoped to the header title (`fluent-text`) and the trigger button only (`src/MmgExplorer/wwwroot/app.css`).

**Enhancement (2026-06-11):** Subtle brand-tinted page canvas. `html body` background is `color-mix(in srgb, var(--colorNeutralBackground1) 97%, var(--brand-primary))` — ~3% brand blue over the live neutral token, giving `#F7FAFB` in light mode and an imperceptible shift in dark mode. Derived from the live token (rather than a static light-mode override) because the library does not set `color-scheme`, so CSS cannot otherwise distinguish the effective mode without extra JS. Elevated surfaces (menus, cards, dialogs) intentionally keep the untinted neutral.

**Tests:** none — no test projects exist yet; verified manually in the running app (mode switching, persistence across reload, System following OS preference).

**Spec:** `specs/features/theme-switching.md`
