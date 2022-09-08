using System.Text;
using BlazorTextEditor.ClassLib.Keyboard;
using BlazorTextEditor.ClassLib.RoslynHelpers;
using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using BlazorTextEditor.ClassLib.TextEditor;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorTextEditor.RazorLib.TextEditor;

public partial class TextEditorDisplay : ComponentBase
{
    [Inject]
    private IStateSelection<TextEditorStates, TextEditorBase> TextEditorStatesSelection { get; set; } = null!;

    [Parameter]
    public TextEditorKey TextEditorKey { get; set; } = null!;
    
    private ElementReference _textEditorDisplayElementReference;
    private List<List<RichCharacter>>? _rows;

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
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task FocusTextEditorOnClickAsync()
    {
        await _textEditorDisplayElementReference.FocusAsync();
    }
    
    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key.Length > 1)
            return;

        TextEditorStatesSelection.Value.Content.Add(new RichCharacter()
        {
            Value = keyboardEventArgs.Key.First(),
            DecorationByte = default
        });
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
}