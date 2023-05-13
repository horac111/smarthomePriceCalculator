using Microsoft.AspNetCore.Components.Web;
using Microsoft.Fast.Components.FluentUI.DesignTokens;
using Microsoft.Fast.Components.FluentUI;

namespace SmartHomeCalculator.Shared
{
    public partial class MainLayout
    {
        bool darkMode = true;

        string baseColor = "#FFFFFF";
        string BaseColor
        {
            get => baseColor;
            set
            {
                baseColor = value;
                Task.Run(OnSelectionChanged);
            }
        }

        FluentDesignSystemProvider designSystemProvider;

        static List<Option<string>> baseColorOptions = new()
        {
            { new Option<string> { Value = "#D83B01", Text = "Office", Selected = true } },
            { new Option<string> { Value = "#a4373a", Text = "Access" } },
            { new Option<string> { Value = "#0078d4", Text = "Exchange" } },
            { new Option<string> { Value = "#185ABD", Text = "Word" } },
            { new Option<string> { Value = "#217346", Text = "Excel" } },
            { new Option<string> { Value = "#C43E1C", Text = "PowerPoint" } },
            { new Option<string> { Value = "#6264A7", Text = "Teams" } },
            { new Option<string> { Value = "#7719AA", Text = "OneNote" } },
            { new Option<string> { Value = "#03787C", Text = "SharePoint" } },
            { new Option<string> { Value = "#BC1948", Text = "Stream" } }

        };

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                Task.Run(OnSelectionChanged);
                Task.Run(() => OnClicked(null));
            }

            base.OnAfterRender(firstRender);
        }

        private async Task OnClicked(MouseEventArgs e)
        {
            darkMode ^= true;
            var luminance = (darkMode ? StandardLuminance.DarkMode : StandardLuminance.LightMode).GetLuminanceValue();
            await baseLayerLuminance.SetValueFor(designSystemProvider!.Element, luminance);
        }

        private async Task OnSelectionChanged()
        {
            await accentBaseColor.SetValueFor(designSystemProvider!.Element, BaseColor.ToSwatch());
        }
    }
}
