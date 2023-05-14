using Blazored.Modal.Services;
using CanvasComponent.Converters;
using CanvasComponent.Facade;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using System.Text.Json;
using CanvasComponent.View;
using System.ComponentModel;

namespace SmartHomePriceCalculatorWasm.Components
{
    public partial class CanvasContainer : ComponentBase
    {
        [Parameter]
        public EventCallback<FacadeWrapper> FacadeWrapperChanged { get; set; }

        [Parameter]
        public FacadeWrapper FacadeWrapper { get; set; }
        private FacadeWrapper oldFacadeWrapper { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public IModalService ModalService { get; set; }

        private readonly string[] UpdateOnProperties = new[]
        {
            nameof(Project)
        };

        protected override async Task OnParametersSetAsync()
        {
            if (oldFacadeWrapper != null)
            {
                oldFacadeWrapper.CanvasFacade.PropertyChanged -= OnPropertyChanged;
                oldFacadeWrapper.CanvasFacade.Dispose();
            }

            FacadeWrapper.CanvasFacade.Initialize(JS, ModalService);
            FacadeWrapper.CanvasFacade.PropertyChanged += (s, e) => InvokeAsync(StateHasChanged);
            FacadeWrapper.CanvasFacade.PropertyChanged += OnPropertyChanged;

            oldFacadeWrapper = FacadeWrapper;
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (UpdateOnProperties.Contains(e.PropertyName) && FacadeWrapperChanged.HasDelegate)
                FacadeWrapperChanged.InvokeAsync(FacadeWrapper);
        }
    }
}
