using Blazored.Modal;
using Blazored.Modal.Services;
using System.Threading.Tasks;

namespace CanvasComponent.Abstract
{
    /// <summary>
    /// Shows modal dialog to name given model
    /// </summary>
    public interface INamingService
    {
        Task<ModalResult> ShowInputText(INamed toName, string text, ModalPosition position = ModalPosition.BottomLeft);
    }
}