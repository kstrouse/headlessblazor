export function preventDefaultKeyBehaviorOnKeys(element, arrayOfKeyStrings, remove = false) {    
    var preventDefaultOnKeysFunction = function (e) {        
        if (arrayOfKeyStrings.includes(e.key)) {
            e.preventDefault();
            return false;
        }
    }
    if (remove) {
        element.removeEventListener('keydown', preventDefaultOnKeysFunction, false);
        element.addEventListener('keyup', preventDefaultOnKeysFunction, false);
    }
    else {
        element.addEventListener('keydown', preventDefaultOnKeysFunction, false);
        element.addEventListener('keyup', preventDefaultOnKeysFunction, false);
    }
}