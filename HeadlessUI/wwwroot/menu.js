export function makeMenu(menuObj) {
    return new Menu(menuObj);
}

const MenuState = {
    Closed: 0,
    Open: 1
}

class Menu {
    constructor(menuObj) {
        this.searchQuery = '';
        this.menuObj = menuObj;
        this.state = MenuState.Closed;
        this.mount();
    }

    handleWindowOnClick = event => {
        if (this.state === MenuState.Closed) return;
        if (this.buttonRef.contains(event.target)) return;
        if (this.itemsRef && !this.itemsRef.contains(event.target)) {
            this.closeMenu();
        }
    }

    closeMenu = () => this.menuObj.invokeMethodAsync("CloseMenu");
    openMenu = () => this.menuObj.invokeMethodAsync("OpenMenu");

    setButtonReference = (button) => {
        this.buttonRef = button;
    }

    setItemsReference = (itemsRef, itemsObj, keydownMethod) => {
        this.itemsRef = itemsRef;
        this.keydownCallback = keydownMethod;
        this.itemsObj = itemsObj;
        itemsRef.addEventListener('keydown', this.handleKeyDown);
    }

    clearSearch = () => { this.searchQuery = ''; }    

    handleKeyDown = event => {
        switch (event.key) {
            case " ":
                if (this.searchQuery !== '') {
                    event.preventDefault();
                }
                break;
            case "Enter":
            case "ArrowDown":
            case "ArrowUp":
            case "Home":
            case "PageUp":
            case "End":
            case "PageDown":
            case "Escape":
            case "Tab":
                event.preventDefault();
                break;
        }
        this.itemsObj.invokeMethodAsync(this.keydownCallback, {
            type: event.type,
            key: event.key,
            code: event.code,
            location: event.location,
            repeat: event.repeat,
            ctrlKey: event.ctrlKey,
            shiftKey: event.shiftKey,
            altKey: event.altKey,
            metaKey: event.metaKey,
        }).then(data => this.searchQuery = data);
    }

    mount = () => {
        window.addEventListener('click', this.handleWindowOnClick);
    }

    unMount = () => {
        window.removeEventListener('click', this.handleWindowOnClick);
    }

    updateState = (state) => {
        this.state = state;
    }
}