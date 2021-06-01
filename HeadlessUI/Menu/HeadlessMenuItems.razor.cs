using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HeadlessMenuItems : IAsyncDisposable
    {
        [CascadingParameter] public HeadlessMenu Menu { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public bool Static { get; set; }
        [Parameter] public string TagName { get; set; } = "div";
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter] public string CssClass { get; set; }

        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }

        protected HtmlElement rootElement;
        private KeyDownEventHandler keyDownEventHandler;

        protected override void OnInitialized() => Menu.RegisterItems(this);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (keyDownEventHandler != null)
                await keyDownEventHandler.RegisterElement(rootElement);
        }

        public async Task HandleKeyDown(KeyboardEventArgs eventArgs)
        {
            string key = eventArgs.Key;
            if (string.IsNullOrEmpty(key)) return;

            switch (key)
            {
                case KeyboardKey.ArrowDown:
                    Menu.GoToItem(MenuFocus.Next);
                    break;
                case KeyboardKey.ArrowUp:
                    Menu.GoToItem(MenuFocus.Previous);
                    break;
                case KeyboardKey.Home:
                case KeyboardKey.PageUp:
                    Menu.GoToItem(MenuFocus.First);
                    break;
                case KeyboardKey.End:
                case KeyboardKey.PageDown:
                    Menu.GoToItem(MenuFocus.Last);
                    break;
                case KeyboardKey.Enter:
                case KeyboardKey.Escape:
                    await Menu.Close();
                    break;
                case KeyboardKey.Tab:
                    await Menu.Close(true);
                    break;
                default:
                    Menu.Search(key);
                    break;
            }
        }

        public ValueTask FocusAsync() => rootElement.FocusAsync();

        public async ValueTask DisposeAsync()
        {
            if (keyDownEventHandler != null)
                await keyDownEventHandler.UnregisterElement(rootElement);
        }

        public static implicit operator ElementReference(HeadlessMenuItems element) => element?.rootElement ?? default;
    }
}
