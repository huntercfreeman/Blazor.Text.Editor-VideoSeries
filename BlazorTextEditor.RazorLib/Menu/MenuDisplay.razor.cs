using BlazorTextEditor.ClassLib.Menu;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Menu;

public partial class MenuDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public MenuRecord MenuRecord { get; set; } = null!;
}