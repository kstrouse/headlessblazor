using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace HeadlessUI.Utilities
{
    public class TransitionGroup : ComponentBase
    {
        private readonly List<Transition> transitions = new();

        [Parameter] public RenderFragment ChildContent { get; set; } = default!;
        [Parameter] public bool Show { get; set; }

        public void RegisterTransition(Transition transition)
        {
            transitions.Add(transition);
        }

        public void NotifyEndTransition()
        {
            StateHasChanged();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Show || !transitions.All(t => t.State == TransitionState.Hidden))
            {
                builder.OpenComponent<CascadingValue<TransitionGroup>>(0);
                builder.AddMultipleAttributes(1, new Dictionary<string, object>
                {
                    ["Value"] = this,
                    ["ChildContent"] = ChildContent
                });
                builder.CloseComponent();
            }
        }
    }
}
