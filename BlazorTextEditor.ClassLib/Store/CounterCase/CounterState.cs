using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.CounterCase;

[FeatureState]
public record CounterState(int Count)
{
    public CounterState() : this(0)
    {
        
    }
}