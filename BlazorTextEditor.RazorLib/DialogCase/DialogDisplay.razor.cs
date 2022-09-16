using BlazorTextEditor.ClassLib.Store.DialogCase;
using BlazorTextEditor.RazorLib.ResizableCase;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.DialogCase;

public partial class DialogDisplay : ComponentBase
{
    private ResizableDisplay? _resizableDisplay;

    [Parameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    private string StyleCssString => DialogRecord.ElementDimensions.StyleString;
    
    private async Task ReRenderAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    private void SubscribeMoveHandle()
    {
        _resizableDisplay?.SubscribeToDragEventWithMoveHandle();
    }
}