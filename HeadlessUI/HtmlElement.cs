using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeadlessUI
{
    public class HtmlElement : ComponentBase
    {
        [Parameter]
        public string TagName { get; set; } = "div";
        [Parameter]
        public bool IsVisible { get; set; } = true;

        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object> Attributes { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public ElementReference ElementReference { get; set; }

        public void OnSetElementReference(ElementReference reference)
        {
            ElementReference = reference;         
        }

        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [Parameter]
        public string[] PreventDefaultOn { get; set; }
        [Parameter]
        public string[] StopPropagationOn { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!IsVisible) return;

            if (string.IsNullOrEmpty(TagName))
            {
                builder.AddContent(0, ChildContent);
                return;
            }

            builder.OpenElement(0, TagName);
            builder.AddMultipleAttributes(1, Attributes);
            builder.AddAttribute(2, "id", Id);
            if (PreventDefaultOn != null)
                foreach (var eventName in PreventDefaultOn.Where(s => !string.IsNullOrEmpty(s)))
                    builder.AddEventPreventDefaultAttribute(3, eventName, true);
            if (StopPropagationOn != null)
                foreach (var eventName in StopPropagationOn.Where(s => !string.IsNullOrEmpty(s)))
                    builder.AddEventStopPropagationAttribute(4, eventName, true);
            builder.AddElementReferenceCapture(5, r => OnSetElementReference(r));
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        }
    }
}
