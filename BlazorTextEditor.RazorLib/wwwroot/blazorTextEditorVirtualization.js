window.blazorTextEditorVirtualization = {
    intersectionObserverMap: new Map(),
    initializeIntersectionObserver: function (intersectionObserverMapKey,
                                              virtualizationDisplayDotNetObjectReference,
                                              scrollableParentFinder,
                                              boundaryIds) {
        
        let scrollableParent = scrollableParentFinder.parentElement;
        
        scrollableParent.addEventListener("scroll", (event) => {
            let hasIntersectingBoundary = false;

            let intersectionObserverMapValue = this.intersectionObserverMap
                .get(intersectionObserverMapKey);
            
            for (let i = 0; i < intersectionObserverMapValue.BoundaryIdIntersectionRatioTuples.length; i++) {
                let boundaryTuple = intersectionObserverMapValue.BoundaryIdIntersectionRatioTuples[i];

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
        }, true);

        let options = {
            root: scrollableParent,
            rootMargin: '0px',
            threshold: 0
        }

        let intersectionObserver = new IntersectionObserver((entries) => {
            let hasIntersectingBoundary = false;
            
            let intersectionObserverMapValue = this.intersectionObserverMap
                .get(intersectionObserverMapKey);
            
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
            
            boundaryIdIntersectionRatioTuples.push({
                BoundaryId: boundaryIds[i],
                IsIntersecting: false
            });
        }

        this.intersectionObserverMap.set(intersectionObserverMapKey, {
            IntersectionObserver: intersectionObserver,
            BoundaryIdIntersectionRatioTuples: boundaryIdIntersectionRatioTuples
        });

        virtualizationDisplayDotNetObjectReference
            .invokeMethodAsync("OnScrollEventAsync", {
                ScrollLeftInPixels: scrollableParent.scrollLeft,
                ScrollTopInPixels: scrollableParent.scrollTop
            });
    },
    disposeIntersectionObserver: function (intersectionObserverMapKey) {
        
        // TODO: Wrong
        
        let intersectionObserver = this.intersectionObserverMap.get(intersectionObserverMapKey);

        this.intersectionObserverMap.delete(intersectionObserverMapKey);
        
        intersectionObserver.disconnect();
    }
}
