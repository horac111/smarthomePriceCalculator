using Blazored.Modal;
using Blazored.Modal.Services;
using CanvasComponent.Abstract;
using CanvasComponent.Facade;
using CanvasComponent.View;
using Microsoft.JSInterop;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
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
        private CancellationTokenSource cts = new();
        public NamingService(IJSRuntime js, IModalService modal)
        {
            this.modal = modal;
            this.js = js;
        }

        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
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
            IModalReference inputText = null;
            ModalResult result = null;
            var wait = Task.Run(async () =>
            {
                while (result is null && !cts.IsCancellationRequested)
                    await Task.Delay(100);
            });
            var show = Task.Run(async () =>
            {
                cts.Token.ThrowIfCancellationRequested();
                inputText = modal.Show<InputText>("Naming room.", param, options);
                result = await inputText.Result;
                await js.InvokeVoidAsync("eval", $"document.getElementById(\"{CanvasFacade.CanvasID}\").focus();");
            }, cts.Token);

            await Task.WhenAny(wait, show);
            if (result is not null)
                return result;

            inputText?.Close();
            return ModalResult.Cancel();
        }
    }
}
