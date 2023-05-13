using Blazored.Modal.Services;
using CanvasComponent.Converters;
using CanvasComponent.Facade;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using System.Text.Json;
using CanvasComponent.View;

namespace SmartHomePriceCalculatorWasm.Components
{
    public partial class CanvasContainer : ComponentBase
    {
        [Parameter]
        public FacadeWrapper FacadeWrapper { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public IModalService ModalService { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            FacadeWrapper.CanvasFacade.PropertyChanged += (s, e) => InvokeAsync(StateHasChanged);
            await base.OnAfterRenderAsync(firstRender);
            await FacadeWrapper.CanvasFacade.Initialize(JS, ModalService);
        }
    }
}
