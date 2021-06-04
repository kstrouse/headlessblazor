﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace HeadlessUI.Listbox
{
    public partial class HeadlessListboxOption<TValue> : IDisposable
    {
        [CascadingParameter] public HeadlessListbox<TValue> CascadedListbox { get; set; } = default!;
        [CascadingParameter] public HeadlessListboxOptions<TValue> CascadedOptions { get; set; } = default!;

        [Parameter] public RenderFragment? ChildContent { get; set; }

        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public bool IsVisible { get; set; } = true;
        
        [Parameter] public TValue? Value { get; set; }
        [Parameter] public string SearchValue { get; set; } = "";
        
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter] public string TagName { get; set; } = "li";

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        public bool IsSelected => EqualityComparer<TValue>.Default.Equals(Value, Listbox.Value);
        public bool IsActive => Listbox.IsActiveOption(this);
        public HeadlessListbox<TValue> Listbox { get; set; } = default!;
        public HeadlessListboxOptions<TValue> Options { get; set; } = default!;

        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            ValidateAndSetListbox();
            ValidateAndSetOptions();

            return base.SetParametersAsync(ParameterView.Empty);
        }
        [MemberNotNull(nameof(Options), nameof(CascadedOptions))]
        private void ValidateAndSetOptions()
        {
            if (Options == null)
            {
                if (CascadedOptions == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessListboxOption<TValue>)} inside an {nameof(HeadlessListboxOptions<TValue>)}.");

                Options = CascadedOptions;
            }
            else if (CascadedOptions != Options)
            {
                throw new InvalidOperationException($"{nameof(HeadlessListboxOption<TValue>)} does not support changing the {nameof(HeadlessListboxOptions<TValue>)} dynamically.");
            }
        }
        [MemberNotNull(nameof(Listbox), nameof(CascadedListbox))]
        private void ValidateAndSetListbox()
        {
            if (Listbox == null)
            {
                if (CascadedListbox == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessListboxOption<TValue>)} inside an {nameof(HeadlessListboxOptions<TValue>)}.");

                Listbox = CascadedListbox;
            }
            else if (CascadedListbox != Listbox)
            {
                throw new InvalidOperationException($"{nameof(HeadlessListboxOption<TValue>)} does not support changing the {nameof(HeadlessListbox<TValue>)} dynamically.");
            }
        }
        protected override void OnInitialized() => Listbox.RegisterOption(this);
        public void Dispose() => Listbox.UnregisterOption(this);

        private async Task HandleClick()
        {
            if (!IsEnabled) return;

            Listbox.CurrentValue = Value;

            await Listbox.Close();
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
