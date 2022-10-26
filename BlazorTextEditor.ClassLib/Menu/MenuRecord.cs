using System.Collections.Immutable;

namespace BlazorTextEditor.ClassLib.Menu;

public record MenuRecord(ImmutableArray<MenuOptionRecord> MenuOptions);