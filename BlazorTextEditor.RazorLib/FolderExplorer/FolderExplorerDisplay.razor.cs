using BlazorTextEditor.ClassLib.Dimensions;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.FolderExplorer;

public partial class FolderExplorerDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ElementDimensions FolderExplorerElementDimensions { get; set; } = null!;
}