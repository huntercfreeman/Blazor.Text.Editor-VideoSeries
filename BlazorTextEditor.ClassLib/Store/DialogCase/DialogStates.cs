using System.Collections.Immutable;
using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.DialogCase;

[FeatureState]
public record DialogStates(ImmutableList<DialogRecord> DialogRecords)
{
    public DialogStates() : this(ImmutableList<DialogRecord>.Empty)
    {
        
    }
}

public class DialogStatesReducer
{
    [ReducerMethod]
    public static DialogStates ReduceRegisterDialogRecordAction(DialogStates previousDialogStates,
        RegisterDialogRecordAction registerDialogRecordAction)
    {
        var nextList = previousDialogStates.DialogRecords
            .Add(registerDialogRecordAction.DialogRecord);

        return previousDialogStates with
        {
            DialogRecords = nextList
        };
    }
    
    [ReducerMethod]
    public static DialogStates ReduceDisposeDialogRecordAction(DialogStates previousDialogStates,
        DisposeDialogRecordAction disposeDialogRecordAction)
    {
        var nextList = previousDialogStates.DialogRecords
            .Remove(disposeDialogRecordAction.DialogRecord);

        return previousDialogStates with
        {
            DialogRecords = nextList
        };
    }
}

public record RegisterDialogRecordAction(DialogRecord DialogRecord);
public record DisposeDialogRecordAction(DialogRecord DialogRecord);

public record DialogRecord(
    DialogKey DialogKey,
    string Title,
    Type RendererType,
    Dictionary<string, object?>? Parameters);

public record DialogKey(Guid Guid)
{
    public static DialogKey NewDialogKey()
    {
        return new(Guid.NewGuid());
    }
}

