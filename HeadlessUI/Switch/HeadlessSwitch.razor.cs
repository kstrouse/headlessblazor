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
        public string Id { get; } = Guid.NewGuid().ToString("N");

        [Parameter]
        public bool Checked { get; set; }
        [Parameter]
        public EventCallback<bool> CheckedChanged { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public string CssClass { get; set; }

        [CascadingParameter]
        public HeadlessSwitchGroup Group { get; set; }

        protected override void OnInitialized()
        {
            Group?.RegisterSwitch(this);
        }

        public async Task HandleClick()
        {
            Checked = !Checked;
            await CheckedChanged.InvokeAsync(Checked);
        }
    }
}
