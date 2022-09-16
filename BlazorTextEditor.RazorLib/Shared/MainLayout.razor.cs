using BlazorTextEditor.ClassLib.Store.DialogCase;
using BlazorTextEditor.ClassLib.Store.DragCase;
using BlazorTextEditor.RazorLib.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Shared;

public partial class MainLayout : LayoutComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private string _message = string.Empty;
    
    private string UnselectableClassCss => DragStateWrap.Value.ShouldDisplay
        ? "bte_unselectable"
        : string.Empty;
    
    private bool _previousDragStateWrapShouldDisplay;

    private int _renderCount = 1;

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        
        base.OnInitialized();
    }
    
    protected override void OnAfterRender(bool firstRender)
    {
        _renderCount++;
    
        base.OnAfterRender(firstRender);
    }

    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (_previousDragStateWrapShouldDisplay != DragStateWrap.Value.ShouldDisplay)
        {
            _previousDragStateWrapShouldDisplay = DragStateWrap.Value.ShouldDisplay;
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OpenDialogOnClick()
    {
        Dispatcher.Dispatch(new RegisterDialogRecordAction(new DialogRecord(
            DialogKey.NewDialogKey(), 
            "Example",
            typeof(ExampleDialog),
            new Dictionary<string, object?>
            {
                { nameof(ExampleDialog.Message), _message }
            })));
    }

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}