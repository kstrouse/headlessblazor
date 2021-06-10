namespace HeadlessUI.Portal
{
    public interface IPortalBinder
    {
        Portal? GetPortal(string name);
        void RegisterPortal(string name, Portal portal);
    }
}