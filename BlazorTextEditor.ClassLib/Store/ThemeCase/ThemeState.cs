using Fluxor;
using Microsoft.VisualBasic.CompilerServices;

namespace BlazorTextEditor.ClassLib.Store.ThemeCase;

[FeatureState]
public record ThemeState(ThemeRecord ActiveThemeRecord)
{
    public ThemeState() : this(ThemeFacts.DarkTheme)
    {
        
    }
}