// Having a service worker is necessary to support Share Target API. We don't
// actually need the worker for anything.
self.addEventListener("fetch", (event) => {
    console.log("Service worker: fetch: %o", event);
});
