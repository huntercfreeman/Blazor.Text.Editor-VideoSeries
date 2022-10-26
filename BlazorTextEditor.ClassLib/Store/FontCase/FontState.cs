using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.FontCase;

[FeatureState]
public record FontState(int FontSizeInPixels)
{
    public FontState() : this(25)
    {
        
    }
}