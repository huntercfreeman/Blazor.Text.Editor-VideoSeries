using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.FileSystem.Classes;
using BlazorTextEditor.ClassLib.FileSystem.Interfaces;
using BlazorTextEditor.ClassLib.TextEditor;
using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.TextEditorCase;

[FeatureState]
public record TextEditorStates(ImmutableList<TextEditorBase> TextEditorList)
{
    private static readonly IAbsoluteFilePath TestCaseAbsoluteFilePath = new AbsoluteFilePath(
        "/home/hunter/Documents/TestData/Hamlet_ Entire Play.html",
        false);
    
    public TextEditorStates() : this(ImmutableList<TextEditorBase>.Empty)
    {
        var testCaseFileContents = File
            .ReadAllText(TestCaseAbsoluteFilePath
                .GetAbsoluteFilePathString());
        
        var textEditorTestCase = new TextEditorBase(testCaseFileContents, TextEditorFacts.TextEditorKeyTestCase);

        TextEditorList = TextEditorList.AddRange(new []
        {
            textEditorTestCase
        });
    }
}