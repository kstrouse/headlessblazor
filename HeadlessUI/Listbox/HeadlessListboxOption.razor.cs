using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeadlessUI.Listbox
{
    public partial class HeadlessListboxOption<TValue> : IDisposable
    {
        [CascadingParameter] public HeadlessListbox<TValue> Listbox { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string CssClass { get; set; }
        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public bool IsVisible { get; set; } = true;
        
        [Parameter] public TValue Value { get; set; }
        [Parameter] public string SearchValue { get; set; } = "";
        
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter] public string TagName { get; set; } = "li";

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        public bool IsSelected => EqualityComparer<TValue>.Default.Equals(Value, Listbox.Value);
        public bool IsActive => Listbox.IsActiveOption(this);

        protected override void OnInitialized() => Listbox.RegisterOption(this);
        public void Dispose() => Listbox.UnregisterOption(this);

        private async Task HandleClick()
        {
            if (IsEnabled)
                await Listbox.SetValue(Value);
        }
        private void HandleFocus(EventArgs e)
        {
            if (IsEnabled)
            {
                Listbox.GoToOption(this);
                return;
            }

            Listbox.GoToOption(ListboxFocus.Nothing);
        }
        protected async Task HandlePointerMove(PointerEventArgs e)
        {
            if (!IsEnabled) return;
            if (Listbox.State == ListboxState.Closed) return;

            await Listbox.OptionsFocusAsync();
            if (IsActive) return;
            Listbox.GoToOption(this);
        }
        protected void HandleMouseOut(MouseEventArgs e)
        {
            if (!IsEnabled) return;
            if (!IsActive) return;
            Listbox.GoToOption(ListboxFocus.Nothing);
        }
    }
}
