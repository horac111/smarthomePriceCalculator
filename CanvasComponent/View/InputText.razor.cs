using Blazored.Modal;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Fast.Components.FluentUI;
using CanvasComponent.Model;
using CanvasComponent.Abstract;

namespace CanvasComponent.View
{
    public partial class InputText
    {
        [CascadingParameter]
        BlazoredModalInstance ModalInstance { get; set; }

        private FluentTextField testRef;

        [Parameter]
        public INamed ToName { get; init; }

        [Parameter]
        public string Text { get; init; }

        private void OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
                ModalInstance.CloseAsync();
            else if (e.Code == "Escape")
            {
                ToName.Name = null;
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
