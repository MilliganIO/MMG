using Microsoft.AspNetCore.Components;
using MmgExplorer.Components.UI.Tables.Models;

namespace MmgExplorer.Components.UI.Tables.Components;

public partial class TableToolbar
{
    [Parameter]
    public bool CanSelectColumns { get; set; }

    [Parameter]
    public bool CanFilter { get; set; }

    [Parameter]
    public bool CanExport { get; set; }

    [Parameter]
    public bool CanRefresh { get; set; }

    [Parameter]
    public EventCallback OnRefresh { get; set; }

    [Parameter]
    public EventCallback OnExport { get; set; }

    [Parameter]
    public EventCallback OnToggleColumns { get; set; }

    [Parameter]
    public EventCallback OnToggleFilters { get; set; }

    [Parameter]
    public TableToolbarLayout ToolbarLayout { get; set; } = TableToolbarLayout.Horizontal;

    [Parameter]
    public TableToolbarButtonMode ToolbarButtonMode { get; set; } = TableToolbarButtonMode.IconAndText;

    [Parameter]
    public RenderFragment? ExtraToolbarContent { get; set; }

    #region Selectable Properties
    [Parameter]
    public bool HasSelectableRows { get; set; }
    [Parameter]
    public int SelectedCount { get; set; }

    [Parameter]
    public string SelectionButtonText { get; set; } = string.Empty;

    [Parameter]
    public TableToolbarSelectionType SelectionActionType { get; set; }
    [Parameter]
    public EventCallback OnSelectionClick { get; set; }
    #endregion
    private string ToolbarClass =>
        ToolbarLayout == TableToolbarLayout.Vertical
            ? "table-toolbar table-toolbar-vertical"
            : "table-toolbar table-toolbar-horizontal";

    private string ToolbarActionsClass =>
        ToolbarButtonMode == TableToolbarButtonMode.IconsOnly
            ? "table-toolbar-actions table-toolbar-actions-icons-only"
            : "table-toolbar-actions";
    private string selectionActionClass => SelectionActionType switch
    {
        TableToolbarSelectionType.Deletion => "delete-action-btn",
        TableToolbarSelectionType.Success => "success-action-btn",
        TableToolbarSelectionType.Info => "info-action-btn",
        TableToolbarSelectionType.Warning => "warning-action-btn",
        _ => string.Empty
    };

    private bool ShowToolbarLabels => ToolbarButtonMode == TableToolbarButtonMode.IconAndText;

    private async Task HandleRefresh()
    {
        if (OnRefresh.HasDelegate)
        {
            await OnRefresh.InvokeAsync();
        }
    }

    private async Task HandleExport()
    {
        if (OnExport.HasDelegate)
        {
            await OnExport.InvokeAsync();
        }
    }

    private async Task HandleToggleFilter()
    {
        if (OnToggleFilters.HasDelegate)
        {
            await OnToggleFilters.InvokeAsync();
        }

    }
    private async Task HandleToggleColumns()
    {
        if (OnToggleColumns.HasDelegate)
            await OnToggleColumns.InvokeAsync();
    }
}