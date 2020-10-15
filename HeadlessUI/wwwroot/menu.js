function RegisterKeyDownHandler(el, callback) {
}

export function makeMenu(button) {
    return new Menu(button);
}

class Menu {
    constructor() {
        this.searchQuery = '';
    }

    setButtonReference = (button) => {
        this.button = button;
    }

    setItemsReference = (items, itemsObj, keydownMethod) => {
        this.items = items;
        this.keydownCallback = keydownMethod;
        this.itemsObj = itemsObj;
        items.addEventListener('keydown', this.handleKeyDown);
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
}