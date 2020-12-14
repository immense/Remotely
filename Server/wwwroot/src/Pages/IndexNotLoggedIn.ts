export const IndexNotLoggedIn = {
    Init() {
        var header = document.getElementById("remotelyHeader") as HTMLHeadingElement;

        typeText(text, header);
    }
}

function typeText(textParts: string[], header: HTMLHeadingElement) {
    var currentText = textParts[0];
    textParts[0] = currentText.slice(1);

    if (currentText.length > 0) {
        if (currentText.split('').some(x => x != " ")) {
            header.innerHTML += currentText.charAt(0);
            window.setTimeout(() => {
                typeText(textParts, header);
            }, 100)
        }
        else {
            header.innerHTML = header.innerHTML.slice(0, -1);
            window.setTimeout(() => {
                typeText(textParts, header);
            }, 50)
        }
    }
    else {
        textParts.shift();

        if (textParts.length > 0) {
            var timeout = 100;

            if (textParts[0].split('').every(x => x == " ")) {
                timeout = 800;
            }
            console.log(timeout);
            window.setTimeout(() => {
                typeText(textParts, header);
            }, timeout);
        }
    }
}

var text = [
    "Remote desktop",
    "        ",
    " scripting",
    "          ",
    "ly"
];