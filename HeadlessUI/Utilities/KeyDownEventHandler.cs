using HeadlessUI.Listbox;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace HeadlessUI.Utilities
{
    public class KeyDownEventHandler : EventHandlerComponentBase<KeyDownEventHandler>
    {
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }
        [Parameter] public List<string> PreventDefaultForKeys { get; set; }
        [JSInvokable] public Task HandleKeyDown(KeyboardEventArgs args)
        {
            return OnKeyDown.InvokeAsync(args);
        }
        
        public KeyDownEventHandler() : base("keydownhandler", nameof(HandleKeyDown)) { }

        protected override IEnumerable<object> GetAdditionalInitializationProperties()
        {
            yield return PreventDefaultForKeys.ToArray();
        }
    }
}
