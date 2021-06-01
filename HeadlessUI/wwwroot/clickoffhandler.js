export function makeHandler(component, handlerMethodName) {
    return new ClickOffHandler(component, handlerMethodName);
}

class ClickOffHandler {
    constructor(component, handlerMethodName) {
        this.component = component;
        this.handlerMethodName = handlerMethodName;
        this.mount();
    }

    debugEnabled = false;

    elements = [];

    registerElement = elementRef => {
        if (elementRef === null) return;
        for (let i = 0; i < this.elements.length; i++) {
            if (this.elements[i] == elementRef) {
                return;
            }
        }
        this.logDebugMessage("Click off handler: Registering element", elementRef);
        this.elements.push(elementRef);
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
        this.logDebugMessage("Click off handler: Unregistering element", this.elements[index]);
        this.elements.splice(index, 1);
        this.logDebugMessage("Elements After:", this.elements);
        return;
    }

    handleWindowOnClick = e => {
        if (this.elements.length == 0) return;
        this.logDebugMessage("Target: ", e.target);
        let isClickOff = true;
        for (let i = this.elements.length - 1; i >= 0; i--) {
            this.logDebugMessage(this.elements[i]);
            if (this.elements[i].contains(event.target)) {
                this.logDebugMessage("FOUND");
                isClickOff = false;
            }
            if (!document.contains(this.elements[i])) {
                this.unregisterElementByIndex(i);
            }
        }
        if (isClickOff) this.onClickOff();
    }

    onClickOff = () => {
        this.logDebugMessage("Click off handler: clicked off", this.elements);
        this.component.invokeMethodAsync(this.handlerMethodName);
    }

    mount = () => {
        this.logDebugMessage("Mounting click off handler");
        window.addEventListener('click', this.handleWindowOnClick);
    }

    unmount = () => {
        this.logDebugMessage("Unmounting click off handler");
        window.removeEventListener('click', this.handleWindowOnClick);
        for (let i = this.elements.length - 1; i >= 0; i--) {
            this.unregisterElementByIndex(i);
        }
    }

    logDebugMessage = (message, ...data) => {
        if (this.debugEnabled)
            if (data.length > 0)
                console.log(message, data)
            else
                console.log(message)
    }
}
