// spec: specs/features/theme-switching.md
namespace MmgExplorer.Theming;

/// <summary>
/// Company brand colors. <see cref="Primary"/> seeds the FluentUI theme ramp;
/// the accents are also exposed as CSS custom properties in wwwroot/app.css
/// (--brand-primary, --brand-accent-orange, --brand-accent-teal).
/// </summary>
public static class BrandTheme
{
    public const string Primary = "#005778";
    public const string AccentOrange = "#FC4C02";
    public const string AccentTeal = "#008E97";
}
