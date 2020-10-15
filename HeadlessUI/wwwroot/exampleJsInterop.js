// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

window.headlessUI = {
    clickAwayHandlers: {},
    registerOnClickAway: function (id, el, callBackObject, callBackMethod) {
        function handler(event) {
            if (!el.contains(event.target)) {
                callBackObject.invokeMethodAsync(callBackMethod, event);
            }
        }
        headlessUI.clickAwayHandlers[id] = handler;
        window.addEventListener('click', headlessUI.clickAwayHandlers[id]);
    },
    unregisterOnClickAway: function (id) {
        window.removeEventListener('click', headlessUI.clickAwayHandlers[id]);
        delete headlessUI.clickAwayHandlers[id];
    },
    showPrompt: function (message) {
        return prompt(message, 'Type anything here');
    },
    showAlert: function (message) {
        return alert(message);
    }
};
