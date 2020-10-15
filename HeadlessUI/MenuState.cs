using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadlessUI
{
    public class MenuItemState
    {
        public bool IsActive { get; }
        public MenuItemState(bool isActive) => IsActive = isActive;
    }
    public enum MenuState
    {
        Closed,
        Open
    }

    public enum Focus
    {
        FirstItem,
        PreviousItem,
        NextItem,
        LastItem,
        Nothing
    }
}
