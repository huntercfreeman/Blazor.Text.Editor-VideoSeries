using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.Context;
using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.ContextCase;

[FeatureState]
public record ContextStates(ImmutableArray<ContextRecord> ActiveContextRecords)
{
    public ContextStates() : this(ImmutableArray<ContextRecord>.Empty)
    {
        ActiveContextRecords = new []
        {
            ContextFacts.GlobalContext
        }.ToImmutableArray();
    }
}