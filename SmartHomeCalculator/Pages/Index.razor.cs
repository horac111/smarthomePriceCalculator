namespace SmartHomeCalculator.Pages
{
    public partial class Index
    {
        bool showSidebar = true;

        protected override void OnInitialized()
        {
            CanvasFacade.PropertyChanged += (s, e) => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
