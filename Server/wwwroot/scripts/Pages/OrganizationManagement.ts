import { ShowModal, ValidateInput, PopupMessage } from "../UI.js";


document.getElementById("usersHelpButton").addEventListener("click", (ev) => {
    ShowModal("Users", `All users for the organization are managed here.<br><br>
        Administrators will have access to this management screen as well as all computers.`);
});
document.getElementById("invitesHelpButton").addEventListener("click", (ev) => {
    ShowModal("Invitations", `All pending invitations will be shown here and can be revoked by deleting them.<br><br>
        If a user does not exist, sending an invite will create their account and add them to the current organization.
        A password reset URL can be generated from the user table.
        <br><br>
        The Admin checkbox determines if the new user will have administrator privileges in this organization.`);
});

document.getElementById("deviceGroupHelpButton").addEventListener("click", (ev) => {
    ShowModal("Device Groups", `Device groups can be used to organize and filter computers on the grid.`);
});


document.getElementById("removeDeviceGroupButton").addEventListener("click", (ev) => {
    var selectList = document.getElementById("deviceGroupList") as HTMLSelectElement;
    for (var i = 0; i < selectList.selectedOptions.length; i++) {
        let selectedValue = selectList.selectedOptions[i].value;
        let xhr = new XMLHttpRequest();
        xhr.onload = (ev) => {
            console.log(ev.srcElement);
            if (xhr.status == 200) {
                document.querySelectorAll(`.all-device-groups-list option[value='${selectedValue}']`).forEach(option => {
                    option.remove();
                })
            }
            else if (xhr.status == 400) {
                ShowModal("Invalid Request", xhr.responseText);
            }
            else {
                showError(xhr);
            }
        }
        xhr.onerror = () => {
            showError(xhr);
        }
        xhr.open("delete", location.origin + "/api/OrganizationManagement/DeviceGroup");
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send(JSON.stringify(selectedValue));
    }

});
document.getElementById("deviceGroupInput").addEventListener("keypress", (e) => {
    if (e.key.toLowerCase() == "enter") {
        document.getElementById("addDeviceGroupButton").click();
    }
})
document.getElementById("addDeviceGroupButton").addEventListener("click", () => {
    var input = document.getElementById("deviceGroupInput") as HTMLInputElement;

    if (input.checkValidity() && input.value.length > 0) {
        var xhr = new XMLHttpRequest();
        xhr.onload = () => {
            if (xhr.status == 200) {
                document.querySelectorAll(`.all-device-groups-list`).forEach((list: HTMLSelectElement) => {
                    var newOption = new Option(input.value, xhr.responseText);
                    list.options.add(newOption);
                })
                input.value = "";
            }
            else if (xhr.status == 400) {
                ShowModal("Invalid Request", xhr.responseText);
            }
            else {
                showError(xhr);
            }
        }
        xhr.onerror = () => {
            showError(xhr);
        }
        xhr.open("post", location.origin + "/api/OrganizationManagement/DeviceGroup");
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send(JSON.stringify({ Name: input.value }));
    }
})

document.getElementById("organizationNameInput").addEventListener("input", (ev) => {
    var addon = (ev.currentTarget as HTMLInputElement).parentElement.querySelector(".fa");
    addon.classList.remove("fa-check-circle");
    addon.classList.add("fa-edit");
});
document.getElementById("organizationNameInput").addEventListener("blur", (ev) => {
    var xhr = new XMLHttpRequest();
    xhr.onload = () => {
        if (xhr.status == 200) {
            var addon = (ev.target as HTMLInputElement).parentElement.querySelector(".fa");
            addon.classList.remove("fa-edit");
            addon.classList.add("fa-check-circle");
        }
        else if (xhr.status == 400) {
            ShowModal("Invalid Request", xhr.responseText);
        }
        else {
            showError(xhr);
        }
    }
    xhr.onerror = () => {
        showError(xhr);
    }
    xhr.open("put", location.origin + "/api/OrganizationManagement/Name");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(JSON.stringify((ev.currentTarget as HTMLInputElement).value));
});


document.querySelectorAll(".user-is-admin-checkbox").forEach((checkbox: HTMLInputElement) => {
    checkbox.addEventListener("change", (ev) => {
        var userID = checkbox.getAttribute("user");
        var xhr = new XMLHttpRequest();
        xhr.onload = () => {
            if (xhr.status == 200) {
                
            }
            else if (xhr.status == 400) {
                ShowModal("Invalid Request", xhr.responseText);
            }
            else {
                showError(xhr);
            }
        }
        xhr.onerror = () => {
            showError(xhr);
        }
        xhr.open("post", location.origin + `/api/OrganizationManagement/ChangeIsAdmin/${userID}`);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send(JSON.stringify((ev.currentTarget as HTMLInputElement).checked));
    })
});
document.querySelectorAll(".reset-password-button").forEach((resetButton: HTMLButtonElement) => {
    resetButton.addEventListener("click", (ev) => {
        var userID = resetButton.getAttribute("user");
        var xhr = new XMLHttpRequest();
        xhr.onload = () => {
            if (xhr.status == 200) {
                ShowModal("Password Reset", 
                `<div class="mb-3">
                    <span>Password Reset URL: </span>
                    <a target="_blank" href="${xhr.responseText}">Copy This Link</a>
                </div>

                <div>
                    NOTE: You must log out before visiting the reset URL.  It's only valid for the selected user.
                </div>
                `)
            }
            else if (xhr.status == 400) {
                ShowModal("Invalid Request", xhr.responseText);
            }
            else {
                showError(xhr);
            }
        }
        xhr.onerror = () => {
            showError(xhr);
        }
        xhr.open("get", `${location.origin}/api/OrganizationManagement/GenerateResetUrl/${userID}`);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send();
    })
});
document.querySelectorAll(".delete-user-button").forEach((removeButton: HTMLButtonElement) => {
    removeButton.addEventListener("click", (ev) => {
        var result = confirm("Are you sure you want to delete this user?");
        if (result) {
            var userID = removeButton.getAttribute("user");
            var xhr = new XMLHttpRequest();
            xhr.onload = () => {
                if (xhr.status == 200) {
                    document.querySelector(`tr[user='${userID}']`).remove();
                }
                else if (xhr.status == 400) {
                    ShowModal("Invalid Request", xhr.responseText);
                }
                else {
                    showError(xhr);
                }
            }
            xhr.onerror = () => {
                showError(xhr);
            }
            xhr.open("delete", `${location.origin}/api/OrganizationManagement/DeleteUser/${userID}`);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.send();
        }
    })
});

document.querySelectorAll(".delete-invite-button").forEach((deleteButton: HTMLButtonElement) => {
    deleteButton.addEventListener("click", (ev) => {
        deleteInvite(ev);
    })
})

function deleteInvite(ev: MouseEvent) {
    var inviteID = (ev.currentTarget as HTMLButtonElement).getAttribute("invite");
    var xhr = new XMLHttpRequest();
    xhr.onload = () => {
        if (xhr.status == 200) {
            var row = document.querySelector(`tr[invite='${inviteID}']`);
            row.remove();
        }
        else if (xhr.status == 400) {
            ShowModal("Invalid Request", xhr.responseText);
        }
        else {
            showError(xhr);
        }
    }
    xhr.onerror = () => {
        showError(xhr);
    }
    xhr.open("delete", location.origin + `/api/OrganizationManagement/DeleteInvite/${inviteID}`);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send();
}
function showError(xhr: XMLHttpRequest) {
    console.error(xhr);
    ShowModal("Error", "There was an error saving the data.", "", () => { location.reload(); });
}