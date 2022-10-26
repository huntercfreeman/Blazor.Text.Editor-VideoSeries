using BlazorTextEditor.ClassLib.Menu;
using BlazorTextEditor.ClassLib.Store.DropdownCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Menu;

public partial class MenuOptionDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public MenuOptionRecord MenuOptionRecord { get; set; } = null!;
    
    private readonly DropdownKey _subMenuDropdownKey = DropdownKey.NewDropdownKey();

    private void HandleOnClick()
    {
        if (MenuOptionRecord.OnClick is not null)
            MenuOptionRecord.OnClick.Invoke();
        
        if (MenuOptionRecord.SubMenu is not null)
            Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_subMenuDropdownKey));
    }
}