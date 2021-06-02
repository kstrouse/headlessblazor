using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI.Switch
{
    public partial class HeadlessSwitchLabel
    {
        [Parameter] public bool Passive { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [CascadingParameter] public HeadlessSwitchGroup Group { get; set; }

        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        public void HandleClick()
        {
            if (!Passive)
                Group?.ToggleSwitch();
        }
    }
}
