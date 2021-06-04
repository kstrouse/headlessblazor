﻿using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HeadlessUI.Listbox
{
    public partial class HeadlessListboxOptions<TValue> : IAsyncDisposable
    {
        [CascadingParameter] public HeadlessListbox<TValue> CascadedListbox { get; set; } = default!;

        [Parameter] public RenderFragment? ChildContent { get; set; }

        [Parameter] public bool Static { get; set; }
        [Parameter] public string TagName { get; set; } = "ul";
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        private HtmlElement? rootElement;
        private KeyDownEventHandler? keyDownEventHandler;

        protected HeadlessListbox<TValue> Listbox { get; set; } = default!;

        [MemberNotNull(nameof(Listbox), nameof(CascadedListbox))]
        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (Listbox == null)
            {
                if (CascadedListbox == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessListboxOptions<TValue>)} inside an {nameof(HeadlessListbox<TValue>)}.");

                Listbox = CascadedListbox;
            }
            else if (CascadedListbox != Listbox)
            {
                throw new InvalidOperationException($"{nameof(HeadlessListboxOptions<TValue>)} does not support changing the {nameof(HeadlessListbox<TValue>)} dynamically.");
            }

            return base.SetParametersAsync(ParameterView.Empty);
        }
        protected override void OnInitialized() => Listbox.RegisterOptions(this);
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
                case KeyboardKey.Enter:
                    Listbox.SetActiveAsValue();
                    await Listbox.Close();
                    break;
                case KeyboardKey.ArrowDown:
                    Listbox.GoToOption(ListboxFocus.Next);
                    break;
                case KeyboardKey.ArrowUp:
                    Listbox.GoToOption(ListboxFocus.Previous);
                    break;
                case KeyboardKey.Home:
                case KeyboardKey.PageUp:
                    Listbox.GoToOption(ListboxFocus.First);
                    break;
                case KeyboardKey.End:
                case KeyboardKey.PageDown:
                    Listbox.GoToOption(ListboxFocus.Last);
                    break;
                case KeyboardKey.Escape:
                    await Listbox.Close();
                    break;
                case KeyboardKey.Tab:
                    await Listbox.Close(true);
                    break;
                default:
                    Listbox.Search(key);
                    break;
            }
        }

        public ValueTask FocusAsync() => rootElement?.FocusAsync() ?? ValueTask.CompletedTask;

        public async ValueTask DisposeAsync()
        {
            if (keyDownEventHandler != null)
                await keyDownEventHandler.UnregisterElement(rootElement!);
        }

        public static implicit operator ElementReference(HeadlessListboxOptions<TValue>? element) => element?.rootElement ?? default!;
    }
}
