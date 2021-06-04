using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HeadlessMenuItems : IAsyncDisposable
    {
        [CascadingParameter] public HeadlessMenu CascadedMenu { get; set; } = default!;

        [Parameter] public RenderFragment? ChildContent { get; set; }

        [Parameter] public bool Static { get; set; }
        [Parameter] public string TagName { get; set; } = "div";
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        private HtmlElement? rootElement;
        private KeyDownEventHandler? keyDownEventHandler;
        private HeadlessMenu Menu { get; set; } = default!;

        [MemberNotNull(nameof(Menu), nameof(CascadedMenu))]
        public override Task SetParametersAsync(ParameterView parameters)
        {
            //This is here to follow the pattern/example as implmented in Microsoft's InputBase component
            //https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Forms/InputBase.cs

            parameters.SetParameterProperties(this);

            if (Menu == null)
            {
                if (CascadedMenu == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessMenuItems)} inside an {nameof(HeadlessMenu)}.");

                Menu = CascadedMenu;
            }
            else if (CascadedMenu != Menu)
            {
                throw new InvalidOperationException($"{nameof(HeadlessMenuItems)} does not support changing the {nameof(HeadlessMenu)} dynamically.");
            }

            return base.SetParametersAsync(ParameterView.Empty);
        }
        protected override void OnInitialized() => Menu.RegisterItems(this);
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (keyDownEventHandler != null)
                await keyDownEventHandler.RegisterElement(rootElement!);
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

        public ValueTask FocusAsync() => rootElement?.FocusAsync() ?? ValueTask.CompletedTask;

        public async ValueTask DisposeAsync()
        {
            if (keyDownEventHandler != null)
                await keyDownEventHandler.UnregisterElement(rootElement!);
        }

        public static implicit operator ElementReference(HeadlessMenuItems element) => element?.rootElement ?? default!;
    }
}
