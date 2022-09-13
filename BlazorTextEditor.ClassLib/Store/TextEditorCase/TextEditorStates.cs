using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.TextEditorCase;

[FeatureState]
public record TextEditorStates(ImmutableDictionary<TextEditorKey, TextEditorBase> TextEditorMap)
{
    public TextEditorStates() : this(ImmutableDictionary<TextEditorKey, TextEditorBase>.Empty)
    {
        var textEditorCodeTest = new TextEditorBase(TextEditorFacts.InitialInput, TextEditorFacts.TextEditorKeyCodeTest);
        var textEditorEmailTest = new TextEditorBase("Dear Martin,\r\nI am looking forward to our meeting on Thursday.\nI will be there at 5:00 PM\rRegards, Bob", TextEditorFacts.TextEditorKeyEmailTest);
        var textEditorEmptyTest = new TextEditorBase(string.Empty, TextEditorFacts.TextEditorKeyEmptyTest);
        
        TextEditorMap = TextEditorMap.AddRange(new []
        {
            new KeyValuePair<TextEditorKey, TextEditorBase>(textEditorCodeTest.Key, textEditorCodeTest),
            new KeyValuePair<TextEditorKey, TextEditorBase>(textEditorEmailTest.Key, textEditorEmailTest),
            new KeyValuePair<TextEditorKey, TextEditorBase>(textEditorEmptyTest.Key, textEditorEmptyTest)
        });
    }
}