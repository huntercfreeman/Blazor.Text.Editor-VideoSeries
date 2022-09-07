using BlazorTextEditor.ClassLib;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditorRazorLibServices(this IServiceCollection services)
    {
        return services
            .AddBlazorTextEditorClassLibServices();
    }
}