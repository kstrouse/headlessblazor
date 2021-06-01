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

        public async Task HandleClick()
        {
            if (!Passive)
                await Group.Switch.HandleClick();
        }
    }
}
