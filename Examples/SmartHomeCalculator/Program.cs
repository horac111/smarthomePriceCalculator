using Blazored.Modal;
using CanvasComponent.Abstract;
using CanvasComponent.Converters;
using CanvasComponent.Facade;
using CanvasComponent.Model;
using CanvasComponent.Model.SmartDevice;
using CanvasComponent.Service;
using CanvasComponent.View;
using JSComponentGeneration.React;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.Hosting;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.JSInterop;
using SmartHomeCalculator.Components;
using System.Text.Json;

namespace SmartHomeCalculator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel();
            builder.WebHost.UseIIS();

            StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
            var devices = new List<ISmartDevice>()
            {
                new PricePerMeterSmartDevice()
                {
                    Id = 0,
                    Price = 10,
                    Icon = GetIcon(path, "heating.png"),
                    Name = "Heating"
                },
                new DevicesPerMeterSmartDevice()
                {
                    Id = 1,
                    Price = 10,
                    DevicesPerMeter = 3,
                    Icon = GetIcon(path, "speakers.png"),
                    Name = "Speakers"
                },
                new WiredFromCentralUnitSmartDevice()
                {
                    Id = 2,
                    BasePrice = 10,
                    Price = 10,
                    Icon = GetIcon(path, "camera.png"),
                    Name = "Surveilence system"
                },
                new DevicesPerRoomSmartDevice()
                {
                    Id = 3,
                    Price = 10,
                    DevicesInRoom = 1,
                    Icon = GetIcon(path, "centralUnit.png"),
                    IsCentralUnit = true,
                    Name = "Control panel"
                }
            };

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();
            builder.Services.AddServerSideBlazor(option =>
            {
                option.RootComponents.RegisterCustomElement<Canvas>("canvas-component");
                option.RootComponents.RegisterForReact<Canvas>();
            });

            LibraryConfiguration config = new(ConfigurationGenerator.GetIconConfiguration(), ConfigurationGenerator.GetEmojiConfiguration());
            builder.Services.AddFluentUIComponents(config);
            builder.Services.AddBlazoredModal();
            builder.Services.AddScoped<CanvasFacade>();
            builder.Services.AddSingleton<IEnumerable<ISmartDevice>>(devices);
            builder.Services.AddSingleton<Project>();

            var app = builder.Build();

            
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }

        private static string GetIcon(string path, string name)
            => "data:image/png;base64," + 
            Convert.ToBase64String(File.ReadAllBytes(Path.Combine(path, name)));
    }
}
