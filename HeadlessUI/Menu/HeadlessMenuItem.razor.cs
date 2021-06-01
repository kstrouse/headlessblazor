using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HeadlessMenuItem : IDisposable
    {
        [CascadingParameter] public HeadlessMenu Menu { get; set; }

        [Parameter] public RenderFragment<HeadlessMenuItem> ChildContent { get; set; }

        [Parameter] public string CssClass { get; set; }
        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public bool IsVisible { get; set; } = true;

        [Parameter] public string SearchValue { get; set; } = "";

        [Parameter] public string TagName { get; set; } = "a";
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();

        [Parameter] public EventCallback OnClick { get; set; }

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }


        public bool IsActive => Menu.IsActiveItem(this);

        protected override void OnInitialized() => Menu.RegisterItem(this);
        public void Dispose() => Menu.UnregisterItem(this);

        private async Task HandleClick(MouseEventArgs e)
        {
            if (!IsEnabled) return;
            await Menu.Close();
            await OnClick.InvokeAsync();
        }

        private void HandleFocus(EventArgs e)
        {
            if (IsEnabled)
            {
                Menu.GoToItem(this);
                return;
            }

            Menu.GoToItem(MenuFocus.Nothing);
        }
        private async Task HandlePointerMove(PointerEventArgs e)
        {
            if (!IsEnabled) return;
            if (Menu.State == MenuState.Closed) return;

            await Menu.MenuItemsFocusAsync();
            if (Menu.IsActiveItem(this)) return;
            Menu.GoToItem(this);
        }
        private void HandleMouseOut(MouseEventArgs e)
        {
            if (!IsEnabled) return;
            if (!IsActive) return;
            Menu.GoToItem(MenuFocus.Nothing);
        }
    }
}
