using BlazorTextEditor.ClassLib.Store.DialogCase;
using BlazorTextEditor.RazorLib.DialogCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private string _message = string.Empty;

    private void OpenDialogOnClick()
    {
        Dispatcher.Dispatch(new RegisterDialogRecordAction(new DialogRecord(
            DialogKey.NewDialogKey(), 
            "Example",
            typeof(ExampleDialog),
            new Dictionary<string, object?>
            {
                { nameof(ExampleDialog.Message), _message }
            })));
    }
}