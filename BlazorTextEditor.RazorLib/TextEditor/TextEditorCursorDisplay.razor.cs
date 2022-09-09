using BlazorTextEditor.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditor;

public partial class TextEditorCursorDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TextEditorCursor TextEditorCursor { get; set; } = null!;
    [Parameter, EditorRequired]
    public FontWidthAndElementHeight FontWidthAndElementHeight { get; set; } = null!;

    public string StyleCss => GetStyleCss();

    private string GetStyleCss()
    {
        var left = $"left: {FontWidthAndElementHeight.FontWidthInPixels * TextEditorCursor.IndexCoordinates.columnIndex}px;";
        var top = $"top: {FontWidthAndElementHeight.ElementHeightInPixels * TextEditorCursor.IndexCoordinates.rowIndex}px;";

        return $"{left} {top}";
    }
}