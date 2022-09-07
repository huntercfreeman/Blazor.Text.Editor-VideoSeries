using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.TextEditorCase;

public class TextEditorStatesReducer
{
    [ReducerMethod]
    public static TextEditorStates ReduceRegisterTextEditorStateAction(TextEditorStates previousTextEditorStates,
        RegisterTextEditorStateAction registerTextEditorStateAction)
    {
        if (previousTextEditorStates.TextEditorMap.ContainsKey(registerTextEditorStateAction.TextEditorKey))
            return previousTextEditorStates;
        
        var nextMap = previousTextEditorStates.TextEditorMap
            .Add(registerTextEditorStateAction.TextEditorKey, registerTextEditorStateAction.TextEditorBase);

        return previousTextEditorStates with
        {
            TextEditorMap = nextMap
        };
    }
}