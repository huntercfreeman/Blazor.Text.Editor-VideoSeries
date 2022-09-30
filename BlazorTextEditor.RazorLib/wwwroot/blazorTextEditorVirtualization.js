window.blazorTextEditorVirtualization = {
    intersectionObserverMap: new Map(),
    initializeIntersectionObserver: function (intersectionObserverMapKey,
                                              virtualizationDisplayDotNetObjectReference,
                                              scrollableParentFinder,
                                              boundaryIds) {
        
        let scrollableParent = scrollableParentFinder.parentElement;
        
        scrollableParent.addEventListener("scroll", (event) => {
            // TODO: Add scroll events    
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

                boundaryTuple.IntersectionRatio = entry.intersectionRatio;
                
                if (boundaryTuple.IntersectionRatio > 0) {
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
            boundaryIdIntersectionRatioTuples.push({
                BoundaryId: boundaryIds[i],
                IntersectionRatio: 0
            });
        }

        this.intersectionObserverMap.set(intersectionObserverMapKey, {
            IntersectionObserver: intersectionObserver,
            BoundaryIdIntersectionRatioTuples: boundaryIdIntersectionRatioTuples
        });
    },
    disposeIntersectionObserver: function (intersectionObserverMapKey) {
        
        let intersectionObserver = this.intersectionObserverMap.get(intersectionObserverMapKey);

        this.intersectionObserverMap.delete(intersectionObserverMapKey);
        
        intersectionObserver.disconnect();
    }
}
