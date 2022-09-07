using BlazorTextEditor.ClassLib.FileSystem.Interfaces;

namespace BlazorTextEditor.ClassLib.FileSystem.Classes;

public record AbsoluteFilePathStringValue(string AbsoluteFilePathString)
{
    public AbsoluteFilePathStringValue(IAbsoluteFilePath absoluteFilePath) 
        : this(absoluteFilePath.GetAbsoluteFilePathString())
    {
        
    }
}