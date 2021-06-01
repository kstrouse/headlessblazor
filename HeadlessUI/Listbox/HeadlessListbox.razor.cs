using HeadlessUI.Listbox;
using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI.Listbox
{
    public partial class HeadlessListbox<TValue> : IDisposable
    {
        [Parameter] public RenderFragment<HeadlessListbox<TValue>> ChildContent { get; set; }

        [Parameter] public TValue Value { get; set; }
        [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter] public EventCallback OnOpen { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }

        [Parameter] public int DebouceTimeout { get; set; } = 350;

        private readonly List<HeadlessListboxOption<TValue>> options = new();
        private HeadlessListboxOption<TValue> activeOption;
        private ClickOffEventHandler clickOffEventHandler;
        private SearchAssistant searchAssistant;

        private HeadlessListboxButton<TValue> buttonElement;
        private HeadlessListboxOptions<TValue> optionsElement;
        private HeadlessListboxLabel<TValue> labelElement;

        public ListboxState State { get; protected set; } = ListboxState.Closed;
        public string SearchQuery => searchAssistant.SearchQuery;

        public string LabelId => labelElement?.Id;
        public string ActiveOptionId => activeOption?.Id;
        public string ButtonElementId => buttonElement?.Id;
        public string OptionsElementId => optionsElement?.Id;

        public HeadlessListbox()
        {
            searchAssistant = new SearchAssistant();
            searchAssistant.OnChange += HandleSearchChange;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (shouldFocus)
            {
                shouldFocus = false;
                if (State == ListboxState.Open)
                    await OptionsFocusAsync();
                else
                    await ButtonFocusAsync();
            }
            await clickOffEventHandler.RegisterElement(buttonElement);
            await clickOffEventHandler.RegisterElement(optionsElement);
        }

        public async Task SetValue(TValue value)
        {
            bool valueChanged = !EqualityComparer<TValue>.Default.Equals(Value, value);
            Value = value;
            if (valueChanged)
                await ValueChanged.InvokeAsync(value);
            await Close();
        }

        public void RegisterOption(HeadlessListboxOption<TValue> option)
        {
            options.Add(option);
        }
        public void UnregisterOption(HeadlessListboxOption<TValue> option)
        {
            if (!options.Contains(option)) return;

            if (activeOption == option)
            {
                GoToOption(ListboxFocus.Next);
            }
            options.Remove(option);
        }
        public bool IsActiveOption(HeadlessListboxOption<TValue> option) => activeOption == option;
        public void GoToOption(HeadlessListboxOption<TValue> option)
        {
            if (option != null && (!option.IsEnabled || !options.Contains(option))) option = null;
            if (activeOption == option) return;

            activeOption = option;
            StateHasChanged();
        }
        public void GoToOption(ListboxFocus focus)
        {
            switch (focus)
            {
                case ListboxFocus.First:
                    {
                        GoToOption(options.FirstOrDefault(mi => mi.IsEnabled));
                        break;
                    }
                case ListboxFocus.Previous:
                    {
                        var option = FindOptionBefore(activeOption);
                        if (activeOption == null)
                            GoToOption(ListboxFocus.Last);
                        else
                            GoToOption(option);
                        break;
                    }
                case ListboxFocus.Next:
                    {
                        var option = FindOptionAfter(activeOption);
                        if (option == null)
                            GoToOption(ListboxFocus.First);
                        else
                            GoToOption(option);
                        break;
                    }
                case ListboxFocus.Last:
                    {
                        activeOption = options.LastOrDefault(mi => mi.IsEnabled);
                        break;
                    }
                default:
                    {
                        GoToOption(null);
                        break;
                    }
            }
        }
        private HeadlessListboxOption<TValue> FindOptionBefore(HeadlessListboxOption<TValue> target)
        {
            var reversedMenuOptions = options.ToList();
            reversedMenuOptions.Reverse();
            bool foundTarget = false;
            var itemIndex = reversedMenuOptions.FindIndex(0, mi =>
            {
                if (mi == target)
                {
                    foundTarget = true;
                    return false;
                }
                return foundTarget && mi.IsEnabled;
            });
            if (itemIndex != -1)
                return reversedMenuOptions[itemIndex];
            else
                return null;
        }
        private HeadlessListboxOption<TValue> FindOptionAfter(HeadlessListboxOption<TValue> target)
        {
            bool foundTarget = false;
            var itemIndex = options.FindIndex(0, mi =>
            {
                if (mi == target)
                {
                    foundTarget = true;
                    return false;
                }
                return foundTarget && mi.IsEnabled;
            });
            if (itemIndex != -1)
                return options[itemIndex];
            else
                return null;
        }

        public void RegisterButton(HeadlessListboxButton<TValue> button)
            => buttonElement = button;
        public void RegisterOptions(HeadlessListboxOptions<TValue> options)
            => optionsElement = options;
        public void Registerlabel(HeadlessListboxLabel<TValue> label)
            => labelElement = label;

        private bool shouldFocus;
        public async Task Toggle()
        {
            if (State == ListboxState.Closed)
                await Open();
            else
                await Close();
        }
        public async Task Close(bool suppressFocus = false)
        {
            if (State == ListboxState.Closed) return;
            State = ListboxState.Closed;
            await OnClose.InvokeAsync();
            activeOption = null;
            shouldFocus = !suppressFocus;
            StateHasChanged();
        }
        public async Task Open()
        {
            if (State == ListboxState.Open) return;
            State = ListboxState.Open;
            await OnOpen.InvokeAsync();
            shouldFocus = true;
            StateHasChanged();
        }
        public ValueTask OptionsFocusAsync() => optionsElement.FocusAsync();
        public ValueTask ButtonFocusAsync() => buttonElement.FocusAsync();
        public Task SetActiveAsValue() => SetValue(activeOption.Value);

        public Task HandleClickOff() => Close();
        private void HandleSearchChange(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                var item = options.FirstOrDefault(mi => (mi.SearchValue ?? "").StartsWith(SearchQuery, StringComparison.OrdinalIgnoreCase) && mi.IsEnabled);
                GoToOption(item);
            }
        }
        public void Search(string key)
        {
            searchAssistant.Search(key);
        }


        public void Dispose() => searchAssistant.Dispose();
    }
}
