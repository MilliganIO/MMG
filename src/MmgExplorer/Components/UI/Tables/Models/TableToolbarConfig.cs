using Microsoft.AspNetCore.Components;

namespace MmgExplorer.Components.UI.Tables.Models;

public class TableToolbarConfig
{
    public bool CanExport { get; set; }
    public bool CanRefresh { get; set; }
    public bool CanChangeColumns { get; set; }
    public bool CanFilter { get; set; }

    public EventCallback OnExport { get; set; }
    public EventCallback OnRefresh { get; set; }
    public EventCallback OnToggleFilters { get; set; }
    public EventCallback OnToggleColumns { get; set; }
}
