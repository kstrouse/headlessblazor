using HeadlessUI.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeadlessUI.Button
{
    public partial class HUIButton : IAsyncDisposable
    {
        [Inject] protected IJSRuntime jsRuntime { get; set; }
        private IJSObjectReference jsModule;

        [Parameter] public bool IsEnabled { get; set; } = true;
        [Parameter] public bool IsVisible { get; set; } = true;

        [Parameter] public EventCallback OnClick { get; set; }

        [Parameter] public string TagName { get; set; } = "button";
        [Parameter] public string AriaLabel { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> AdditionalAttributes { get; set; }

        protected HtmlElement buttonElement;
        private string previouslyRenderedElementId = null;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await EnsureInitialized();   
        }

        private async Task EnsureInitialized()
        {
            if (((ElementReference)buttonElement).Id != previouslyRenderedElementId)
            {
                try
                {
                    await PreventDefaultKeyBehaviorOnEnterAndSpace();
                    previouslyRenderedElementId = ((ElementReference)buttonElement).Id;
                }
                catch (JSException)
                {
                    //if we are prerendering we don't have access to the jsRuntime so just ignore the exception
                }
            }
        }

        private async Task PreventDefaultKeyBehaviorOnEnterAndSpace()
        {
            jsModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/HeadlessUI/common.js");
            await jsModule.InvokeVoidAsync("preventDefaultKeyBehaviorOnKeys", ((ElementReference)buttonElement), new List<string> { KeyboardKey.Enter, KeyboardKey.Space });
        }

        protected async Task HandleClick(MouseEventArgs e)
        {
            if (!IsEnabled || !IsVisible) return;

            await OnClick.InvokeAsync((this, e));
        }

        [JSInvokable]
        public async Task HandleKeyUp(KeyboardEventArgs eventArgs)
        {
            if (!IsEnabled || !IsVisible) return;

            switch (eventArgs.Key)
            {
                case KeyboardKey.Space:
                case KeyboardKey.Enter:
                    {
                        await OnClick.InvokeAsync((this, new MouseEventArgs()));
                        break;
                    }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await jsModule.InvokeVoidAsync("preventDefaultKeyBehaviorOnKeys", ((ElementReference)buttonElement), new List<string> { }, false);
        }

    }
}
