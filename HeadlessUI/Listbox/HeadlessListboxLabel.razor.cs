using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI.Listbox
{
    public partial class HeadlessListboxLabel<TValue>
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public string CssClass { get; set; }
        [CascadingParameter] public HeadlessListbox<TValue> Listbox { get; set; }
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter] public string TagName { get; set; } = "label";

        protected override void OnInitialized() => Listbox.Registerlabel(this);
    }
}
