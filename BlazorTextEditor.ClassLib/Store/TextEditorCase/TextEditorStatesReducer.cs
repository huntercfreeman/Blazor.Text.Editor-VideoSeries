using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.TextEditorCase;

public class TextEditorStatesReducer
{
    [ReducerMethod]
    public static TextEditorStates ReduceRegisterTextEditorStateAction(TextEditorStates previousTextEditorStates,
        RegisterTextEditorStateAction registerTextEditorStateAction)
    {
        if (previousTextEditorStates.TextEditorList
            .Any(x => x.Key == registerTextEditorStateAction.TextEditorBase.Key))
            return previousTextEditorStates;
        
        var nextMap = previousTextEditorStates.TextEditorList
            .Add(registerTextEditorStateAction.TextEditorBase);

        return previousTextEditorStates with
        {
            TextEditorList = nextMap
        };
    }
    
    [ReducerMethod]
    public static TextEditorStates ReduceEditTextEditorAction(TextEditorStates previousTextEditorStates,
        EditTextEditorAction editTextEditorAction)
    {
        var textEditor = previousTextEditorStates.TextEditorList
            .Single(x => x.Key == editTextEditorAction.TextEditorKey);

        var nextTextEditor = textEditor.PerformEditTextEditorAction(editTextEditorAction);

        var nextMap = previousTextEditorStates.TextEditorList
            .Replace(textEditor, nextTextEditor);

        return previousTextEditorStates with
        {
            TextEditorList = nextMap
        };
    }
}