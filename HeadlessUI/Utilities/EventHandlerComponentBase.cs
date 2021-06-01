using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeadlessUI.Utilities
{
    public abstract class EventHandlerComponentBase<TComponent> : ComponentBase, IAsyncDisposable
        where TComponent : EventHandlerComponentBase<TComponent>
    {
        protected IJSObjectReference jsHandlerReference;
        private string jsFileName;
        private string handlerMethodName;
        private readonly List<ElementReference> registeredElements = new List<ElementReference>();

        protected EventHandlerComponentBase(string jsFileName, string handlerMethodName)
        {
            this.jsFileName = jsFileName;
            this.handlerMethodName = handlerMethodName;
        }

        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/HeadlessUI/{jsFileName}.js");
                var objRef = DotNetObjectReference.Create((TComponent)this);

                var properties = GetAdditionalInitializationProperties().ToList();
                properties.Insert(0, objRef);
                properties.Insert(1, handlerMethodName);

                jsHandlerReference = await jsModule.InvokeAsync<IJSObjectReference>("makeHandler", properties.ToArray());
                foreach (var element in registeredElements)
                {
                    await jsHandlerReference.InvokeVoidAsync("registerElement", element);
                }
            }
            catch (JSException)
            {
                //if we are prerendering we don't have access to the jsRuntime so just ignore the exception
            }
        }

        protected virtual IEnumerable<object> GetAdditionalInitializationProperties() => Enumerable.Empty<object>();

        public ValueTask DisposeAsync()
            => jsHandlerReference.InvokeVoidAsync("unmount");

        public async ValueTask RegisterElement(ElementReference element)
        {
            if (element.Id == null) return;
            if (registeredElements.Any(e => e.Id == element.Id)) return;
            registeredElements.Add(element);
            if (jsHandlerReference == null) return;
            await jsHandlerReference.InvokeVoidAsync("registerElement", element);
        }

        public async ValueTask UnregisterElement(ElementReference element)
        {
            if (element.Id == null) return;
            registeredElements.Remove(element);
            if (jsHandlerReference == null) return;
            await jsHandlerReference.InvokeVoidAsync("unregisterElement", element);
        }
    }
}
