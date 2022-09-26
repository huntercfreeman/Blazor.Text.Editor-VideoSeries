using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.IconCase;

[FeatureState]
public record IconState(int IconSizeInPixels)
{
    public IconState() : this(32)
    {
        
    }
}