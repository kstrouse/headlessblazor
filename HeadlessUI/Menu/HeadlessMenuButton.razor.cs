using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HeadlessMenuButton : IAsyncDisposable
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string TagName { get; set; } = "button";
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }
        [CascadingParameter] public HeadlessMenu CascadedMenu { get; set; } = default!;

        private HtmlElement? rootElement;
        private KeyDownEventHandler? keyDownEventHandler;

        protected HeadlessMenu Menu { get; set; } = default!;

        [MemberNotNull(nameof(Menu), nameof(CascadedMenu))]
        public override Task SetParametersAsync(ParameterView parameters)
        {
            //This is here to follow the pattern/example as implmented in Microsoft's InputBase component
            //https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputBase.cs

            parameters.SetParameterProperties(this);

            if (Menu == null)
            {
                if (CascadedMenu == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessMenuButton)} inside an {nameof(HeadlessMenu)}.");

                Menu = CascadedMenu;
            }
            else if (CascadedMenu != Menu)
            {
                throw new InvalidOperationException($"{nameof(HeadlessMenuButton)} does not support changing the {nameof(HeadlessMenu)} dynamically.");
            }

            return base.SetParametersAsync(ParameterView.Empty);
        }
        protected override void OnInitialized() => Menu.RegisterButton(this);
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (keyDownEventHandler != null)
                await keyDownEventHandler.RegisterElement(rootElement!);
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

        public ValueTask FocusAsync() => rootElement?.FocusAsync() ?? ValueTask.CompletedTask;
        public async ValueTask DisposeAsync()
        {
            if (keyDownEventHandler != null)
                await keyDownEventHandler.UnregisterElement(rootElement!);
        }

        public static implicit operator ElementReference(HeadlessMenuButton element) => element?.rootElement ?? default!;
    }
}
