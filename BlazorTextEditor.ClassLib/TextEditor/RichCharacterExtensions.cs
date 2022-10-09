using BlazorTextEditor.ClassLib.Keyboard;

namespace BlazorTextEditor.ClassLib.TextEditor;

public static class RichCharacterExtensions
{
    public static RichCharacterKind GetRichCharacterKind(this RichCharacter richCharacter)
    {
        if (KeyboardKeyFacts.IsWhitespaceCharacter(richCharacter.Value))
            return RichCharacterKind.Whitespace;
        else if (KeyboardKeyFacts.IsPunctuationCharacter(richCharacter.Value))
            return RichCharacterKind.Punctuation;
        else
            return RichCharacterKind.LetterOrDigit;
    }
}