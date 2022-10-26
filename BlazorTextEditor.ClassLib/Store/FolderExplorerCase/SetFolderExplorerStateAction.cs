using BlazorTextEditor.ClassLib.FileSystem.Interfaces;

namespace BlazorTextEditor.ClassLib.Store.FolderExplorerCase;

public record SetFolderExplorerStateAction(IAbsoluteFilePath? AbsoluteFilePath);