using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.ClassLib.Keyboard;
using BlazorTextEditor.ClassLib.RoslynHelpers;
using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using BlazorTextEditor.ClassLib.SyntaxHighlighting;
using BlazorTextEditor.ClassLib.TextEditor;
using BlazorTextEditor.RazorLib.JavaScriptObjects;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.JSInterop;

namespace BlazorTextEditor.RazorLib.TextEditor;

public partial class TextEditorDisplay : ComponentBase
{
    [Inject]
    private IStateSelection<TextEditorStates, TextEditorBase> TextEditorStatesSelection { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Parameter]
    public TextEditorKey TextEditorKey { get; set; } = null!;

    private Guid _textEditorGuid = Guid.NewGuid();
    private ElementReference _textEditorDisplayElementReference;
    private List<List<RichCharacter>>? _rows;
    private bool _shouldMeasureDimensions = true;
    private string _testStringForMeasurement = "abcdefghijklmnopqrstuvwxyz0123456789";
    private int _testStringRepeatCount = 6;
    private FontWidthAndElementHeight? _characterWidthAndRowHeight;
    private WidthAndHeightOfElement? _textEditorWidthAndHeight;
    private RelativeCoordinates? _relativeCoordinatesOnClick;
    private TextEditorCursor _textEditorCursor = new();
    private TextEditorCursorDisplay? _textEditorCursorDisplay;
    private bool _showNewlines = true;
    private bool _showWhitespace = true;
    private bool _showGetAllTextEscaped;
    /// <summary>
    /// Do not select text just because the user has the Left Mouse Button down.
    /// They might hold down Left Mouse Button from outside the TextEditorDisplay's content div
    /// then move their mouse over the content div while holding the Left Mouse Button down.
    /// <br/><br/>
    /// Instead only select text if an @onmousedown event triggered <see cref="_thinksLeftMouseButtonIsDown"/>
    /// to be equal to true and the @onmousemove event followed afterwards.
    /// </summary>
    private bool _thinksLeftMouseButtonIsDown;

    private TextEditorKey? _previousTextEditorKey;

    private DateTime _onInitializedDateTime;
    private DateTime _onAfterFirstRenderDateTime;

    private TimeSpan TimeToFirstRender => _onAfterFirstRenderDateTime
        .Subtract(_onInitializedDateTime);
    
    private string TextEditorContentId => $"bte_text-editor-content_{_textEditorGuid}";
    private string MeasureCharacterWidthAndRowHeightId => $"bte_measure-character-width-and-row-height_{_textEditorGuid}";
    private MarkupString GetAllTextEscaped => (MarkupString) TextEditorStatesSelection.Value
        .GetAllText()
        .Replace("\r\n", "\\r\\n<br/>")
        .Replace("\r", "\\r<br/>")
        .Replace("\n", "\\n<br/>")
        .Replace("\t", "--->")
        .Replace(" ", "·");

    protected override async Task OnParametersSetAsync()
    {
        if (_previousTextEditorKey is null ||
            _previousTextEditorKey != TextEditorKey)
        {
            await GetRowsAsync();
        }
        
        await base.OnParametersSetAsync();
    }

    protected override void OnInitialized()
    {
        _onInitializedDateTime = DateTime.UtcNow;
        
        TextEditorStatesSelection
            .Select(textEditorStates => textEditorStates.TextEditorList
                .Single(x => x.Key == TextEditorKey));
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _onAfterFirstRenderDateTime = DateTime.UtcNow;
            
            await GetRowsAsync();
        }

