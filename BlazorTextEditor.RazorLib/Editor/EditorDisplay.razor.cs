using BlazorTextEditor.ClassLib.Dimensions;
using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Editor;

public partial class EditorDisplay : ComponentBase
{
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;
}