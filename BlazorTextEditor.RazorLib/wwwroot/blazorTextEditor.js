window.blazorTextEditor = {
    measureFontWidthAndElementHeightByElementId: function (elementId, amountOfCharactersRendered) {
        let element = document.getElementById(elementId);
        
        let fontWidth = element.offsetWidth / amountOfCharactersRendered;
        
        return {
            FontWidthInPixels: fontWidth,
            ElementHeightInPixels: element.offsetHeight
        }
    },
    measureWidthAndHeightByElementId: function (elementId) {
        let element = document.getElementById(elementId);
        
        return {
            WidthInPixels: element.offsetWidth,
            HeightInPixels: element.offsetHeight
        }
    },
    getRelativePosition: function (elementId, clientX, clientY) {
        let element = document.getElementById(elementId);
        
        let bounds = element.getBoundingClientRect();
        
        let x = clientX - bounds.left;
        let y = clientY - bounds.top;
        
        return {
            RelativeX: x,
            RelativeY: y
        }
    },
}

Blazor.registerCustomEventType('customkeydown', {
    browserEventName: 'keydown',
    createEventArgs: e => {
        if (e.code !== "Tab") {
            e.preventDefault();
        }

        return {
            "key": e.key,
            "code": e.code,
            "ctrlWasPressed": e.ctrlKey,
            "shiftWasPressed": e.shiftKey,
            "altWasPressed": e.altKey
        };
    }
});