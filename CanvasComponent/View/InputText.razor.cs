using Blazored.Modal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;

namespace CanvasComponent.View
{
    public partial class InputText
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        private FluentTextField testRef;

        [Parameter]
        public CanvasComponent.Model.Room Room { get; init; }

        private void OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
                ModalInstance.CloseAsync();
            else if (e.Code == "Escape")
            {
                Room.Name = null;
                ModalInstance.CloseAsync();
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                testRef.FocusAsync();
            }
        }
    }
}
