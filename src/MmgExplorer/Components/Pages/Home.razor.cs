using Microsoft.AspNetCore.Components;
using MmgExplorer.Components.UI.Tables.Models;
using MmgExplorer.Models;

namespace MmgExplorer.Components.Pages;

public partial class Home
{
    [Inject] private IMmgatClient client { get; set; }
    string? ErrorMessage;

    private bool loading = true;
    private List<Guide> records = [];
    private List<TableColumn<Guide>> columns = [];
    private TableToolbarConfig tableToolbarConfig = new();
    private ActiveTablePanel activePanel = ActiveTablePanel.None;
    private void ToggleColumnsPanel() => TogglePanel(ActiveTablePanel.Columns);
    private void ToggleFiltersPanel() => TogglePanel(ActiveTablePanel.Filters);
    private void CloseColumnsPanel() => ClosePanel(ActiveTablePanel.Columns);
    private void CloseFiltersPanel() => ClosePanel(ActiveTablePanel.Filters);
    private void TogglePanel(ActiveTablePanel panel) =>
        activePanel = activePanel == panel ? ActiveTablePanel.None : panel;

    private void ClosePanel(ActiveTablePanel panel)
    {
        if (activePanel == panel)
            activePanel = ActiveTablePanel.None;
    }

    protected override async Task OnInitializedAsync()
    {
        SetColumns();
        records.Clear();
        ErrorMessage = null;
        var response = await client.GetAllAsync();
        if (response.StatusCode == 200)
        {
            records = response.Result;
        }
        loading = false;
    }

}
