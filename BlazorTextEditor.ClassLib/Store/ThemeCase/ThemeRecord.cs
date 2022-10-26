namespace BlazorTextEditor.ClassLib.Store.ThemeCase;

public record ThemeRecord(
    string ClassCssString,
    ThemeColorKind ThemeColorKind,
    ThemeContrastKind ThemeContrastKind);