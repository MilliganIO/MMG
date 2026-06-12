using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using MmgExplorer.Models;

namespace MmgExplorer.Components.Pages.Guides.Components;

public partial class GuideDisplay
{
    string? activeId;
    [Parameter, EditorRequired] public Guide? Guide { get; set; }

    private bool descriptionExpanded;

    private void ToggleDescription() => descriptionExpanded = !descriptionExpanded;

    protected override void OnParametersSet()
    {
        // Re-collapse when a different guide is displayed.
        descriptionExpanded = false;
    }

    private void HandleOnAccordionItemChange(AccordionItemEventArgs args)
    {
        activeId = args?.Item?.Id;
    }
}
