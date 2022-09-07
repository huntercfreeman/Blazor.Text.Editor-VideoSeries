using System.Text;

namespace BlazorTextEditor.ClassLib.TextEditor;

public class TextEditorBase
{
    private const string _initialInput = @"public class MyClass
{
    public void MyMethod()
    {
        return;
    }
}
";

    public TextEditorBase()
    {
        foreach (var character in _initialInput)
        {
            Content.Add(new RichCharacter
            {
                Value = character,
                DecorationByte = default
            });
        }
    }
    
    public List<RichCharacter> Content { get; set; } = new();
}