using BlazorTextEditor.ClassLib.Dimensions;
using BlazorTextEditor.ClassLib.Store.DragCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.ResizableCase;

public partial class ResizableDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<DragState> DragStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<Task> ReRenderFuncAsync { get; set; } = null!;
    
    public const int RESIZE_HANDLE_SQUARE_PIXELS = 7;

    private Func<MouseEventArgs, Task>? _dragEventHandler;
    private MouseEventArgs? _previousDragMouseEventArgs;
    
    private ElementDimensions _northResizeHandleDimensions = new();
    private ElementDimensions _eastResizeHandleDimensions = new();
    private ElementDimensions _southResizeHandleDimensions = new();
    private ElementDimensions _westResizeHandleDimensions = new(); 	
    private ElementDimensions _northEastResizeHandleDimensions = new();
    private ElementDimensions _southEastResizeHandleDimensions = new();
    private ElementDimensions _southWestResizeHandleDimensions = new();
    private ElementDimensions _northWestResizeHandleDimensions = new();

    protected override void OnInitialized()
    {
        DragStateWrap.StateChanged += DragStateWrapOnStateChanged;
        
        base.OnInitialized();
    }
    
    private async void DragStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (!DragStateWrap.Value.ShouldDisplay)
        {
            _dragEventHandler = null;
            _previousDragMouseEventArgs = null;
        }
        else
        {
            var mouseEventArgs = DragStateWrap.Value.MouseEventArgs;

            if (_dragEventHandler is not null)
            {
                if (_previousDragMouseEventArgs is not null &&
                    mouseEventArgs is not null)
                {
                    await _dragEventHandler.Invoke(mouseEventArgs);
                }

                _previousDragMouseEventArgs = mouseEventArgs;
                await ReRenderFuncAsync.Invoke();
            }
        }
    }

    private void SubscribeToDragEvent(Func<MouseEventArgs, Task> dragEventHandler)
    {
        _dragEventHandler = dragEventHandler;
    }
    
    #region ResizeHandleStyleCss
    private string GetNorthResizeHandleStyleCss()
    {
        return string.Empty;
    }
    
    private string GetEastResizeHandleStyleCss()
    {
        return string.Empty;
    }
    
    private string GetSouthResizeHandleStyleCss()
    {
        return string.Empty;
    }
    
    private string GetWestResizeHandleStyleCss()
    {
        return string.Empty;
    }
    
    private string GetNorthEastResizeHandleStyleCss()
    {
        return string.Empty;
    }
    
    private string GetSouthEastResizeHandleStyleCss()
    {
        return string.Empty;
    }
    
    private string GetSouthWestResizeHandleStyleCss()
    {
        return string.Empty;
    }
    
    private string GetNorthWestResizeHandleStyleCss()
    {
        return string.Empty;
    }
    #endregion

    #region DragEventHandlers
    private async Task DragEventHandlerNorthResizeHandleAsync(MouseEventArgs mouseEventArgs)
    {
    }
    
    private async Task DragEventHandlerEastResizeHandleAsync(MouseEventArgs mouseEventArgs)
    {
    }
    
    private async Task DragEventHandlerSouthResizeHandleAsync(MouseEventArgs mouseEventArgs)
    {
    }
    
    private async Task DragEventHandlerWestResizeHandleAsync(MouseEventArgs mouseEventArgs)
    {
    }
    
    private async Task DragEventHandlerNorthEastResizeHandleAsync(MouseEventArgs mouseEventArgs)
    {
    }
    
    private async Task DragEventHandlerSouthEastResizeHandleAsync(MouseEventArgs mouseEventArgs)
    {
    }
    
    private async Task DragEventHandlerSouthWestResizeHandleAsync(MouseEventArgs mouseEventArgs)
    {
    }
    
    private async Task DragEventHandlerNorthWestResizeHandleAsync(MouseEventArgs mouseEventArgs)
    {
    }
    #endregion

    public void Dispose()
    {
        DragStateWrap.StateChanged -= DragStateWrapOnStateChanged;
    }
}