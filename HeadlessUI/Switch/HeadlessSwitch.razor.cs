using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI.Switch
{
    public partial class HeadlessSwitch
    {
        [Parameter] public string Id { get; set; } = HtmlElement.GenerateId();
        [Parameter] public string TagName { get; set; } = "button";

        [Parameter] public bool Checked { get; set; }
        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public EventCallback<bool> CheckedChanged { get; set; }


        [Parameter] public RenderFragment? ChildContent { get; set; }

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

        [CascadingParameter] public HeadlessSwitchGroup? Group { get; set; }

        protected bool CurrentChecked
        {
            get => Checked;
            set
            {
                var hasChanged = value != Checked;
                if (hasChanged)
                {
                    Checked = value;
                    _ = CheckedChanged.InvokeAsync(Checked);
                }
            }
        }

        protected override void OnInitialized() => Group?.RegisterSwitch(this);

        public void Toggle() => CurrentChecked = !CurrentChecked;

        public void HandleClick()
        {
            if (IsEnabled)
                Toggle();
        }


    }
}
