using CanvasComponent.Converters;
using CanvasComponent.Facade;
using CanvasComponent.Model;
using System.Text.Json.Serialization;

namespace SmartHomePriceCalculatorWasm
{
    public class FacadeWrapper
    {
        [JsonConverter(typeof(CanvasFacadeConverter))]
        public CanvasFacade CanvasFacade { get; set; }
    }
}
