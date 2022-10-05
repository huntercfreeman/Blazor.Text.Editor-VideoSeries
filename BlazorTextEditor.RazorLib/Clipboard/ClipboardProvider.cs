namespace BlazorTextEditor.RazorLib.Clipboard;

using BlazorTextEditor.ClassLib.Clipboard;
using Microsoft.JSInterop;

public class ClipboardProvider : IClipboardProvider
{
    private readonly IJSRuntime _jsRuntime;

    public ClipboardProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> ReadClipboard()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>(
                "blazorTextEditor.readClipboard");
        }
        catch (TaskCanceledException e)
        {
            return string.Empty;
        }
    }

    public async Task SetClipboard(string value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "blazorTextEditor.setClipboard",
                value);
        }
        catch (TaskCanceledException e)
        {
        }
    }
}