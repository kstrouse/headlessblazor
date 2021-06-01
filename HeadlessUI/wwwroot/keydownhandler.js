export function makeHandler(component, handlerMethodName, preventDefaultKeys) {
    return new KeyDownHandler(component, handlerMethodName, preventDefaultKeys);
}

class KeyDownHandler {
    constructor(component, handlerMethodName, preventDefaultKeys) {
        this.component = component;
        this.preventDefaultKeys = preventDefaultKeys;
        this.handlerMethodName = handlerMethodName;
    }

    elements = [];

    registerElement = elementRef => {
        if (elementRef === null) return;
        for (let i = 0; i < this.elements.length; i++) {
            if (this.elements[i] == elementRef) {
                return;
            }
        }
        this.elements.push(elementRef);
        elementRef.addEventListener('keydown', this.handleKeyDown);
    }

    unregisterElement = elementRef => {
        if (elementRef === null) return;
        for (let i = 0; i < this.elements.length; i++) {
            if (this.elements[i] == elementRef) {
                this.unregisterElementByIndex(i);
                return;
            }
        }
    }

    unregisterElementByIndex = index => {
        const element = this.elements[index];
        element.removeEventListener('keydown', this.handleKeyDown);
        this.elements.splice(index, 1);
        return;
    }

    unmount = () => {
        for (let i = this.elements.length - 1; i >= 0; i--) {
            this.unregisterElementByIndex(i);
        }
    }

    handleKeyDown = event => {
        for (let i = 0; i < this.preventDefaultKeys.length; i++) {
            if (this.preventDefaultKeys[i] == event.key) {
                event.preventDefault();
                break;
            }
        }
        this.component.invokeMethodAsync(this.handlerMethodName, {
            type: event.type,
            key: event.key,
            code: event.code,
            location: event.location,
            repeat: event.repeat,
            ctrlKey: event.ctrlKey,
            shiftKey: event.shiftKey,
            altKey: event.altKey,
            metaKey: event.metaKey,
        });
    }
}
