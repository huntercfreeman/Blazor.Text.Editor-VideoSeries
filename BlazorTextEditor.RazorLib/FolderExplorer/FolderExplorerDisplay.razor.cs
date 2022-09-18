using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.Dimensions;
using BlazorTextEditor.ClassLib.FileSystem.Classes;
using BlazorTextEditor.ClassLib.FileSystem.Interfaces;
using BlazorTextEditor.ClassLib.Store.FolderExplorerCase;
using BlazorTextEditor.ClassLib.TreeView;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorTextEditor.RazorLib.FolderExplorer;

public partial class FolderExplorerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<FolderExplorerState> FolderExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public ElementDimensions FolderExplorerElementDimensions { get; set; } = null!;

    private string _filePath = string.Empty;
    private TreeViewModel<IAbsoluteFilePath>? _rootTreeViewModel;

    protected override void OnInitialized()
    {
        FolderExplorerStateWrap.StateChanged += FolderExplorerStateWrapOnStateChanged;
        
        base.OnInitialized();
    }

    private void FolderExplorerStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (FolderExplorerStateWrap.Value.AbsoluteFilePath is null)
            return;

        _rootTreeViewModel = new TreeViewModel<IAbsoluteFilePath>(
            FolderExplorerStateWrap.Value.AbsoluteFilePath, 
            LoadChildrenAsync);

        _rootTreeViewModel.LoadChildrenFuncAsync.Invoke(_rootTreeViewModel);
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
    
    private void DispatchSetFolderExplorerStateOnClick()
    {
        if (!Directory.Exists(_filePath))
            throw new DirectoryNotFoundException();
        
        Dispatcher.Dispatch(
            new SetFolderExplorerStateAction(
                new AbsoluteFilePath(_filePath, true)));
    }

    public void Dispose()
    {
        FolderExplorerStateWrap.StateChanged -= FolderExplorerStateWrapOnStateChanged;
    }
}