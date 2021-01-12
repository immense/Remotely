function setViewportWidth() {
    if (window.screen.orientation.type.includes("portrait")) {
        var desiredWidth = Math.max(550, window.screen.width);
        document.querySelector('meta[name="viewport"').setAttribute("content", `width=${desiredWidth}, user-scalable=no`);
    }
    else {
        var desiredHeight = Math.max(700, window.screen.height);
        document.querySelector('meta[name="viewport"').setAttribute("content", `width=device-width, height=${desiredHeight}, user-scalable=no`);
    }
}

setViewportWidth();
window.addEventListener("orientationchange", setViewportWidth);