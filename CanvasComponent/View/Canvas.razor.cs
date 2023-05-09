using Blazored.Modal;
using Blazored.Modal.Services;
using CanvasComponent.Abstract;
using CanvasComponent.Convertors;
using CanvasComponent.Facade;
using CanvasComponent.Model;
using CanvasComponent.Model.JSObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace CanvasComponent.View
{
    public partial class Canvas
    {
        [Parameter]
        public CanvasFacade Facade { get; init; }

        [Inject]
        public IJSRuntime JS { get; set; }



        protected override void OnInitialized()
        {
            Facade.PropertyChanged += (s, e) => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }

        public async Task ManualAfterRender()
            => await OnAfterRenderAsync(true);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var rect = await JS.InvokeAsync<BoundingClientRect>("eval",
                 $"let e = document.querySelector(\"#CanvasContainer\"); e = e.getBoundingClientRect();  e");
                Facade.DrawingHelper = new DrawingHelper(rect);
            }
            await Facade.OnAfterRender(firstRender);
        }

        private async Task ClickImport(MouseEventArgs e)
        {
            if (e.Button == 0)
                await JS.InvokeVoidAsync("eval", "document.querySelector(\"#ImportJson\").click()");
        }
    }
}
