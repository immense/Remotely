window.addClassName = (element, className) => {
    element.classList.add(className);
}

window.getSelectionStart = (element) => {
    return element.selectionStart;
}

window.invokeClick = elementId => {
    document.getElementById(elementId).click();
}

window.invokeConfirm = async (message) => {
    return confirm(message);
}

window.invokeAlert = async (message) => {
    alert(message);
}

window.invokePrompt = async (message) => {
    return prompt(message);
}

window.preventTabOut = (element) => {
    element.addEventListener("keydown", ev => {
        if (ev.key.toLowerCase() == "tab") {
            ev.preventDefault();
        }
    })
}
window.scrollToEnd = (element) => {
    if (!element) {
        return;
    }

    element.scrollTop = element.scrollHeight;
}
window.scrollToElement = (element) => {
    window.setTimeout(() => {
        window.scrollTo({ top: element.offsetTop, behavior: "smooth"});
    }, 200);

}
window.setStyleProperty = (element, propertyName, value) => {
    element.style[propertyName] = value;
}

window.startDraggingY = (element, clientY) => {
    if (!element) {
        return;
    }

    var startTop = Number(clientY);

    function pointerMove(ev) {
        if (Math.abs(ev.clientY - startTop) > 10) {
            if (ev.clientY < 0 || ev.clientY > window.innerHeight - element.clientHeight) {
                return;
            }

            element.style.top = `${ev.clientY}px`;
        }
    }

    function pointerUpOrLeave(ev) {
        window.removeEventListener("pointermove", pointerMove);
        window.removeEventListener("pointerup", pointerUpOrLeave);
        window.removeEventListener("pointerleave", pointerUpOrLeave);
    }

    pointerUpOrLeave();

    window.addEventListener("pointermove", pointerMove);
    window.addEventListener("pointerup", pointerUpOrLeave);
    window.addEventListener("pointerleave", pointerUpOrLeave);
}

window.autoHeight = () => {
    document.querySelectorAll(".auto-height").forEach(x => {
        var desiredHeight = window.innerHeight - x.getBoundingClientRect().top;
        x.style.height = `${desiredHeight}px`;
        x.style.opacity = 1;
    })
}

window.openWindow = (url, target) => {
    window.open(url, target);
}

window.addEventListener("load", () => {
    window.addEventListener("resize", () => {
        autoHeight();
    });
    autoHeight();
})

window.addBeforeUnloadHandler = () => {
    window.addEventListener("beforeunload", (ev) => {
        if (location.href.indexOf("localhost") == -1) {
            // All three methods have been implemented in different browsers at some point.
            // Might as well cover all bases.
            ev.preventDefault();
            var msg = "This will disconnect your session.  Are you sure you want to exit?";
            ev.returnValue = msg;
            return msg;
        }
    })
}