using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace HeadlessUI.Utilities
{
    public class ClickOffEventHandler : EventHandlerComponentBase<ClickOffEventHandler>
    {
        [Parameter] public EventCallback OnClickOff { get; set; }
        [JSInvokable] public Task HandleClickOff() => OnClickOff.InvokeAsync();

        public ClickOffEventHandler() : base("clickoffhandler", nameof(HandleClickOff)) { }
    }
}
