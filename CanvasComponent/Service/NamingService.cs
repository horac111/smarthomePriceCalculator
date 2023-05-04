using Blazored.Modal;
using Blazored.Modal.Services;
using CanvasComponent.Abstract;
using CanvasComponent.Facade;
using CanvasComponent.View;
using Microsoft.JSInterop;
using System.Reflection;
using System.Threading.Tasks;

namespace CanvasComponent.Service
{
    /// <summary>
    /// Shows modal dialog to name given model
    /// </summary>
    public class NamingService : INamingService
    {
        private IModalService modal;
        private IJSRuntime js;

        public NamingService(IJSRuntime js, IModalService modal)
        {
            this.modal = modal;
            this.js = js;
        }

        public async Task<ModalResult> ShowInputText(INamed toName, string text,
            ModalPosition position = ModalPosition.BottomLeft)
        {
            ModalParameters param = new()
            {
                { nameof(InputText.ToName), toName },
                { nameof(InputText.Text), text }
            };
            ModalOptions options = new()
            {
                Position = position
            };
            var inputText = modal.Show<InputText>("Naming room.", param, options);
            var result = await inputText.Result;
            await js.InvokeVoidAsync("eval", $"document.getElementById(\"{CanvasFacade.CanvasID}\").focus();");
            return result;
        }
    }
}
