using Fluxor;

namespace BlazorTextEditor.ClassLib.Store.ContextCase;

public class ContextStatesReducer
{
    [ReducerMethod]
    public static ContextStates ReduceSetActiveContextRecordsAction(ContextStates previousContextStates,
        SetActiveContextRecordsAction setActiveContextRecordsAction)
    {
        return previousContextStates with
        {
            ActiveContextRecords = setActiveContextRecordsAction.ContextRecords
        };
    }
}