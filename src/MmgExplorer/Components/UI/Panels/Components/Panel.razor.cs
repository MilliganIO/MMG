using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using MmgExplorer.Components.UI.Panels.Models;

namespace MmgExplorer.Components.UI.Panels.Components;

public partial class Panel
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public PanelWidthVariant WidthVariant { get; set; } = PanelWidthVariant.Narrow;

    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public Icon? HeaderIcon { get; set; }
    [Parameter] public bool ShowCloseButton { get; set; } = true;
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public RenderFragment? HeaderActions { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? FooterContent { get; set; }

    [Parameter] public string? AriaLabel { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    private bool ShowHeader =>
        !string.IsNullOrEmpty(Title) || HeaderIcon is not null || HeaderActions is not null || ShowCloseButton;

    private string WidthVariantClass => WidthVariant switch
    {
        PanelWidthVariant.Standard => "panel-width-standard",
        PanelWidthVariant.Wide => "panel-width-wide",
        PanelWidthVariant.ExtraWide => "panel-width-extra-wide",
        PanelWidthVariant.Maximized => "panel-width-maximized",
        PanelWidthVariant.Quarter => "panel-width-quarter",
        PanelWidthVariant.Half => "panel-width-half",
        PanelWidthVariant.ThreeQuarters => "panel-width-threequarters",
        _ => ""
    };

    private async Task HandleClose()
    {
        if (OnClose.HasDelegate)
            await OnClose.InvokeAsync();
    }
}
