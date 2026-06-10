using Microsoft.AspNetCore.Components;

namespace MmgExplorer.Components.UI.Cards;

public partial class Card
{
    [Parameter]
    public string SubTitle { get; set; } = string.Empty;

    [Parameter]
    public string BorderDirection { get; set; } = string.Empty;

    [Parameter]
    public string BorderColor { get; set; } = string.Empty;

    [Parameter]
    public string Class { get; set; } = string.Empty;

    [Parameter]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// When true, title, subtitle, and table-toolbar (from FluentDataTable) share one header row.
    /// Use for Card + FluentDataTable layouts (Messages, ErrorsAndWarnings, etc.).
    /// </summary>
    [Parameter]
    public bool TableLayout { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback OnClick { get; set; }

    private string CardClass
    {
        get
        {
            var classes = new List<string>();
            if (!string.IsNullOrWhiteSpace(BorderDirection))
            {
                classes.Add($"{BorderDirection.ToLower()}-border");
            }

            if (!string.IsNullOrWhiteSpace(BorderColor))
            {
                classes.Add($"border-{BorderColor.ToLower()}");
            }

            return string.Join(" ", classes);
        }
    }

    private async Task OnClickHandler()
    {
        await OnClick.InvokeAsync();
    }

}
