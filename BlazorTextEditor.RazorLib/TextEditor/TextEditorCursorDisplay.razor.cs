using BlazorTextEditor.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.TextEditor;

public partial class TextEditorCursorDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public TextEditorBase TextEditor { get; set; } = null!;
    [Parameter, EditorRequired]
    public TextEditorCursor TextEditorCursor { get; set; } = null!;
    [Parameter, EditorRequired]
    public FontWidthAndElementHeight FontWidthAndElementHeight { get; set; } = null!;

    public string CursorStyleCss => GetCursorStyleCss();
    public string CaretRowStyleCss => GetCaretRowStyleCss();

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
}