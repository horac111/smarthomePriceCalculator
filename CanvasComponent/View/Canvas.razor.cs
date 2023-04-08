using Blazored.Modal;
using Blazored.Modal.Services;
using CanvasComponent.Abstract;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanvasComponent.View
{
    public partial class Canvas
    {
        [Parameter]
        public ICanvasFacade Facade { get; init; }

        [CascadingParameter]
        public IModalService Modal { get; set; }

        protected override void OnInitialized()
        {
            Facade.PropertyChanged += (s, e) => InvokeAsync(StateHasChanged);
            Facade.NewRoom += async (s, e) =>
            {
                await InvokeAsync(async () =>
                {
                    ModalParameters param = new();
                    param.Add(nameof(InputText.Room), e.Room);
                    var modal = Modal.Show<InputText>("Naming room.", param);
                    var result = await modal.Result;
                });


            };
            base.OnInitialized();
        }
        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                var rect = await js.InvokeAsync<BoundingClientRect>("eval",
                  $"let e = document.querySelector(\"#CanvasContainer\"); e = e.getBoundingClientRect();  e");
                Facade.DrawingHelper = new DrawingHelper(rect);
            }

            await Facade.OnAfterRender(firstRender);
            base.OnAfterRender(firstRender);
        }
    }
}
