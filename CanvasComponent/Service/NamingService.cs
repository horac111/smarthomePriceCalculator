using Blazored.Modal;
using Blazored.Modal.Services;
using CanvasComponent.Abstract;
using CanvasComponent.View;
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

        public NamingService(IModalService modal)
        {
            this.modal = modal;
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
            return await inputText.Result;
        }
    }
}
