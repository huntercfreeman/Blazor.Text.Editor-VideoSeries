using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorTextEditor.ClassLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorTextEditorClassLibServices(this IServiceCollection services)
    {
        return services
            .AddFluxor(options => options
                .ScanAssemblies(typeof(ServiceCollectionExtensions).Assembly));
    }
}