        if (_shouldMeasureDimensions)
        {
            _characterWidthAndRowHeight = await JsRuntime.InvokeAsync<FontWidthAndElementHeight>(
                "blazorTextEditor.measureFontWidthAndElementHeightByElementId",
                MeasureCharacterWidthAndRowHeightId,
                _testStringRepeatCount * _testStringForMeasurement.Length);
            
            _textEditorWidthAndHeight = await JsRuntime.InvokeAsync<WidthAndHeightOfElement>(
                "blazorTextEditor.measureWidthAndHeightByElementId",
                TextEditorContentId);

            {
                _shouldMeasureDimensions = false;
                await InvokeAsync(StateHasChanged);
            }
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task GetRowsAsync()
    {
        var localTextEditor = TextEditorStatesSelection.Value;
            
        _rows = localTextEditor.GetRows(0, Int32.MaxValue);

        await InvokeAsync(StateHasChanged);
    }

    private async Task FocusTextEditorOnClickAsync()
    {
        if (_textEditorCursorDisplay is not null) 
            await _textEditorCursorDisplay.FocusAsync();
    }
    
    private async Task HandleContentOnClickAsync(MouseEventArgs mouseEventArgs)
    {
        var rowAndColumnIndex = await DetermineRowAndColumnIndex(mouseEventArgs);
        
        _textEditorCursor.IndexCoordinates = (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
        _textEditorCursor.PreferredColumnIndex = rowAndColumnIndex.columnIndex;
    }
    
    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
        if (KeyboardKeyFacts.IsMovementKey(keyboardEventArgs.Key))
        {
            TextEditorCursor.MoveCursor(
                keyboardEventArgs, 
                _textEditorCursor, 
                TextEditorStatesSelection.Value);
        }
        else
        {
            Dispatcher.Dispatch(new EditTextEditorAction(TextEditorKey,
                new (ImmutableTextEditorCursor, TextEditorCursor)[]
                {
                    (new ImmutableTextEditorCursor(_textEditorCursor), _textEditorCursor)
                }.ToImmutableArray(),
                keyboardEventArgs,
                CancellationToken.None));
            
            await GetRowsAsync();
        }
    }
    
    private async Task HandleContentOnMouseDownAsync(MouseEventArgs mouseEventArgs)
    {
        var rowAndColumnIndex = await DetermineRowAndColumnIndex(mouseEventArgs);

        _textEditorCursor.IndexCoordinates = (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
        _textEditorCursor.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

        _textEditorCursorDisplay?.PauseBlinkAnimation();

        var cursorPositionIndex = TextEditorStatesSelection.Value.GetCursorPositionIndex(new TextEditorCursor(rowAndColumnIndex));
        
        _textEditorCursor.TextEditorSelection.AnchorPositionIndex = cursorPositionIndex;

        _textEditorCursor.TextEditorSelection.EndingPositionIndex = cursorPositionIndex;
        
        _thinksLeftMouseButtonIsDown = true;
    }
    
    /// <summary>
    /// OnMouseUp is unnecessary
    /// </summary>
    /// <param name="mouseEventArgs"></param>
    private async Task HandleContentOnMouseMoveAsync(MouseEventArgs mouseEventArgs)
    {
        // Buttons is a bit flag
        // '& 1' gets if left mouse button is held
        if (_thinksLeftMouseButtonIsDown && 
            (mouseEventArgs.Buttons & 1) == 1)
        {
            var rowAndColumnIndex = await DetermineRowAndColumnIndex(mouseEventArgs);
            
            _textEditorCursor.IndexCoordinates = (rowAndColumnIndex.rowIndex, rowAndColumnIndex.columnIndex);
            _textEditorCursor.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

            _textEditorCursorDisplay?.PauseBlinkAnimation();
            
            _textEditorCursor.TextEditorSelection.EndingPositionIndex =
                TextEditorStatesSelection.Value.GetCursorPositionIndex(new TextEditorCursor(rowAndColumnIndex));
        }
        else
        {
            _thinksLeftMouseButtonIsDown = false;
        }
    }
    
    private async Task ApplyRoslynSyntaxHighlightingOnClick()
    {
        var localTextEditor = TextEditorStatesSelection.Value;

        await CSharpRolsynSyntaxHighting.ApplyRoslynSyntaxHighlightingAsync(localTextEditor);
    }
    
    private async Task<(int rowIndex, int columnIndex)> DetermineRowAndColumnIndex(MouseEventArgs mouseEventArgs)
    {
        var localTextEditor = TextEditorStatesSelection.Value;
        
        _relativeCoordinatesOnClick = await JsRuntime.InvokeAsync<RelativeCoordinates>(
            "blazorTextEditor.getRelativePosition",
            TextEditorContentId,
            mouseEventArgs.ClientX,
            mouseEventArgs.ClientY);

        if (_characterWidthAndRowHeight is null)
            return (0, 0);

        // Gutter padding column offset
        {
            _relativeCoordinatesOnClick.RelativeX -= 
                (TextEditorBase.GutterPaddingLeftInPixels + TextEditorBase.GutterPaddingRightInPixels);
        }
        
        var columnIndexDouble = _relativeCoordinatesOnClick.RelativeX / _characterWidthAndRowHeight.FontWidthInPixels;

        var columnIndexInt = (int)Math.Round(columnIndexDouble, MidpointRounding.AwayFromZero);
        
        var rowIndex = (int) (_relativeCoordinatesOnClick.RelativeY / _characterWidthAndRowHeight.ElementHeightInPixels);

        rowIndex = rowIndex > localTextEditor.RowCount - 1
            ? localTextEditor.RowCount - 1
            : rowIndex;

        var lengthOfRow = localTextEditor.GetLengthOfRow(rowIndex);

        // Tab key column offset
        {
            var parameterForGetTabsCountOnSameRowBeforeCursor = columnIndexInt > lengthOfRow
                ? lengthOfRow
                : columnIndexInt;

            var tabsOnSameRowBeforeCursor = localTextEditor
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex, 
                    parameterForGetTabsCountOnSameRowBeforeCursor);
            
            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TabWidth - 1;
            
            columnIndexInt -= (extraWidthPerTabKey * tabsOnSameRowBeforeCursor);
        }
        
        // Line number column offset
        {
            var mostDigitsInARowLineNumber = TextEditorStatesSelection.Value.RowCount
                .ToString()
                .Length;

            columnIndexInt -= mostDigitsInARowLineNumber;
        }
        
        columnIndexInt = columnIndexInt > lengthOfRow
            ? lengthOfRow
            : columnIndexInt;

        rowIndex = Math.Max(rowIndex, 0);
        columnIndexInt = Math.Max(columnIndexInt, 0);
        
        return (rowIndex, columnIndexInt);
    }

    private string GetCssClass(byte currentDecorationByte)
    {
        var decoration = (DecorationKind)currentDecorationByte;

        return decoration switch
        {
            DecorationKind.None => string.Empty,
            DecorationKind.Method => "bte_method",
            DecorationKind.Type => "bte_type",
            DecorationKind.Parameter => "bte_parameter",
            DecorationKind.StringLiteral => "bte_string-literal",
            DecorationKind.Keyword => "bte_keyword",
            DecorationKind.Comment => "bte_comment",
                _ => throw new ApplicationException(
                    $"The {nameof(DecorationKind)}: {decoration} was not recognized.")
        };
    }

    private string GetRowStyleCss(int index)
    {
        if (_characterWidthAndRowHeight is null)
            return string.Empty;
        
        var top = $"top: {index * _characterWidthAndRowHeight.ElementHeightInPixels}px;";
        var height = $"height: {_characterWidthAndRowHeight.ElementHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditorStatesSelection.Value.RowCount
            .ToString()
            .Length;
        
        var widthOfGutterInPixels = mostDigitsInARowLineNumber * _characterWidthAndRowHeight.FontWidthInPixels;
        var left = $"left: {widthOfGutterInPixels + TextEditorBase.GutterPaddingLeftInPixels + TextEditorBase.GutterPaddingRightInPixels}px;";
        
        return $"{top} {height} {left}";
    }

    private string GetGutterStyleCss(int index)
    {
        if (_characterWidthAndRowHeight is null)
            return string.Empty;
        
        var top = $"top: {index * _characterWidthAndRowHeight.ElementHeightInPixels}px;";
        var height = $"height: {_characterWidthAndRowHeight.ElementHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditorStatesSelection.Value.RowCount
            .ToString()
            .Length;

        var widthInPixels = mostDigitsInARowLineNumber * _characterWidthAndRowHeight.FontWidthInPixels;

        widthInPixels += TextEditorBase.GutterPaddingLeftInPixels + TextEditorBase.GutterPaddingRightInPixels;
        
        var width = $"width: {widthInPixels}px;";

        var paddingLeft = $"padding-left: {TextEditorBase.GutterPaddingLeftInPixels}px;";
        var paddingRight = $"padding-right: {TextEditorBase.GutterPaddingRightInPixels}px;";
        
        return $"{top} {height} {width} {paddingLeft} {paddingRight}";
    }

    private string GetTextSelectionStyleCss(int lowerBound, int upperBound, int rowIndex)
    {
        if (_characterWidthAndRowHeight is null ||
            rowIndex >= TextEditorStatesSelection.Value.RowEndingPositions.Length)
        {
            return string.Empty;
        }
        
        var startOfRowTuple = TextEditorStatesSelection.Value.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = TextEditorStatesSelection.Value.RowEndingPositions[rowIndex];

        var selectionStartingColumnIndex = 0;
        var selectionEndingColumnIndex = endOfRowTuple.positionIndex 
                                         - 1;

        var fullWidthOfRowIsSelected = true;
        
        if (lowerBound > startOfRowTuple.positionIndex)
        {
            selectionStartingColumnIndex = lowerBound
                                           - startOfRowTuple.positionIndex;

            fullWidthOfRowIsSelected = false;
        }

        if (upperBound < endOfRowTuple.positionIndex)
        {
            selectionEndingColumnIndex = upperBound 
                                         - startOfRowTuple.positionIndex;
            
            fullWidthOfRowIsSelected = false;
        }
        
        var top = $"top: {rowIndex * _characterWidthAndRowHeight.ElementHeightInPixels}px;";
        var height = $"height: {_characterWidthAndRowHeight.ElementHeightInPixels}px;";

        var mostDigitsInARowLineNumber = TextEditorStatesSelection.Value.RowCount
            .ToString()
            .Length;
        
        var widthOfGutterInPixels = mostDigitsInARowLineNumber * _characterWidthAndRowHeight.FontWidthInPixels;

        var gutterSizeInPixels = widthOfGutterInPixels 
                + TextEditorBase.GutterPaddingLeftInPixels 
                + TextEditorBase.GutterPaddingRightInPixels;
        
        var selectionStartInPixels = selectionStartingColumnIndex 
                                     * _characterWidthAndRowHeight.FontWidthInPixels;
        
        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorStatesSelection.Value
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex, 
                    selectionStartingColumnIndex);
            
            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TabWidth - 1;
            
            selectionStartInPixels += (extraWidthPerTabKey * tabsOnSameRowBeforeCursor * _characterWidthAndRowHeight.FontWidthInPixels);    
        }
        
