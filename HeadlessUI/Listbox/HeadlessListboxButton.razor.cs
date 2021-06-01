using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeadlessUI.Listbox
{
    public partial class HeadlessListboxButton<TValue> 
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [CascadingParameter] public HeadlessListbox<TValue> Listbox { get; set; }
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter] public string TagName { get; set; } = "button";
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }

        private HtmlElement rootElement;
        private KeyDownEventHandler keyDownEventHandler;

        protected override void OnInitialized()
        {
            Listbox.RegisterButton(this);
        }

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
                        await Listbox.Open();
                        Listbox.GoToOption(ListboxFocus.First);
                        break;
                    }
                case KeyboardKey.ArrowUp:
                    {
                        await Listbox.Open();
                        Listbox.GoToOption(ListboxFocus.Last);
                        break;
                    }

            }
        }

        protected async Task HandleFocus(EventArgs eventArgs)
        {
            if (Listbox.State == ListboxState.Open)
                await Listbox.OptionsFocusAsync();
        }

        public async Task HandleClick() => await Listbox.Toggle();
        public ValueTask FocusAsync() => rootElement.FocusAsync();
        public static implicit operator ElementReference(HeadlessListboxButton<TValue> element) => element.rootElement;
    }
}
