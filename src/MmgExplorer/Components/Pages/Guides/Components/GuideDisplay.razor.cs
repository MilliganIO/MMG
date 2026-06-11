using Microsoft.AspNetCore.Components;
using MmgExplorer.Models;

namespace MmgExplorer.Components.Pages.Guides.Components;

public partial class GuideDisplay
{
    [Parameter, EditorRequired] public Guide? Guide { get; set; }
}
