using BlazorTextEditor.ClassLib.Store.DialogCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.DialogCase;

public partial class DialogInitializer : FluxorComponent
{
    [Inject]
    private IState<DialogStates> DialogStatesWrap { get; set; } = null!;
}