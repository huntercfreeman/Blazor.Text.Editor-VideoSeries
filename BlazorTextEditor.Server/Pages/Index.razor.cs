using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.Server.Pages;

public partial class Index : FluxorComponent
{
    [Inject]
    private IState<TextEditorStates> TextEditorStatesWrap { get; set; } = null!;
}