using BlazorTextEditor.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.TextEditor;

public partial class TextEditorCursorDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorCursor TextEditorCursor { get; set; } = null!;
    [Parameter, EditorRequired]
    public FontWidthAndElementHeight FontWidthAndElementHeight { get; set; } = null!;
    [Parameter, EditorRequired]
    public string ScrollableContainerId { get; set; } = null!;
    [Parameter, EditorRequired]
    public bool IsFocusTarget { get; set; }

    private ElementReference? _textEditorCursorDisplayElementReference;
    private bool _hasBlinkAnimation = true;
    private CancellationTokenSource _blinkingCursorCancellationTokenSource = new();
    private TimeSpan _blinkingCursorTaskDelay = TimeSpan.FromMilliseconds(1000);
    private readonly Guid _intersectionObserverMapKey = Guid.NewGuid();

    public string CursorStyleCss => GetCursorStyleCss();
    public string CaretRowStyleCss => GetCaretRowStyleCss();
    public string BlinkAnimationCssClass => _hasBlinkAnimation
        ? "bte_blink"
        : string.Empty;

    public string TextEditorCursorDisplayId => $"bte_text-editor-cursor-display_{_intersectionObserverMapKey}";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsFocusTarget)
        {
            await JsRuntime.InvokeVoidAsync(
                "blazorTextEditor.initializeTextEditorCursorIntersectionObserver",
                _intersectionObserverMapKey.ToString(),
                ScrollableContainerId,
                TextEditorCursorDisplayId);
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private string GetCursorStyleCss()
    {
        var leftInPixels = 0d;
        
        // Gutter padding column offset
        {
            leftInPixels += 
                (TextEditorBase.GutterPaddingLeftInPixels + TextEditorBase.GutterPaddingRightInPixels);
        }
        
        // Tab key column offset
        {
            var tabsOnSameRowBeforeCursor = TextEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    TextEditorCursor.IndexCoordinates.rowIndex, 
                    TextEditorCursor.IndexCoordinates.columnIndex);
            
            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TabWidth - 1;
            
            leftInPixels += (extraWidthPerTabKey * tabsOnSameRowBeforeCursor * FontWidthAndElementHeight.FontWidthInPixels);
        }
        
        // Line number column offset
        {
            var mostDigitsInARowLineNumber = TextEditor.RowCount
                .ToString()
                .Length;

            leftInPixels += mostDigitsInARowLineNumber * FontWidthAndElementHeight.FontWidthInPixels;
        }

        leftInPixels += FontWidthAndElementHeight.FontWidthInPixels * TextEditorCursor.IndexCoordinates.columnIndex;
        
        var left = $"left: {leftInPixels}px;";
        var top = $"top: {FontWidthAndElementHeight.ElementHeightInPixels * TextEditorCursor.IndexCoordinates.rowIndex}px;";
        var height = $"height: {FontWidthAndElementHeight.ElementHeightInPixels}px;";

        return $"{left} {top} {height}";
    }

    private string GetCaretRowStyleCss()
    {
        var top = $"top: {FontWidthAndElementHeight.ElementHeightInPixels * TextEditorCursor.IndexCoordinates.rowIndex}px;";
        var height = $"height: {FontWidthAndElementHeight.ElementHeightInPixels}px;";

        return $"{top} {height}";
    }

    public async Task FocusAsync()
    {
        if (_textEditorCursorDisplayElementReference is not null)
            await _textEditorCursorDisplayElementReference.Value.FocusAsync();
    }

    public void PauseBlinkAnimation()
    {
        _hasBlinkAnimation = false;

        var cancellationToken = CancelSourceAndCreateNewThenReturnToken();

        _ = Task.Run(async () =>
        {
            await Task.Delay(_blinkingCursorTaskDelay, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                _hasBlinkAnimation = true;
                await InvokeAsync(StateHasChanged);    
            }
        }, cancellationToken);
    }
    
    private void HandleOnKeyDown()
    {
        PauseBlinkAnimation();
    }

    private CancellationToken CancelSourceAndCreateNewThenReturnToken()
    {
        _blinkingCursorCancellationTokenSource.Cancel();
        _blinkingCursorCancellationTokenSource = new();

        return _blinkingCursorCancellationTokenSource.Token;
    }
    
    public void Dispose()
    {
        _blinkingCursorCancellationTokenSource.Cancel();

        if (IsFocusTarget)
        {
            _ = Task.Run(async () =>
            {
                await JsRuntime.InvokeVoidAsync(
                    "blazorTextEditor.disposeTextEditorCursorIntersectionObserver",
                    _intersectionObserverMapKey.ToString());
            });
        }
    }
}