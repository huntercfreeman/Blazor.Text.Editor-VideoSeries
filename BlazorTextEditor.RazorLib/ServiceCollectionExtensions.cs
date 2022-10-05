using BlazorTextEditor.ClassLib;
using BlazorTextEditor.ClassLib.Clipboard;
using BlazorTextEditor.RazorLib.Clipboard;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditorRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IClipboardProvider, ClipboardProvider>()
            .AddBlazorTextEditorClassLibServices();
    }
}