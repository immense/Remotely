var serverConfigForm = document.getElementById("serverConfigForm") as HTMLFormElement;
var serverConfigSaveButton = document.getElementById("serverConfigSaveButton") as HTMLButtonElement;

var bannedDevicesAddButton = document.getElementById("bannedDevicesAddButton") as HTMLButtonElement;
var bannedDevicesRemoveButton = document.getElementById("bannedDevicesRemoveButton") as HTMLButtonElement;
var bannedDevicesInput = document.getElementById("bannedDevicesInput") as HTMLInputElement;
var bannedDevicesSelect = document.getElementById("bannedDevicesSelect") as HTMLSelectElement;

var trustedCorsAddButton = document.getElementById("trustedCorsAddButton") as HTMLButtonElement;
var trustedCorsRemoveButton = document.getElementById("trustedCorsRemoveButton") as HTMLButtonElement;
var trustedCorsInput = document.getElementById("trustedCorsInput") as HTMLInputElement;
var trustedCorsSelect = document.getElementById("trustedCorsSelect") as HTMLSelectElement;

var knownProxiesAddButton = document.getElementById("knownProxiesAddButton") as HTMLButtonElement;
var knownProxiesRemoveButton = document.getElementById("knownProxiesRemoveButton") as HTMLButtonElement;
var knownProxiesInput = document.getElementById("knownProxiesInput") as HTMLInputElement;
var knownProxiesSelect = document.getElementById("knownProxiesSelect") as HTMLSelectElement;

var serverAdminsAddButton = document.getElementById("serverAdminsAddButton") as HTMLButtonElement;
var serverAdminsRemoveButton = document.getElementById("serverAdminsRemoveButton") as HTMLButtonElement;
var serverAdminsInput = document.getElementById("serverAdminsInput") as HTMLInputElement;
var serverAdminsSelect = document.getElementById("serverAdminsSelect") as HTMLSelectElement;

export const ServerConfig = {
    Init() {
        serverConfigSaveButton.addEventListener("click", e => {
            for (var i = 0; i < bannedDevicesSelect.options.length; i++) {
                bannedDevicesSelect.options[i].selected = true;
            }
            for (var i = 0; i < trustedCorsSelect.options.length; i++) {
                trustedCorsSelect.options[i].selected = true;
            }
            for (var i = 0; i < knownProxiesSelect.options.length; i++) {
                knownProxiesSelect.options[i].selected = true;
            }
            for (var i = 0; i < serverAdminsSelect.options.length; i++) {
                serverAdminsSelect.options[i].selected = true;
            }

            serverConfigForm.submit();
        });

        bannedDevicesAddButton.addEventListener("click", ev => {
            if (bannedDevicesInput.value.length > 0) {
                var option = document.createElement("option");
                option.value = bannedDevicesInput.value;
                option.text = bannedDevicesInput.value;
                bannedDevicesSelect.add(option);
                bannedDevicesInput.value = "";
            }
        });

        bannedDevicesInput.addEventListener("keypress", ev => {
            if (ev.key.toLowerCase() == "enter") {
                ev.preventDefault();
                ev.stopPropagation();
                bannedDevicesAddButton.click();
            }
        })

        bannedDevicesRemoveButton.addEventListener("click", ev => {
            while (bannedDevicesSelect.selectedOptions.length > 0) {
                bannedDevicesSelect.selectedOptions[0].remove();
            }
        });

        trustedCorsAddButton.addEventListener("click", ev => {
            if (trustedCorsInput.value.length > 0) {
                var option = document.createElement("option");
                option.value = trustedCorsInput.value;
                option.text = trustedCorsInput.value;
                trustedCorsSelect.add(option);
                trustedCorsInput.value = "";
            }
        });

        trustedCorsInput.addEventListener("keypress", ev => {
            if (ev.key.toLowerCase() == "enter") {
                ev.preventDefault();
                ev.stopPropagation();
                trustedCorsAddButton.click();
            }
        })

        trustedCorsRemoveButton.addEventListener("click", ev => {
            while (trustedCorsSelect.selectedOptions.length > 0) {
                trustedCorsSelect.selectedOptions[0].remove();
            }
        });


        knownProxiesAddButton.addEventListener("click", ev => {
            if (knownProxiesInput.value.length > 0) {
                var option = document.createElement("option");
                option.value = knownProxiesInput.value;
                option.text = knownProxiesInput.value;
                knownProxiesSelect.add(option);
                knownProxiesInput.value = "";
            }
        });

        knownProxiesInput.addEventListener("keypress", ev => {
            if (ev.key.toLowerCase() == "enter") {
                ev.preventDefault();
                ev.stopPropagation();
                knownProxiesAddButton.click();
            }
        })

        knownProxiesRemoveButton.addEventListener("click", ev => {
            while (knownProxiesSelect.selectedOptions.length > 0) {
                knownProxiesSelect.selectedOptions[0].remove();
            }
        });


        serverAdminsAddButton.addEventListener("click", ev => {
            if (serverAdminsInput.value.length > 0) {
                var option = document.createElement("option");
                option.value = serverAdminsInput.value;
                option.text = serverAdminsInput.value;
                serverAdminsSelect.add(option);
                serverAdminsInput.value = "";
            }
        });

        serverAdminsInput.addEventListener("keypress", ev => {
            if (ev.key.toLowerCase() == "enter") {
                ev.preventDefault();
                ev.stopPropagation();
                serverAdminsAddButton.click();
            }
        })

        serverAdminsRemoveButton.addEventListener("click", ev => {
            while (serverAdminsSelect.selectedOptions.length > 0) {
                serverAdminsSelect.selectedOptions[0].remove();
            }
        });

    }
}