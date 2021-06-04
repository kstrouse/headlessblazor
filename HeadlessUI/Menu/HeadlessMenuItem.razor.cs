using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HeadlessMenuItem : IDisposable
    {
        [CascadingParameter] public HeadlessMenu CascadedMenu { get; set; } = default!;
        [CascadingParameter] public HeadlessMenuItems CascadedItems { get; set; } = default!;

        [Parameter] public RenderFragment<HeadlessMenuItem>? ChildContent { get; set; }

        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public bool IsVisible { get; set; } = true;

        [Parameter] public string SearchValue { get; set; } = "";

        [Parameter] public string TagName { get; set; } = "a";
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();

        [Parameter] public EventCallback OnClick { get; set; }

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        private HeadlessMenu Menu { get; set; } = default!;
        private HeadlessMenuItems Items { get; set; } = default!;

        public bool IsActive => Menu.IsActiveItem(this);

        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            ValidateAndSetMenu();
            ValidateAndSetItems();

            return base.SetParametersAsync(ParameterView.Empty);
        }
        [MemberNotNull(nameof(Items), nameof(CascadedItems))]
        private void ValidateAndSetItems()
        {
            if (Items == null)
            {
                if (CascadedItems == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessMenuItem)} inside an {nameof(HeadlessMenuItems)}.");

                Items = CascadedItems;
            }
            else if (CascadedItems != Items)
            {
                throw new InvalidOperationException($"{nameof(HeadlessMenuItem)} does not support changing the {nameof(HeadlessMenuItems)} dynamically.");
            }
        }
        [MemberNotNull(nameof(Menu), nameof(CascadedMenu))]
        private void ValidateAndSetMenu()
        {
            if (Menu == null)
            {
                if (CascadedMenu == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessMenuItem)} inside an {nameof(HeadlessMenuItems)}.");

                Menu = CascadedMenu;
            }
            else if (CascadedMenu != Menu)
            {
                throw new InvalidOperationException($"{nameof(HeadlessMenuItem)} does not support changing the {nameof(HeadlessMenuItems)} dynamically.");
            }
        }
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
