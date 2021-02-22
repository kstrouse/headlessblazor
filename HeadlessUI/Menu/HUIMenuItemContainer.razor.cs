using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HUIMenuItemContainer
    {
        [Parameter] public string TagName { get; set; } = "div";
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
        [CascadingParameter] public HUIMenu Menu { get; set; }

        public bool IsVisible => Menu.MenuState == MenuState.Open;

        protected HtmlElement containerElement;
        public bool HasRendered => containerElement.ElementReference.Id != null;
        public ValueTask FocusAsync() => containerElement.FocusAsync();

        protected override void OnInitialized()
        {
            Menu.MenuItemsControl = this;
        }

        private bool isHandlerRegistered = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!isHandlerRegistered && IsVisible)
            {
                isHandlerRegistered = await Menu.SetItemsReference(containerElement);
            }
            else if (!IsVisible)
            {
                isHandlerRegistered = false;
            }
        }

        [JSInvokable]
        public async Task<string> HandleKeyDown(KeyboardEventArgs eventArgs)
        {
            Menu.ClearDebounceTimer();
            string key = eventArgs.Key;
            if (string.IsNullOrEmpty(key))
                return Menu.SearchQuery;

            switch (key)
            {
                case KeyboardKey.Enter:
                {
                    await Menu.CloseMenu();
                    break;
                }
                case KeyboardKey.ArrowDown:
                {
                    Menu.GoToItem(MenuFocus.NextItem);
                    break;
                }
                case KeyboardKey.ArrowUp:
                {
                    Menu.GoToItem(MenuFocus.PreviousItem);
                    break;
                }
                case KeyboardKey.Home:
                case KeyboardKey.PageUp:
                {
                    Menu.GoToItem(MenuFocus.FirstItem);
                    break;
                }
                case KeyboardKey.End:
                case KeyboardKey.PageDown:
                {
                    Menu.GoToItem(MenuFocus.LastItem);
                    break;
                }
                case KeyboardKey.Escape:
                {
                    await Menu.CloseMenu();
                    await Menu.FocusButtonAsync();
                    break;
                }
                case KeyboardKey.Tab:
                {
                    break;
                }
                default:
                {
                    Menu.Search(key);
                    break;
                }
            }

            return Menu.SearchQuery;
        }

    }
}
