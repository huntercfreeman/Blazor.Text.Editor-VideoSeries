using System.Text;

namespace BlazorTextEditor.ClassLib.TextEditor;

public class TextEditorBase
{
    public StringBuilder Content { get; set; } = new();
}