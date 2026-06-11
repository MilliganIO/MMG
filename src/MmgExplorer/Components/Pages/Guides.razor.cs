using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using MmgExplorer.Models;
using MmgExplorer.Services.Interfaces;

namespace MmgExplorer.Components.Pages;

public partial class Guides
{
    [Inject] private IGuideClient GuideClient { get; set; } = default!;
    private bool loading = true;
    FluentDataGrid<Guide> grid = default!;
    List<Guide> records = [];
    string nameFilter = string.Empty;
    string errorMessage = string.Empty;
    PaginationState pagination = new PaginationState { ItemsPerPage = 10 };
    Func<Guide, string?> rowClass = x => x.Name.StartsWith("A") ? "highlighted" : null;
    Func<Guide, string?> rowStyle = x => x.Name.StartsWith("Au") ? "background-color: var(--brand-accent-orange)" : null;

    protected override async Task OnInitializedAsync()
    {
        loading = true;
        errorMessage = string.Empty;
        var response = await GuideClient.GetGuides();
        if (response.Success)
            records = response.Data ?? [];
        else
            errorMessage = response.ErrorMessage ?? "An unknown error occurred while fetching guides.";
        loading = false;
    }
}
