using BlazorTextEditor.ClassLib.FileSystem.Interfaces;
using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.FolderExplorerCase;

[FeatureState]
public record FolderExplorerState(IAbsoluteFilePath? AbsoluteFilePath)
{
    public FolderExplorerState() : this(default(IAbsoluteFilePath))
    {
        
    }
}