using CanvasComponent.Converters;
using CanvasComponent.Facade;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace SmartHomeCalculator.Pages
{
    public partial class Index
    {
        [Inject]
        public CanvasFacade CanvasFacade { get; set; }

        bool showSidebar = true;

        protected override void OnInitialized()
        {
            CanvasFacade.PropertyChanged += (s, e) => InvokeAsync(StateHasChanged);
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                // WriteIndented = true
            };
            options.Converters.Add(new CanvasFacadeConverter());
            var a = JsonSerializer.Deserialize<CanvasFacade>(JsonSerializer.Serialize(CanvasFacade, options), options);
            base.OnInitialized();
        }
    }
}
