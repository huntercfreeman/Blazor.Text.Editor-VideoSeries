using BlazorTextEditor.ClassLib.Dimensions;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Editor;

public partial class EditorDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;
}