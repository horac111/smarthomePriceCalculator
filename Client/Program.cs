using Blazored.Modal;
using Blazored.Modal.Services;
using CanvasComponent.Convertors;
using CanvasComponent.Model;
using Client;
using Client.Components;
using JSComponentGeneration.React;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Fast.Components.FluentUI;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.RegisterForReact<CanvasContainer>();
        LibraryConfiguration config = new(ConfigurationGenerator.GetIconConfiguration(), ConfigurationGenerator.GetEmojiConfiguration());
        builder.Services.AddFluentUIComponents(config);
        builder.Services.AddScoped(sp => new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddBlazoredModal();

        await builder.Build().RunAsync();
    }
}




