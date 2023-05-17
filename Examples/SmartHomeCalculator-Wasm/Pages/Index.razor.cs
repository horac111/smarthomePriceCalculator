using CanvasComponent.Converters;
using System.Text.Json.Serialization;
using System.Text.Json;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using CanvasComponent.Facade;

namespace SmartHomePriceCalculatorWasm.Pages
{
    public partial class Index
    {
        [Inject]
        public CanvasFacade CanvasFacade { get; set; }

        bool showSidebar = true;

    }
}
