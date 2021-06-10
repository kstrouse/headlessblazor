using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI.Portal
{
    public class PortalBinder : IPortalBinder
    {
        private Dictionary<string, Portal> portals = new();
        public void RegisterPortal(string name, Portal portal) => portals.Add(name, portal);
        public Portal? GetPortal(string name) => portals.TryGetValue(name, out var portal) ? portal : null;
    }
    public class Portal : ComponentBase
    {
        [Inject] public IPortalBinder? PortalBinder { get; set; }
        [Parameter] public string Name { get; set; } = "root";

        private RenderFragment? content;

        protected override void OnInitialized()
        {
            PortalBinder?.RegisterPortal(Name, this);
        }

        public void RenderContent(RenderFragment? content)
        {
            this.content = content;
            StateHasChanged();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, content);
        }
    }

    public class PortalContent : ComponentBase, IDisposable
    {
        [Inject] public IPortalBinder? PortalBinder { get; set; }
        
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string PortalName { get; set; } = "";

        public void Dispose() => PortalBinder?.GetPortal(PortalName)?.RenderContent(builder => builder.AddContent(0, ""));

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            PortalBinder?.GetPortal(PortalName)?.RenderContent(ChildContent);
        }
    }
}
