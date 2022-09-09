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
            var localTextEditor = TextEditorStatesSelection.Value;
            
            _rows = localTextEditor.GetRows(0, Int32.MaxValue);

            await InvokeAsync(StateHasChanged);
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

    private async Task FocusTextEditorOnClickAsync()
    {
        await _textEditorDisplayElementReference.FocusAsync();
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
        
        var columnIndexDouble = _relativeCoordinatesOnClick.RelativeX / _characterWidthAndRowHeight.FontWidthInPixels;

        var columnIndexInt = (int)Math.Round(columnIndexDouble, MidpointRounding.AwayFromZero);
        
        var rowIndex = (int) (_relativeCoordinatesOnClick.RelativeY / _characterWidthAndRowHeight.ElementHeightInPixels);

        rowIndex = rowIndex > localTextEditor.RowCount - 1
            ? localTextEditor.RowCount - 1
            : rowIndex;

        var lengthOfRow = localTextEditor.GetLengthOfRow(rowIndex);

        columnIndexInt = columnIndexInt > lengthOfRow - 1
            ? lengthOfRow - 1
            : columnIndexInt;
        
        // TODO: handle tab keys being a varying width from characters

        _textEditorCursor.IndexCoordinates = (rowIndex, columnIndexInt);
    }
    
    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
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
            if (keyboardEventArgs.Key.Length > 1)
                return;

            TextEditorStatesSelection.Value.Content.Add(new RichCharacter()
            {
                Value = keyboardEventArgs.Key.First(),
                DecorationByte = default
            });
        }
    }
    
    private async Task ApplyRoslynSyntaxHighlightingOnClick()
    {
        var stringContent = new string(TextEditorStatesSelection.Value.Content
            .Select(rc => rc.Value)
            .ToArray());

        var syntaxTree = CSharpSyntaxTree.ParseText(stringContent);

        var syntaxNodeRoot = await syntaxTree.GetRootAsync();

        var generalSyntaxCollector = new GeneralSyntaxCollector();

        generalSyntaxCollector.Visit(syntaxNodeRoot);

        var methodIdentifiers = generalSyntaxCollector.MethodDeclarationSyntaxes
            .Select(mds => mds.Identifier)
            .ToList();

        var method = methodIdentifiers.First();

        var correspondingRichCharacters = TextEditorStatesSelection.Value.Content
            .Skip(method.Span.Start)
            .Take(method.Span.Length)
            .ToList();

        foreach (var richCharacter in correspondingRichCharacters)
        {
            richCharacter.DecorationByte = 1;
        }
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
        var left = $"left: {widthOfGutterInPixels}px;";
        
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
        var width = $"width: {widthInPixels}px;";
        
        return $"{top} {height} {width}";
    }
}










