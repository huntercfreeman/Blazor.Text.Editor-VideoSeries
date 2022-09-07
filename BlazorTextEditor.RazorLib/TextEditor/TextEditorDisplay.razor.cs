using System.Text;
using BlazorTextEditor.ClassLib.Keyboard;
using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using BlazorTextEditor.ClassLib.TextEditor;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorTextEditor.RazorLib.TextEditor;

public partial class TextEditorDisplay : ComponentBase
{
    [Inject]
    private IStateSelection<TextEditorStates, TextEditorBase> TextEditorStatesSelection { get; set; } = null!;

    [Parameter]
    public TextEditorKey TextEditorKey { get; set; } = null!;
    
    private ElementReference _textEditorDisplayElementReference;

    protected override void OnInitialized()
    {
        TextEditorStatesSelection
            .Select(textEditorStates => textEditorStates.TextEditorMap[TextEditorKey]);
        
        base.OnInitialized();
    }

    private async Task FocusTextEditorOnClickAsync()
    {
        await _textEditorDisplayElementReference.FocusAsync();
    }
    
    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key.Length > 1)
            return;

        TextEditorStatesSelection.Value.Content.Append(keyboardEventArgs.Key);
    }
}