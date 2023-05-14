using CanvasComponent.Converters;
using CanvasComponent.Facade;
using CanvasComponent.Model;
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
                 WriteIndented = true
            };
            options.Converters.Add(new CanvasFacadeConverter());
            CanvasFacade.DrawingHelper = new DrawingHelper(0, 0, 0, 0);
            var a = JsonSerializer.Serialize(CanvasFacade, options);
            base.OnInitialized();
        }
    }
}
