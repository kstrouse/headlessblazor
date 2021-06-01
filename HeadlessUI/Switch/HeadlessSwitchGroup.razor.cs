using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI.Switch
{
    public partial class HeadlessSwitchGroup
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public HeadlessSwitch Switch { get; private set; }

        public void RegisterSwitch(HeadlessSwitch switchElement)
            => Switch = switchElement;

    }
}
