window.blazorTextEditor = {
    intersectionObserverMap: new Map(),
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
            RelativeY: y,
            RelativeScrollLeft: element.scrollLeft,
            RelativeScrollTop: element.scrollTop
        }
    },
    initializeTextEditorCursorIntersectionObserver: function (intersectionObserverMapKey,
                                              scrollableContainerElementId,
                                              cursorElementId) {

        let scrollableContainer = document.getElementById(scrollableContainerElementId);

        let options = {
            root: scrollableContainer,
            rootMargin: '0px',
            threshold: 0
        }

        let intersectionObserver = new IntersectionObserver((entries) => {
            let intersectionObserverMapValue = this.intersectionObserverMap
                .get(intersectionObserverMapKey);
            
            for (let i = 0; i < entries.length; i++) {

                let entry = entries[i];

                let cursorTuple = intersectionObserverMapValue.CursorIsIntersectingTuples
                    .find(x => x.CursorElementId === entry.target.id);

                cursorTuple.IsIntersecting = entry.isIntersecting;
            }
        }, options);

        let cursorIsIntersectingTuples = [];

        let cursorElement = document.getElementById(cursorElementId);

        intersectionObserver.observe(cursorElement);

        cursorIsIntersectingTuples.push({
            CursorElementId: cursorElementId,
            IsIntersecting: false
        });

        this.intersectionObserverMap.set(intersectionObserverMapKey, {
            IntersectionObserver: intersectionObserver,
            CursorIsIntersectingTuples: cursorIsIntersectingTuples
        });
    },
    revealCursor: function (intersectionObserverMapKey,
                            cursorElementId) {

        let intersectionObserverMapValue = this.intersectionObserverMap
            .get(intersectionObserverMapKey);

        let cursorTuple = intersectionObserverMapValue.CursorIsIntersectingTuples
            .find(x => x.CursorElementId === cursorElementId);
        
        if (!cursorTuple.IsIntersecting) {
            let cursorElement = document.getElementById(cursorElementId);
            
            cursorElement.scrollIntoView({
                block: "nearest", 
                inline: "nearest"
            });
        }
    },
    disposeTextEditorCursorIntersectionObserver: function (intersectionObserverMapKey) {

        // TODO: Add dispose
    }
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