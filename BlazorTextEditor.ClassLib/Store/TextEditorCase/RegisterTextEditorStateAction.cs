using BlazorTextEditor.ClassLib.TextEditor;

namespace BlazorTextEditor.ClassLib.Store.TextEditorCase;

public record RegisterTextEditorStateAction(TextEditorKey TextEditorKey, TextEditorBase TextEditorBase);