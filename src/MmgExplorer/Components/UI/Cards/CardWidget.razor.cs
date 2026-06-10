using Microsoft.AspNetCore.Components;

namespace MmgExplorer.Components.UI.Cards;

public partial class CardWidget
{
    private bool _isOptionsOpen;

    [Parameter] public string CardId { get; set; } = string.Empty;
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public string? Subtitle { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? Footer { get; set; }
    [Parameter] public RenderFragment? OptionsMenu { get; set; }

    private void ToggleOptionsMenu()
    {
        if (OptionsMenu is null)
            return;

        _isOptionsOpen = !_isOptionsOpen;
    }
}