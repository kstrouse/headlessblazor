using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HUIMenuItem : IDisposable
    {
        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public bool IsVisible { get; set; } = true;

        [Parameter] public bool OnClickPreventDefault { get; set; }
        [Parameter] public EventCallback<ComponentEventArgs<HUIMenuItem, MouseEventArgs>> OnClick { get; set; }

        [Parameter] public string TagName { get; set; } = "a";
        [Parameter] public string SearchValue { get; set; } = "";
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();

        [Parameter] public Func<HUIMenuItem, string> CssClassBuilder { get; set; }

        [Parameter] public RenderFragment<HUIMenuItem> ChildContent { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        [CascadingParameter] public HUIMenu Menu { get; set; }

        public bool IsActive => Menu.IsActiveItem(this);

        protected override void OnInitialized() => Menu.RegisterItem(this);
        public void Dispose() => Menu.UnregisterItem(this);

        public async Task HandleClick(MouseEventArgs e)
        {
            if (!IsEnabled) return;
            await Menu.CloseMenu();
            await Menu.FocusButtonAsync();
            await OnClick.InvokeAsync((this, e));
        }

        public void HandleFocus(EventArgs e)
        {
            if (IsEnabled)
                Menu.GoToItem(this);

            Menu.GoToItem(MenuFocus.Nothing);
        }
        public void HandlePointerMove(PointerEventArgs e)
        {
            if (!IsEnabled) return;
            if (Menu.IsActiveItem(this)) return;
            Menu.GoToItem(this);
        }
        public void HandlePointerLeave(MouseEventArgs e)
        {
            if (!IsEnabled) return;
            if (!Menu.IsActiveItem(this)) return;
            Menu.GoToItem(MenuFocus.Nothing);
        }
    }
}
