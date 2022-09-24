using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.Menu;
using BlazorTextEditor.ClassLib.Store.DialogCase;
using BlazorTextEditor.ClassLib.Store.DropdownCase;
using BlazorTextEditor.RazorLib.InputFile;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.Shared;

public partial class BlazorTextEditorHeader : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    private DropdownKey _dropdownKeyFileDropdown = DropdownKey.NewDropdownKey();
    private MenuRecord _fileMenu = new(ImmutableArray<MenuOptionRecord>.Empty);

    protected override Task OnInitializedAsync()
    {
        _fileMenu = new MenuRecord(
            new []
            {
                GetMenuOptionNew(),
                GetMenuOptionOpen()
            }.ToImmutableArray());
        
        return base.OnInitializedAsync();
    }

    private MenuOptionRecord GetMenuOptionNew()
    {
        return new MenuOptionRecord(
            "New",
            () => {});
    }
    
    private MenuOptionRecord GetMenuOptionOpen()
    {
        var openFile = new MenuOptionRecord(
            "File",
            ShowInputFileDialog);
        
        var openCSharpProject = new MenuOptionRecord(
            "C# Project",
            () => {});
        
        var openDotNetSolution = new MenuOptionRecord(
            ".NET Solution",
            () => {});

        return new MenuOptionRecord(
            "Open",
            () => {},
            new MenuRecord(
                new []
                {
                    openFile,
                    openCSharpProject,
                    openDotNetSolution
                }.ToImmutableArray()));
    }

    private void AddActiveFileDropdown()
    {
        Dispatcher.Dispatch(new AddActiveDropdownKeyAction(_dropdownKeyFileDropdown));
    }

    private void ShowInputFileDialog()
    {
        Dispatcher.Dispatch(new RegisterDialogRecordAction(new DialogRecord(
            DialogKey.NewDialogKey(), 
            "Input File",
            typeof(InputFileDisplay),
            null)));
    }
}