using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.Dimensions;
using BlazorTextEditor.ClassLib.FileSystem.Classes;
using BlazorTextEditor.ClassLib.Store.FolderExplorerCase;
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
    private ImmutableArray<AbsoluteFilePath> _absoluteFilePaths = ImmutableArray<AbsoluteFilePath>.Empty;

    protected override void OnInitialized()
    {
        FolderExplorerStateWrap.StateChanged += FolderExplorerStateWrapOnStateChanged;
        
        base.OnInitialized();
    }

    private void FolderExplorerStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        if (FolderExplorerStateWrap.Value.AbsoluteFilePath is null)
            return;
        
        var absoluteFilePathString = 
            FolderExplorerStateWrap.Value.AbsoluteFilePath.GetAbsoluteFilePathString();

        var childFiles = Directory
            .GetFiles(absoluteFilePathString)
            .Select(cf => new AbsoluteFilePath(cf, false));
        
        var childDirectories = Directory
            .GetDirectories(absoluteFilePathString)
            .Select(cd => new AbsoluteFilePath(cd, true));

        _absoluteFilePaths = childDirectories
            .Union(childFiles)
            .ToImmutableArray();
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