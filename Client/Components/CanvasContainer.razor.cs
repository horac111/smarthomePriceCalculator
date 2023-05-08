using Blazored.Modal.Services;
using CanvasComponent.Convertors;
using CanvasComponent.Facade;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SmartHomePriceCalculatorClient.Components
{
    public partial class CanvasContainer : ComponentBase
    {
        [Parameter]
        public string Project { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public IModalService ModalService { get; set; }

        private CanvasFacade canvasFacade
        {
            get
            {
                JsonSerializerOptions options = new()
                {
                    IgnoreReadOnlyProperties = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    IncludeFields = true
                };
                options.Converters.Add(new ProjectConverter());
                options.Converters.Add(new ISmartDeviceConverter());
                options.Converters.Add(new LineConverter());
                options.Converters.Add(new RoomConverter());
                return new(JsonSerializer.Deserialize<Project>(Project, options), JS, ModalService);
            }
        }
    }
}
