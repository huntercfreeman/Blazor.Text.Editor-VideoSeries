using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Icons.Codicon;

public class IconBase : ComponentBase
{
    [Parameter]
    public int WidthInPixels { get; set; } = 24;
    [Parameter]
    public int HeightInPixels { get; set; } = 24;
}