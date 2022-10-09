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
            for (let i = 0; i < entries.length; i++) {

                let entry = entries[i];

                let boundaryTuple = intersectionObserverMapValue.BoundaryIdIntersectionRatioTuples
                    .find(x => x.BoundaryId === entry.target.id);

                boundaryTuple.IsIntersecting = entry.isIntersecting;

                if (boundaryTuple.IsIntersecting) {
                    hasIntersectingBoundary = true;
                }
            }

            if (hasIntersectingBoundary) {
                virtualizationDisplayDotNetObjectReference
                    .invokeMethodAsync("OnScrollEventAsync", {
                        ScrollLeftInPixels: scrollableParent.scrollLeft,
                        ScrollTopInPixels: scrollableParent.scrollTop
                    });
            }
        }, options);

        let boundaryIdIntersectionRatioTuples = [];

        for (let i = 0; i < boundaryIds.length; i++) {

            let boundaryElement = document.getElementById(boundaryIds[i]);

            intersectionObserver.observe(boundaryElement);

            boundaryIdIntersectionRatioTuples.push();
        }

        this.intersectionObserverMap.set(intersectionObserverMapKey, {
            IntersectionObserver: intersectionObserver,
            CursorIsIntersectingTuples: {
                BoundaryId: cursorElementId,
                IsIntersecting: false
            }
        });

        virtualizationDisplayDotNetObjectReference
            .invokeMethodAsync("OnScrollEventAsync", {
                ScrollLeftInPixels: scrollableParent.scrollLeft,
                ScrollTopInPixels: scrollableParent.scrollTop
            });
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