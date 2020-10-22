using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI.Menu
{
    public partial class HUIMenu : IAsyncDisposable
    {
        [Inject] protected IJSRuntime jsRuntime { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public int DebouceTimeout { get; set; } = 350;
        [Parameter] public EventCallback<ComponentEventArgs<HUIMenu>> OnOpen { get; set; }
        [Parameter] public EventCallback<ComponentEventArgs<HUIMenu>> OnClose { get; set; }

        private readonly List<HUIMenuItem> menuItems = new List<HUIMenuItem>();
        private IJSObjectReference jsModule;
        private IJSObjectReference jsMenu;
        private HUIMenuItem activeItem;
        private System.Timers.Timer debounceTimer;

        public HUIMenuItemContainer MenuItemsControl { get; set; }
        public HUIMenuButton MenuButtonControl { get; set; }
        public string ActiveItemId => activeItem?.Id;
        public string SearchQuery { get; set; } = "";
        public MenuState MenuState { get; protected set; } = MenuState.Closed;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                jsModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/HeadlessUI/menu.js");
                var objRef = DotNetObjectReference.Create(this);
                jsMenu = await jsModule.InvokeAsync<IJSObjectReference>("makeMenu", objRef);
            }
            catch (JSException)
            {
                //if we are prerendering we don't have access to the jsRuntime so just ignore the exception
            }
        }

        public async Task<bool> SetButtonReference(ElementReference element)
        {
            if (jsMenu == null) return false;
             
            await jsMenu.InvokeVoidAsync("setButtonReference", element);
            return true;
        }

        public async Task<bool> SetItemsReference(ElementReference element)
        {
            if (jsMenu == null) return false;

            var objRef = DotNetObjectReference.Create(MenuItemsControl);
            await jsMenu.InvokeVoidAsync("setItemsReference", element, objRef, nameof(MenuItemsControl.HandleKeyDown));
            return true;
        }

        public void RegisterItem(HUIMenuItem item)
        {
            menuItems.Add(item);
        }
        public void UnregisterItem(HUIMenuItem item)
        {
            if (!menuItems.Contains(item)) return;

            if (IsActiveItem(item))
            {
                GoToItem(MenuFocus.NextItem);
                if (activeItem == null)
                {
                    GoToItem(MenuFocus.FirstItem);
                }
            }
            menuItems.Remove(item);
        }
        public bool IsActiveItem(HUIMenuItem item) => activeItem == item;

        public void GoToItem(MenuFocus focus)
        {
            switch (focus)
            {
                case MenuFocus.FirstItem:
                {
                    activeItem = menuItems.FirstOrDefault(mi => mi.IsEnabled);
                    break;
                }
                case MenuFocus.PreviousItem:
                {
                    var reversedMenuItems = menuItems.ToList();
                    reversedMenuItems.Reverse();
                    bool foundActiveItem = false;
                    var itemIndex = reversedMenuItems.FindIndex(0, mi =>
                    {
                        if (mi == activeItem)
                        {
                            foundActiveItem = true;
                            return false;
                        }
                        return foundActiveItem && mi.IsEnabled;
                    });
                    if (itemIndex != -1)
                        activeItem = reversedMenuItems[itemIndex];
                    else if (!foundActiveItem)
                        GoToItem(MenuFocus.LastItem);
                    break;
                }
                case MenuFocus.NextItem:
                {
                    bool foundActiveItem = false;
                    var itemIndex = menuItems.FindIndex(0, mi =>
                    {
                        if (mi == activeItem)
                        {
                            foundActiveItem = true;
                            return false;
                        }
                        return foundActiveItem && mi.IsEnabled;
                    });
                    if (itemIndex != -1)
                        activeItem = menuItems[itemIndex];
                    else if (!foundActiveItem)
                        GoToItem(MenuFocus.FirstItem);
                    break;
                }
                case MenuFocus.LastItem:
                {
                    activeItem = menuItems.LastOrDefault(mi => mi.IsEnabled);
                    break;
                }
                default:
                {
                    activeItem = null;
                    break;
                }
            }
            SearchQuery = "";
            StateHasChanged();
        }
        public void GoToItem(HUIMenuItem item)
        {
            SearchQuery = "";
            if (!menuItems.Contains(item))
                throw new InvalidOperationException("Cannot goto item that isn't in the menu.");
            activeItem = item;
            StateHasChanged();
        }

        [JSInvokable]
        public async ValueTask OpenMenu()
        {
            MenuState = MenuState.Open;
            StateHasChanged();
            await UpdateJSState();
            await OnOpen.InvokeAsync();
        }
        [JSInvokable]
        public async ValueTask CloseMenu()
        {
            MenuState = MenuState.Closed;
            StateHasChanged();
            await UpdateJSState();
            await OnClose.InvokeAsync();
        }

        private async ValueTask UpdateJSState()
        {
            await jsMenu.InvokeVoidAsync("updateState", MenuState);
        }

        public async Task FocusButtonAsync()
        {
            await MenuButtonControl.FocusAsync();
        }
        public async Task MenuItemsFocusAsync()
        {
            await MenuItemsControl.FocusAsync();
        }

        public void Search(string value)
        {
            SearchQuery += value;
            activeItem = menuItems.FirstOrDefault(mi => mi.SearchValue.StartsWith(SearchQuery, StringComparison.OrdinalIgnoreCase) && mi.IsEnabled);
            debounceTimer = new System.Timers.Timer(DebouceTimeout);
            debounceTimer.Elapsed += DebounceElapsed;
            debounceTimer.Enabled = true;
            StateHasChanged();
        }
        private async void DebounceElapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            await ClearSearch();
            StateHasChanged();
            debounceTimer.Dispose();
        }
        public void ClearDebounceTimer()
        {
            if (debounceTimer != null)
            {
                debounceTimer.Enabled = false;
                debounceTimer.Dispose();
            }
        }
        public async Task ClearSearch()
        {
            SearchQuery = "";
            await jsMenu.InvokeVoidAsync("clearSearch");
        }

        public async ValueTask DisposeAsync()
        {
            await jsMenu.InvokeVoidAsync("unMount");
        }
    }
}
