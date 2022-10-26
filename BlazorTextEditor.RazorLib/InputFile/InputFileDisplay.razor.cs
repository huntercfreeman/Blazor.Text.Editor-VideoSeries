using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.FileSystem.Classes;
using BlazorTextEditor.ClassLib.FileSystem.Interfaces;
using BlazorTextEditor.ClassLib.Menu;
using BlazorTextEditor.ClassLib.Store.TextEditorCase;
using BlazorTextEditor.ClassLib.TextEditor;
using BlazorTextEditor.ClassLib.TreeView;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.InputFile;

public partial class InputFileDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    private TreeViewModel<IAbsoluteFilePath>? _rootTreeViewModel;
    
    protected override void OnInitialized()
    {
        var homeAbsoluteFilePath = new AbsoluteFilePath(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            true);
        
        _rootTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            homeAbsoluteFilePath, 
            LoadChildrenAsync);

        _rootTreeViewModel.LoadChildrenFuncAsync.Invoke(_rootTreeViewModel);
        
        base.OnInitialized();
    }

    private Task LoadChildrenAsync(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var absoluteFilePathString = treeViewModel.Item.GetAbsoluteFilePathString();

        var childFiles = Directory
            .GetFiles(absoluteFilePathString)
            .OrderBy(filename => filename)
            .Select(cf => new AbsoluteFilePath(cf, false));
        
        var childDirectories = Directory
            .GetDirectories(absoluteFilePathString)
            .OrderBy(filename => filename)
            .Select(cd => new AbsoluteFilePath(cd, true));

        var childTreeViewModels = childDirectories
            .Union(childFiles)
            .Select(afp => new TreeViewModel<IAbsoluteFilePath>(afp, LoadChildrenAsync));

        treeViewModel.Children.Clear();
        treeViewModel.Children.AddRange(childTreeViewModels);
        
        return Task.CompletedTask;
    }

    private MenuRecord GetContextMenu(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        var openInTextEditorMenuOption = new MenuOptionRecord(
            "Open in Text Editor",
            () => OpenInTextEditor(treeViewModel));

        return new MenuRecord(new []
        {
            openInTextEditorMenuOption
        }.ToImmutableArray());
    }

    private void OpenInTextEditor(TreeViewModel<IAbsoluteFilePath> treeViewModel)
    {
        _ = Task.Run(async () =>
        {
            var fileContents = await File
                .ReadAllTextAsync(treeViewModel.Item.GetAbsoluteFilePathString());

            var textEditor = new TextEditorBase(fileContents);
            
            Dispatcher.Dispatch(new RegisterTextEditorStateAction(textEditor));
        });
    }
}