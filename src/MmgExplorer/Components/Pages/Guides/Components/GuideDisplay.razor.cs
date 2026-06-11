using Microsoft.AspNetCore.Components;
using MmgExplorer.Models;

namespace MmgExplorer.Components.Pages.Guides.Components;

public partial class GuideDisplay
{
    [Parameter, EditorRequired] public Guide? Guide { get; set; }

    private bool descriptionExpanded;

    private void ToggleDescription() => descriptionExpanded = !descriptionExpanded;

    protected override void OnParametersSet()
    {
        // Re-collapse when a different guide is displayed.
        descriptionExpanded = false;
    }
}
