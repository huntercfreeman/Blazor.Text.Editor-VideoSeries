using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using BlazorTextEditor.ClassLib.TextEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Server.Pages;

public partial class Index : FluxorComponent, IDisposable
{
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    private TextEditorBase _textEditorBaseOne = new();
    private TextEditorBase _textEditorBaseTwo = new();
    private TextEditorBase _textEditorBaseThree = new();

    private bool _isLoaded;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Dispatcher.Dispatch(new RegisterTextEditorStateAction(TextEditorFacts.TextEditorKeyOne, _textEditorBaseOne));
            Dispatcher.Dispatch(new RegisterTextEditorStateAction(TextEditorFacts.TextEditorKeyTwo, _textEditorBaseTwo));
            Dispatcher.Dispatch(new RegisterTextEditorStateAction(TextEditorFacts.TextEditorKeyThree, _textEditorBaseThree));
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    public void Dispose()
    {
        // Dispatcher.Dispatch(new DisposeTextEditorStateAction(_textEditorKeyOne, _textEditorBaseOne));
        // Dispatcher.Dispatch(new DisposeTextEditorStateAction(_textEditorKeyTwo, _textEditorBaseTwo));
        // Dispatcher.Dispatch(new DisposeTextEditorStateAction(_textEditorKeyThree, _textEditorBaseThree));
    }
}