// Having a service worker is necessary to support Share Target API. We don't
// actually need the worker for anything.
self.addEventListener("fetch", (event) => {
    event.respondWith(
        fetch(event.request)
            .then(response => response)
            .catch(err => new Response("Error: Unable to fetch. This is most likely a network error, i.e. you're not connected to the internet or the site is offline. " + JSON.stringify(err)))    
    );
});
