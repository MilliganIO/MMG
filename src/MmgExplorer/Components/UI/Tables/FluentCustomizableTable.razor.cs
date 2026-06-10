using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using MmgExplorer.Components.UI.Tables.Models;

namespace MmgExplorer.Components.UI.Tables;

public partial class FluentCustomizableTable<TItem> where TItem : class
{
    FluentDataGrid<TItem>? grid;
    PaginationState _pagination = new PaginationState { ItemsPerPage = 20 };
    private TItem? _focusedRow;

    /// <summary>
    /// Number of items to show per page. Defaults to 20.
    /// </summary>
    [Parameter] public int ItemsPerPage { get; set; } = 20;

    protected override void OnParametersSet()
    {
        if (_pagination.ItemsPerPage != ItemsPerPage)
        {
            _pagination = new PaginationState { ItemsPerPage = ItemsPerPage };
        }
    }

    [Inject]
    public required IDialogService DialogService { get; set; }

    [Parameter]
    public TableToolbarConfig ToolbarConfig { get; set; } = new();

    [Parameter]
    public EventCallback OnToggleColumns { get; set; }
    [Parameter]
    public EventCallback OnToggleFilters { get; set; }
    [Parameter]
    public bool IsLoading { get; set; } = true;

    /// <summary>Row height in pixels. Overrides <see cref="RowDensity"/> when set explicitly.</summary>
    [Parameter] public float? RowSize { get; set; }

    /// <summary>
    /// Semantic row density. Defaults to <see cref="DataGridRowSize.Small"/>.
    /// Use <see cref="DataGridRowSize.Large"/> for rows with prominent buttons,
    /// progress bars, or two-line cells.
    /// </summary>
    [Parameter] public DataGridRowSize RowDensity { get; set; } = DataGridRowSize.Small;

    [Parameter] public string ObjectNamePlural { get; set; } = "Objects";
    [Parameter] public string ObjectName { get; set; } = "Object";

    [Parameter]
    public IQueryable<TItem> Items { get; set; } = default!;

    [Parameter]
    public List<TableColumn<TItem>> Columns { get; set; } = new();

    #region Action Column Properties
    [Parameter]
    public bool RowHoverHighlight { get; set; }
    [Parameter]
    public string ActionColumnWidth { get; set; } = "auto";
    [Parameter]
    public string ActionColumnText { get; set; } = string.Empty;
    [Parameter]
    public string EndRowColumnText { get; set; } = string.Empty;

    [Parameter]
    public RenderFragment<TItem>? ActionContent { get; set; }
    #endregion

    #region Selectable Properties
    [Parameter]
    public bool IsSelectable { get; set; }

    [Parameter]
    public IEnumerable<TItem> SelectedItems { get; set; } = new HashSet<TItem>();

    [Parameter]
    public EventCallback<IEnumerable<TItem>> SelectedItemsChanged { get; set; }

    [Parameter]
    public string SelectableButtonText { get; set; } = string.Empty;

    [Parameter]
    public TableToolbarSelectionType SelectableActionType { get; set; }

    [Parameter]
    public EventCallback OnSelectableClick { get; set; }

    #endregion

    private IEnumerable<TableColumn<TItem>> VisibleColumns => Columns.Where(c => c.IsVisible).OrderBy(m => m.SortOrder);
    private bool HasUserGuide => UserGuideContent is not null || !string.IsNullOrEmpty(UserGuideHref);
    [Parameter] public string? UserGuideHref { get; set; }
    [Parameter] public RenderFragment? UserGuideContent { get; set; }

    [Parameter] public RenderFragment? ToolbarContent { get; set; }
    private bool HasEndRow => EndRowContent is not null;
    [Parameter] public string EndRowText { get; set; } = string.Empty;
    [Parameter] public RenderFragment<TItem>? EndRowContent { get; set; }
    private string GridTemplateColumns
    {
        get
        {
            var visibleCols = VisibleColumns.ToList();
            if (!visibleCols.Any()) return "auto";
            var columns = string.Join(" ", visibleCols.Select(c => c.Width ?? "auto"));

            if (IsSelectable)
                columns = $"40px {columns}";

            if (ActionContent is not null)
                columns = $"{ActionColumnWidth} {columns}";
            return columns;
        }
    }

    private void SetFocusedRow(TItem item) => _focusedRow = item;

    private string selectionActionCheckboxClass => SelectableActionType switch
    {
        TableToolbarSelectionType.Deletion => "delete-checkbox",
        TableToolbarSelectionType.Success => "success-checkbox",
        TableToolbarSelectionType.Info => "info-checkbox",
        TableToolbarSelectionType.Warning => "warning-action-btn",
        _ => string.Empty
    };
}

