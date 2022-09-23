using BlazorTextEditor.ClassLib.Store.DropdownCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Shared;

public partial class BlazorTextEditorHeader : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    private DropdownKey _dropdownKeyVerticalDropdown = DropdownKey.NewDropdownKey();
    private DropdownKey _dropdownKeyHorizontalDropdown = DropdownKey.NewDropdownKey();

    private void AddActiveVerticalDropdown()
    {
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_dropdownKeyVerticalDropdown));
    }
    private void AddActiveHorizontalDropdown()
    {
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_dropdownKeyHorizontalDropdown));
    }
}