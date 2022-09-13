using System.Collections.Immutable;
using System.Text;
using BlazorTextEditor.ClassLib.Keyboard;
using BlazorTextEditor.ClassLib.RoslynHelpers;
using BlazorTextEditor.ClassLib.Store.TextEditorCase;
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

    private string TextEditorContentId => $"bte_text-editor-content_{_textEditorGuid}";
    private string MeasureCharacterWidthAndRowHeightId => $"bte_measure-character-width-and-row-height_{_textEditorGuid}";
    
    protected override void OnInitialized()
    {
        TextEditorStatesSelection
            .Select(textEditorStates => textEditorStates.TextEditorMap[TextEditorKey]);
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
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
        var localTextEditor = TextEditorStatesSelection.Value;
        
        _relativeCoordinatesOnClick = await JsRuntime.InvokeAsync<RelativeCoordinates>(
            "blazorTextEditor.getRelativePosition",
            TextEditorContentId,
            mouseEventArgs.ClientX,
            mouseEventArgs.ClientY);

        if (_characterWidthAndRowHeight is null)
            return;

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
        
        _textEditorCursor.IndexCoordinates = (rowIndex, columnIndexInt);
        _textEditorCursor.PreferredColumnIndex = columnIndexInt;
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
            if (KeyboardKeyFacts.IsMetaKey(keyboardEventArgs))
                return;

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
    
    private async Task ApplyRoslynSyntaxHighlightingOnClick()
    {
        var localTextEditor = TextEditorStatesSelection.Value;
        
        var stringContent = TextEditorStatesSelection.Value.GetAllText();

        var syntaxTree = CSharpSyntaxTree.ParseText(stringContent);

        var syntaxNodeRoot = await syntaxTree.GetRootAsync();

        var generalSyntaxCollector = new GeneralSyntaxCollector();

        generalSyntaxCollector.Visit(syntaxNodeRoot);

        var methodIdentifiers = generalSyntaxCollector.MethodDeclarationSyntaxes
            .Select(mds => mds.Identifier)
            .ToList();

        var method = methodIdentifiers.First();

        localTextEditor.ApplyDecorationRange(DecorationKind.Method,
            methodIdentifiers.Select(x => x.Span));
    }

    private string GetCssClass(byte currentDecorationByte)
    {
        if (currentDecorationByte == 0)
        {
            return string.Empty;
        }
        else
        {
            return "bte_method";
        }
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
}










