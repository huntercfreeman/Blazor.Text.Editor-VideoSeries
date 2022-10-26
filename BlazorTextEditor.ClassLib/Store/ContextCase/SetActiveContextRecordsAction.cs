using System.Collections.Immutable;
using BlazorTextEditor.ClassLib.Context;

namespace BlazorTextEditor.ClassLib.Store.ContextCase;

public record SetActiveContextRecordsAction(ImmutableArray<ContextRecord> ContextRecords);