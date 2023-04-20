using Blazored.Modal;
using Blazored.Modal.Services;
using CanvasComponent.Abstract;
using CanvasComponent.Model;
using CanvasComponent.Model.JSObjects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace CanvasComponent.View
{
    public partial class Canvas
    {
        [Parameter]
        public ICanvasFacade Facade { get; init; }

        protected override void OnInitialized()
        {
            Facade.PropertyChanged += (s, e) => InvokeAsync(StateHasChanged);
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

        private async Task ClickImport(MouseEventArgs e)
        {
            if(e.Button == 0)
                await js.InvokeVoidAsync("eval", "document.querySelector(\"#ImportJson\").click()");
        }
    }
}
