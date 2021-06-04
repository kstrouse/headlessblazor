using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HeadlessUI.Listbox
{
    public partial class HeadlessListboxLabel<TValue>
    {
        [CascadingParameter] public HeadlessListbox<TValue> CascadedListbox { get; set; } = default!;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter] public string TagName { get; set; } = "label";

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        protected HeadlessListbox<TValue> Listbox { get; set; } = default!;

        [MemberNotNull(nameof(Listbox), nameof(CascadedListbox))]
        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (Listbox == null)
            {
                if (CascadedListbox == null)
                    throw new InvalidOperationException($"You must use {nameof(HeadlessListboxLabel<TValue>)} inside an {nameof(HeadlessListbox<TValue>)}.");

                Listbox = CascadedListbox;
            }
            else if (CascadedListbox != Listbox)
            {
                throw new InvalidOperationException($"{nameof(HeadlessListboxLabel<TValue>)} does not support changing the {nameof(HeadlessListbox<TValue>)} dynamically.");
            }

            return base.SetParametersAsync(ParameterView.Empty);
        }

        protected override void OnInitialized() => Listbox.Registerlabel(this);
    }
}
