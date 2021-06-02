using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace HeadlessUI.Listbox
{
    public partial class HeadlessListboxLabel<TValue>
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [CascadingParameter] public HeadlessListbox<TValue> Listbox { get; set; }
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter] public string TagName { get; set; } = "label";

        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }

        protected override void OnInitialized() => Listbox.Registerlabel(this);
    }
}
