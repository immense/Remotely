using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Components.TreeView;

public partial class TreeView<T> : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public required IEnumerable<T> DataSource { get; set; }

    [Parameter]
    [EditorRequired]
    public required Func<T, List<T>> ChildItemSelector { get; set; }

    [Parameter]
    [EditorRequired]
    public required Func<T, string> ItemHeaderSelector { get; set; }

    [Parameter]
    [EditorRequired]
    public required Func<T, string> KeySelector { get; set; }

    [Parameter]
    public EventCallback<T> ItemSelected { get; set; }

    [Parameter]
    public string? WrapperStyle { get; set; }

    [Parameter]
    public string? ChildItemStyle { get; set; }

    [Parameter]
    public int IndentLevel { get; set; }

    [Parameter]
    [EditorRequired]
    public required Func<T, TreeItemType> ItemTypeSelector { get; set; }

    [Parameter]
    [EditorRequired]
    public required Func<T, string>? ItemIconCssSelector { get; set; }

    public TreeViewItem<T>? SelectedNode { get; set; }

}
