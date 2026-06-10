using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Linq.Expressions;

namespace MmgExplorer.Components.UI.Tables.Models;

public class TableColumn<TItem>
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool Sortable { get; set; } = true;
    public bool IsVisible { get; set; } = true;
    public string? Width { get; set; }
    public DataGridCellAlignment Alignment { get; set; } = DataGridCellAlignment.Start;
    public string? CssClass { get; set; }
    public RenderFragment<TItem>? Template { get; set; }
    public Func<TItem, string>? DisplayFunc { get; set; }
    public GridSort<TItem>? SortByExpression { get; set; }
    public int SortOrder { get; set; }

    public static TableColumn<TItem> Create<TProp>(
        string title,
        Expression<Func<TItem, TProp>> property,
        bool sortable = true,
        string? width = null,
        DataGridCellAlignment alignment = DataGridCellAlignment.Start,
        Func<TProp, string>? formatFunc = null)
    {
        var compiled = property.Compile();
        var key = GetMemberName(property) ?? title;

        return new TableColumn<TItem>
        {
            Key = key,
            Title = title,
            DisplayFunc = item =>
            {
                var value = compiled(item);
                if (value == null) return string.Empty;
                return formatFunc != null ? formatFunc(value) : value.ToString() ?? string.Empty;
            },
            SortByExpression = sortable ? GridSort<TItem>.ByAscending(property).ThenAscending(property) : null,
            Sortable = sortable,
            Width = width,
            Alignment = alignment
        };
    }

    private static string? GetMemberName<TProp>(Expression<Func<TItem, TProp>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        if (expression.Body is UnaryExpression unaryExpression &&
            unaryExpression.Operand is MemberExpression unaryMemberExpression)
        {
            return unaryMemberExpression.Member.Name;
        }

        return null;
    }
}
