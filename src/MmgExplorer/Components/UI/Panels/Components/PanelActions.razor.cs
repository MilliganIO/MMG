using Microsoft.AspNetCore.Components;

namespace MmgExplorer.Components.UI.Panels.Components;

public partial class PanelActions
{
    [Parameter] public string ApplyLabel { get; set; } = "Apply";
    [Parameter] public string ResetLabel { get; set; } = "Reset";

    [Parameter] public bool CanApply { get; set; } = true;
    [Parameter] public bool CanReset { get; set; } = true;

    [Parameter] public EventCallback OnApply { get; set; }
    [Parameter] public EventCallback OnReset { get; set; }

    [Parameter] public bool ShowUndo { get; set; }
    [Parameter] public bool CanUndo { get; set; }
    [Parameter] public EventCallback OnUndo { get; set; }

    private async Task HandleApply()
    {
        if (OnApply.HasDelegate)
            await OnApply.InvokeAsync();
    }

    private async Task HandleReset()
    {
        if (OnReset.HasDelegate)
            await OnReset.InvokeAsync();
    }

    private async Task HandleUndo()
    {
        if (OnUndo.HasDelegate)
            await OnUndo.InvokeAsync();
    }
}
