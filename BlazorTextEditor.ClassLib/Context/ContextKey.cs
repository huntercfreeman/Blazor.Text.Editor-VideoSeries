namespace BlazorTextEditor.ClassLib.Context;

public record ContextKey(Guid Guid)
{
    public static ContextKey NewContextKey()
    {
        return new(Guid.NewGuid());
    }
}