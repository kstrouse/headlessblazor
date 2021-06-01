
namespace HeadlessUI.Utilities
{
    public class ComponentEventArgs<TSender, TEventArgs> : ComponentEventArgs<TSender>
    {
        public ComponentEventArgs(TSender sender, TEventArgs eventArgs)
            : base(sender)
        {
            EventArgs = eventArgs;
        }

        public TEventArgs EventArgs { get; }

        public void Deconstruct(out TSender sender, out TEventArgs eventArgs)
        {
            sender = Sender;
            eventArgs = EventArgs;
        }

        public static implicit operator TEventArgs(ComponentEventArgs<TSender, TEventArgs> eventArgs) => eventArgs.EventArgs;
        public static implicit operator ComponentEventArgs<TSender, TEventArgs>((TSender, TEventArgs) eventArgs) => new ComponentEventArgs<TSender, TEventArgs>(eventArgs.Item1, eventArgs.Item2);
    }

    public class ComponentEventArgs<TSender>
    {
        public ComponentEventArgs(TSender sender)
        {
            Sender = sender;
        }

        public TSender Sender { get; }

        public static implicit operator TSender(ComponentEventArgs<TSender> eventArgs) => eventArgs.Sender;
        public static implicit operator ComponentEventArgs<TSender>(TSender sender) => new ComponentEventArgs<TSender>(sender);
    }
}
