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
            base.OnInitialized();
        }
    }
}
