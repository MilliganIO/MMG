using Microsoft.AspNetCore.Components;
using MmgExplorer.Models;

namespace MmgExplorer.Components.Pages.Guides.Components;

public partial class GuidIdAction
{
    private bool isLoading;
    [Parameter, EditorRequired] public Guide? Guide { get; set; }
    [Parameter] public EventCallback OnOpen { get; set; }


    private async Task HandleOpen()
    {
        isLoading = true;
        if (OnOpen.HasDelegate)
            await OnOpen.InvokeAsync();
        isLoading = false;
    }
}
