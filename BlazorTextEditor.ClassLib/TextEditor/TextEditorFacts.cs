namespace BlazorTextEditor.ClassLib.TextEditor;

public static class TextEditorFacts
{
    public static TextEditorKey TextEditorKeyCodeTest = TextEditorKey.NewTextEditorKey();
    public static TextEditorKey TextEditorKeyEmailTest = TextEditorKey.NewTextEditorKey();
    public static TextEditorKey TextEditorKeyEmptyTest = TextEditorKey.NewTextEditorKey();
    
    public const string InitialInput = "public class MyClass\n{\n\tpublic void MyMethod()\n\t{\n\t\treturn;\n\t}\n}\n";
}