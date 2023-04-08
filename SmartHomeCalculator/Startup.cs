using Blazored.Modal;
using CanvasComponent.Abstract;
using CanvasComponent.Facade;
using CanvasComponent.Model;
using CanvasComponent.Model.SmartDevice;
using CanvasComponent.Service;
using CanvasComponent.View;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Fast.Components.FluentUI;
using System;
using System.Collections.Generic;
using System.IO;

namespace SmartHomeCalculator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var icon = "data:image/png;base64," +
                Convert.ToBase64String(File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "idea.png")));
            var devices = new List<ISmartDevice>()
            {
                new PricePerMeterSmartDevice()
                {
                    Id = 0,
                    Price = 10,
                    Icon = icon,
                    Name = "10 per meter"
                },
                new DevicesPerMeterSmartDevice()
                {
                    Id = 1,
                    Price = 10,
                    DevicesPerMeter = 3,
                    Icon = icon,
                    Name = "3 devices per meter"
                },
                new WiredFromCentralUnitSmartDevice()
                {
                    Id = 2,
                    BasePrice = 10,
                    Price = 10,
                    Icon = icon,
                    Name = "from central unit"
                },
                new DevicesPerRoomSmartDevice()
                {
                    Id = 3,
                    Price = 10,
                    DevicesInRoom = 2,
                    Icon = icon,
                    IsCentralUnit = true,
                    Name = "2 devices in room also cental unit"
                }
            };
            services.AddServerSideBlazor(option =>
            {
                option.RootComponents.RegisterCustomElement<Canvas>("canvas-component");
            });
            services.AddRazorPages();
            services.AddScoped<Drawing>();
            services.AddScoped<ICanvasFacade, CanvasFacade>();
            services.AddSingleton<IEnumerable<ISmartDevice>>(devices);
            services.AddSingleton<Project>();
            services.AddBlazoredModal();
            services.AddHttpClient();
            LibraryConfiguration config = new(ConfigurationGenerator.GetIconConfiguration(), ConfigurationGenerator.GetEmojiConfiguration());
            services.AddFluentUIComponents(config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
