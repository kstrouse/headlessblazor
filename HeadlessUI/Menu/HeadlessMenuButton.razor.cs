using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HeadlessMenuButton : IAsyncDisposable
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string TagName { get; set; } = "button";
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
        [CascadingParameter] public HeadlessMenu Menu { get; set; }

        private HtmlElement rootElement;
        private KeyDownEventHandler keyDownEventHandler;

        protected override void OnInitialized() => Menu.RegisterButton(this);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (keyDownEventHandler != null)
                await keyDownEventHandler.RegisterElement(rootElement);
        }

        protected async Task HandleKeyDown(KeyboardEventArgs eventArgs)
        {
            switch (eventArgs.Key)
            {
                case KeyboardKey.Space:
                case KeyboardKey.Enter:
                case KeyboardKey.ArrowDown:
                    {
                        await Menu.Open();
                        Menu.GoToItem(MenuFocus.First);
                        break;
                    }
                case KeyboardKey.ArrowUp:
                    {
                        await Menu.Open();
                        Menu.GoToItem(MenuFocus.Last);
                        break;
                    }

            }
        }
        protected async Task HandleClick(EventArgs eventArgs) => await Menu.Toggle();

        protected async Task HandleFocus(EventArgs eventArgs)
        {
            if (Menu.State == MenuState.Open)
                await Menu.MenuItemsFocusAsync();
        }

        public ValueTask FocusAsync() => rootElement.FocusAsync();
        public async ValueTask DisposeAsync()
        {
            if (keyDownEventHandler != null)
                await keyDownEventHandler.UnregisterElement(rootElement);
        }

        public static implicit operator ElementReference(HeadlessMenuButton element) => element?.rootElement ?? default;
    }
}
