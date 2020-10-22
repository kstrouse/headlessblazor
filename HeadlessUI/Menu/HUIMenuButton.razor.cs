using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HUIMenuButton
    {
        [Parameter] public RenderFragment ChildContent { get; set; } 
        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public string TagName { get; set; } = "button";
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }
        [CascadingParameter] public HUIMenu Menu { get; set; }

        public ValueTask FocusAsync() => rootElement.FocusAsync();

        private bool isButtonRegistered = false;
        protected HtmlElement rootElement;

        protected override void OnInitialized()
        {
            Menu.MenuButtonControl = this;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!isButtonRegistered)
            {
                isButtonRegistered = await Menu.SetButtonReference(rootElement);
            }
        }

        protected async Task HandleKeyDown(KeyboardEventArgs eventArgs)
        {
            switch (eventArgs.Key)
            {
                case KeyboardKey.Space:
                case KeyboardKey.Enter:
                case KeyboardKey.ArrowDown:
                {
                    await Menu.OpenMenu();
                    await Menu.MenuItemsFocusAsync();
                    Menu.GoToItem(MenuFocus.FirstItem);
                    break;
                }
                case KeyboardKey.ArrowUp:
                {
                    await Menu.OpenMenu();
                    await Menu.MenuItemsFocusAsync();
                    Menu.GoToItem(MenuFocus.LastItem);
                    break;
                }

            }
        }
        protected async Task HandlePointerUp(PointerEventArgs eventArgs)
        {
            if (!IsEnabled) return;
            if (Menu.MenuState == MenuState.Open)
            {
                await Menu.CloseMenu();
                await FocusAsync();
            }       
            else
            {
                await Menu.OpenMenu();
                await Menu.MenuItemsFocusAsync();
            }
        }
        protected async Task HandleFocus(EventArgs eventArgs)
        {
            if (Menu.MenuState == MenuState.Open)
            {
                await Menu.MenuItemsFocusAsync();
            }
        }
    }
}
