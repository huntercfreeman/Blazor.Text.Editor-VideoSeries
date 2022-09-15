using System;
using BlazorTextEditor.RazorLib;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

namespace BlazorTextEditor.NativeApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
            appBuilder.Services
                .AddLogging();

            appBuilder.Services.AddBlazorTextEditorRazorLibServices();
            
            // register root component
            appBuilder.RootComponents.Add<App>("app");

            var app = appBuilder.Build();

            // customize window
            app.MainWindow
                .SetIconFile("favicon.ico")
                .SetTitle("Photino Hello World");

            AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
            {
                app.MainWindow.OpenAlertWindow("Fatal exception", error.ExceptionObject.ToString());
            };

            app.Run();
        }

    }
}
