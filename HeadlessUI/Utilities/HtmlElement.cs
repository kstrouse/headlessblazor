using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI
{
    public class HtmlElement : ComponentBase
    {
        [Parameter] public string Id { get; set; } = GenerateId();
        [Parameter] public string TagName { get; set; } = "div";
        [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object> Attributes { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public List<string> PreventDefaultOn { get; set; } = new();

        private ElementReference elementReference;

        public static string GenerateId() => Guid.NewGuid().ToString("N");
        
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (string.IsNullOrEmpty(TagName))
            {
                builder.AddContent(0, ChildContent);
                return;
            }

            builder.OpenElement(0, TagName);
            builder.AddAttribute(1, "id", Id);
            builder.AddMultipleAttributes(3, Attributes);
            foreach (var eventName in PreventDefaultOn.Where(s => !string.IsNullOrEmpty(s)))
                builder.AddEventPreventDefaultAttribute(4, eventName, true);
            //if (StopPropagationOn != null)
            //    foreach (var eventName in StopPropagationOn.Where(s => !string.IsNullOrEmpty(s)))
            //        builder.AddEventStopPropagationAttribute(5, eventName, true);
            builder.AddElementReferenceCapture(6, r => OnSetElementReference(r));
            builder.AddContent(6, ChildContent);
            builder.CloseElement();
        }
        
        public void OnSetElementReference(ElementReference reference) => elementReference = reference;
        public ValueTask FocusAsync() => elementReference.FocusAsync();


        public static implicit operator ElementReference(HtmlElement element) => element == null ? default : element.elementReference;
    }
}
