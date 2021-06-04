using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HeadlessUI.Listbox
{
    public partial class HeadlessListboxButton<TValue>
    {
        [CascadingParameter] public HeadlessListbox<TValue> CascadedListbox { get; set; } = default!;

        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter] public string TagName { get; set; } = "button";

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        private HtmlElement? rootElement;
        private KeyDownEventHandler? keyDownEventHandler;
        protected HeadlessListbox<TValue> Listbox { get; set; } = default!;

        protected override void OnInitialized()
        {
            Listbox.RegisterButton(this);
        }

        [MemberNotNull(nameof(Listbox), nameof(CascadedListbox))]
        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (Listbox == null)
            {
                if (CascadedListbox == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessListboxButton<TValue>)} inside an {nameof(HeadlessListbox<TValue>)}.");

                Listbox = CascadedListbox;
            }
            else if (CascadedListbox != Listbox)
            {
                throw new InvalidOperationException($"{nameof(HeadlessListboxButton<TValue>)} does not support changing the {nameof(HeadlessListbox<TValue>)} dynamically.");
            }

            return base.SetParametersAsync(ParameterView.Empty);
        }


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
        public ValueTask FocusAsync() => rootElement?.FocusAsync() ?? ValueTask.CompletedTask;
        public static implicit operator ElementReference(HeadlessListboxButton<TValue>? element) => element?.rootElement ?? default!;
    }
}