        var left = $"left: {gutterSizeInPixels + selectionStartInPixels}px;";

        var selectionWidthInPixels = selectionEndingColumnIndex 
                                     * _characterWidthAndRowHeight.FontWidthInPixels
                                     - selectionStartInPixels;
        
        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = TextEditorStatesSelection.Value
                .GetTabsCountOnSameRowBeforeCursor(
                    rowIndex, 
                    selectionEndingColumnIndex);
            
            // 1 of the character width is already accounted for

            var extraWidthPerTabKey = TextEditorBase.TabWidth - 1;
            
            selectionWidthInPixels += (extraWidthPerTabKey * tabsOnSameRowBeforeCursor * _characterWidthAndRowHeight.FontWidthInPixels);    
        }
        
        var widthCssStyleString = "width: ";

        if (fullWidthOfRowIsSelected)
            widthCssStyleString += "100%";
        else if (selectionStartingColumnIndex != 0 && upperBound > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc(100% - {selectionStartInPixels}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixels}px;";
        
        return $"{top} {height} {left} {widthCssStyleString}";
    }

    private void AppendTextEscaped(
        StringBuilder spanBuilder,
        RichCharacter richCharacter,
        string tabKeyOutput,
        string spaceKeyOutput)
    {
        switch (richCharacter.Value)
        {
            case '\t':
                spanBuilder.Append(tabKeyOutput);
                break;
            case ' ':
                spanBuilder.Append(spaceKeyOutput);
                break;
            case '\r':
                break;
            case '\n':
                break;
            case '<':
                spanBuilder.Append("&lt;");
                break;
            case '>':
                spanBuilder.Append("&gt;");
                break;
            case '"':
                spanBuilder.Append("&quot;");
                break;
            case '\'':
                spanBuilder.Append("&#39;");
                break;
            case '&':
                spanBuilder.Append("&amp;");
                break;
            default:
                spanBuilder.Append(richCharacter.Value);
                break;
        }
    }
}










