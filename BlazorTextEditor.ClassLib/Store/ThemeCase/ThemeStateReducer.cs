using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.ThemeCase;

public class ThemeStateReducer
{
    [ReducerMethod]
    public static ThemeState ReduceSetThemeStateAction(ThemeState previousThemeState,
        SetThemeStateAction setThemeStateAction)
    {
        return previousThemeState with
        {
            ActiveThemeRecord = setThemeStateAction.ThemeRecord
        };
    }
}