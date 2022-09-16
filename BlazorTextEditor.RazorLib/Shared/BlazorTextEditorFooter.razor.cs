using BlazorTextEditor.ClassLib.Dimensions;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Shared;

public partial class BlazorTextEditorFooter : ComponentBase
{
    [Parameter, EditorRequired]
    public ElementDimensions FooterElementDimensions { get; set; } = null!;
}