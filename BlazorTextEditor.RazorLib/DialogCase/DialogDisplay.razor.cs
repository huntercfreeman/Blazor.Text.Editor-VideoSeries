using BlazorTextEditor.ClassLib.Store.DialogCase;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.DialogCase;

public partial class DialogDisplay : ComponentBase
{
    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;
}