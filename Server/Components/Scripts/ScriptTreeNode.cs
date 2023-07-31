using Remotely.Server.Components.TreeView;
using Remotely.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Components.Scripts;

public class ScriptTreeNode
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public TreeItemType ItemType { get; set; }
    public string Name { get; init; } = string.Empty;
    public ScriptTreeNode? ParentNode { get; set; }
    public List<ScriptTreeNode> ChildItems { get; } = new();
    public SavedScript? Script { get; init; }
}
