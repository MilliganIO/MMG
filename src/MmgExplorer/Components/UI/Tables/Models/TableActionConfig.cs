// spec: specs/features/fluent-customizable-datagrid-action-column.md
using Microsoft.FluentUI.AspNetCore.Components;

namespace MmgExplorer.Components.UI.Tables.Models;

public enum TableActionType
{
    Navigate,
    Modal,
    Panel
}

public class TableActionConfig<TItem> where TItem : class
{
    /// <summary>Which type of primary action this column performs.</summary>
    public TableActionType ActionType { get; set; }

    /// <summary>Width of the action column in CSS grid terms. Defaults to "auto".</summary>
    public string Width { get; set; } = "auto";

    // --- Navigate properties ---

    /// <summary>Base URL for navigate actions (e.g., "/admin/edit-user/").</summary>
    public string? NavigateBaseHref { get; set; }

    /// <summary>Extracts the route parameter value from the item (e.g., item => item.UserId.ToString()).</summary>
    public Func<TItem, string>? NavigateRouteValueAccessor { get; set; }

    public string NavigateDisplayText { get; set; } = string.Empty;
    /// <summary>Extracts the display text for the link (e.g., item => item.Email).</summary>
    public Func<TItem, string>? NavigateDisplayTextAccessor { get; set; }

    /// <summary>FluentUI icon to show alongside the link/button.</summary>
    public Icon? Icon { get; set; }

    /// <summary>Tooltip for the action button.</summary>
    public Func<TItem, string>? ToolTipDescription { get; set; }
    // --- Modal / Panel properties ---

    /// <summary>Callback invoked for Modal or Panel action types, passing the clicked item.</summary>
    public Func<TItem, Task>? OnActionClick { get; set; }

    /// <summary>Text label for the modal/panel button.</summary>
    public string? ButtonText { get; set; }
}
