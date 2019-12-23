import { ShowModal, ValidateInput, PopupMessage } from "../UI.js";
document.getElementById("usersHelpButton").addEventListener("click", (ev) => {
    ShowModal("Users", `All users for the organization are managed here.<br><br>
        Administrators will have access to this management screen as well as all computers.`);
});
document.getElementById("invitesHelpButton").addEventListener("click", (ev) => {
    ShowModal("Invitations", `All pending invitations will be shown here and can be revoked by deleting them.<br><br>
        If a user does not exist, sending an invite will create their account and send them a password reset email too.
        The password reset must be completed before accepting the invitation.
        <br><br>
        The Admin checkbox determines if the new user will have administrator privileges in this organization
        after they accept the invitation.`);
});
document.getElementById("deviceGroupHelpButton").addEventListener("click", (ev) => {
    ShowModal("Device Groups", `Device groups can be used to organize and filter computers on the grid.`);
});
document.getElementById("removeDeviceGroupButton").addEventListener("click", (ev) => {
    var selectList = document.getElementById("deviceGroupList");
    for (var i = 0; i < selectList.selectedOptions.length; i++) {
        let selectedValue = selectList.selectedOptions[i].value;
        let xhr = new XMLHttpRequest();
        xhr.onload = (ev) => {
            console.log(ev.srcElement);
            if (xhr.status == 200) {
                document.querySelectorAll(`.all-device-groups-list option[value='${selectedValue}']`).forEach(option => {
                    option.remove();
                });
            }
            else if (xhr.status == 400) {
                ShowModal("Invalid Request", xhr.responseText);
            }
            else {
                showError(xhr);
            }
        };
        xhr.onerror = () => {
            showError(xhr);
        };
        xhr.open("delete", location.origin + "/api/OrganizationManagement/DeviceGroup");
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send(JSON.stringify(selectedValue));
    }
});
document.getElementById("deviceGroupInput").addEventListener("keypress", (e) => {
    if (e.key.toLowerCase() == "enter") {
        document.getElementById("addDeviceGroupButton").click();
    }
});
document.getElementById("addDeviceGroupButton").addEventListener("click", () => {
    var input = document.getElementById("deviceGroupInput");
    if (input.checkValidity() && input.value.length > 0) {
        var xhr = new XMLHttpRequest();
        xhr.onload = () => {
            if (xhr.status == 200) {
                document.querySelectorAll(`.all-device-groups-list`).forEach((list) => {
                    var newOption = new Option(input.value, xhr.responseText);
                    list.options.add(newOption);
                });
                input.value = "";
            }
            else if (xhr.status == 400) {
                ShowModal("Invalid Request", xhr.responseText);
            }
            else {
                showError(xhr);
            }
        };
        xhr.onerror = () => {
            showError(xhr);
        };
        xhr.open("post", location.origin + "/api/OrganizationManagement/DeviceGroup");
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send(JSON.stringify({ Name: input.value }));
    }
});
document.getElementById("organizationNameInput").addEventListener("input", (ev) => {
    var addon = ev.currentTarget.parentElement.querySelector(".fa");
    addon.classList.remove("fa-check-circle");
    addon.classList.add("fa-edit");
});
document.getElementById("organizationNameInput").addEventListener("blur", (ev) => {
    var xhr = new XMLHttpRequest();
    xhr.onload = () => {
        if (xhr.status == 200) {
            var addon = ev.target.parentElement.querySelector(".fa");
            addon.classList.remove("fa-edit");
            addon.classList.add("fa-check-circle");
        }
        else if (xhr.status == 400) {
            ShowModal("Invalid Request", xhr.responseText);
        }
        else {
            showError(xhr);
        }
    };
    xhr.onerror = () => {
        showError(xhr);
    };
    xhr.open("put", location.origin + "/api/OrganizationManagement/Name");
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(JSON.stringify(ev.currentTarget.value));
});
document.querySelectorAll(".user-is-admin-checkbox").forEach((checkbox) => {
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
        };
        xhr.onerror = () => {
            showError(xhr);
        };
        xhr.open("post", location.origin + `/api/OrganizationManagement/ChangeIsAdmin/${userID}`);
        xhr.setRequestHeader("Content-Type", "application/json");
        xhr.send(JSON.stringify(ev.currentTarget.checked));
    });
});
document.querySelectorAll(".remove-user-button").forEach((removeButton) => {
    removeButton.addEventListener("click", (ev) => {
        var result = confirm("Are you sure you want to remove this user from the organization?");
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
            };
            xhr.onerror = () => {
                showError(xhr);
            };
            xhr.open("delete", `${location.origin}/api/OrganizationManagement/RemoveUserFromOrganization/${userID}`);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.send();
        }
    });
});
document.getElementById("sendInviteButton").addEventListener("click", (ev) => {
    var inviteUserInput = document.querySelector("#inviteUserInput");
    if (!ValidateInput(inviteUserInput)) {
        return;
    }
    var invitedUser = inviteUserInput.value;
    inviteUserInput.value = "";
    var isAdmin = document.getElementById("inviteIsAdmin").checked;
    var xhr = new XMLHttpRequest();
    xhr.onload = () => {
        if (xhr.status == 200) {
            var newInvite = JSON.parse(xhr.responseText);
            var tbody = document.querySelector("#invitesTable tbody");
            var newRow = document.createElement("tr");
            newRow.setAttribute("invite", newInvite.ID);
            var innerHtml = `<td class="middle-aligned"><label class="control-label">${newInvite.InvitedUser}</label></td>
                                <td class="middle-aligned text-center"><input type="checkbox" disabled ${newInvite.IsAdmin ? "checked" : ""}/></td>
                                <td class="middle-aligned">
                                    <label class="control-label">
                                        <a href="${location.origin}/Invite/?id=${newInvite.ID}">Join Link</a>`;
            if (newInvite.ResetUrl) {
                innerHtml += `<br /> <a href="${newInvite.ResetUrl}">Reset Password</a>`;
            }
            innerHtml += ` </label> </td>
                                <td><button type="button" class="btn btn-danger delete-invite-button" invite="${newInvite.ID}">Delete</button></td>`;
            newRow.innerHTML = innerHtml;
            tbody.appendChild(newRow);
            newRow.querySelector(".delete-invite-button").addEventListener("click", (ev) => {
                deleteInvite(ev);
            });
        }
        else if (xhr.status == 400) {
            ShowModal("Invalid Request", xhr.responseText);
        }
        else {
            showError(xhr);
        }
    };
    xhr.onerror = () => {
        showError(xhr);
    };
    xhr.open("post", location.origin + `/api/OrganizationManagement/SendInvite/`);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send(JSON.stringify({ InvitedUser: invitedUser, IsAdmin: isAdmin }));
    PopupMessage("Sending invite...");
});
document.getElementById("inviteUserInput").addEventListener("keypress", (e) => {
    if (e.key.toLowerCase() == "enter") {
        document.getElementById("sendInviteButton").click();
    }
});
document.querySelectorAll(".delete-invite-button").forEach((deleteButton) => {
    deleteButton.addEventListener("click", (ev) => {
        deleteInvite(ev);
    });
});
function deleteInvite(ev) {
    var inviteID = ev.currentTarget.getAttribute("invite");
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
    };
    xhr.onerror = () => {
        showError(xhr);
    };
    xhr.open("delete", location.origin + `/api/OrganizationManagement/DeleteInvite/${inviteID}`);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.send();
}
function showError(xhr) {
    console.error(xhr);
    ShowModal("Error", "There was an error saving the data.", "", () => { location.reload(); });
}
//# sourceMappingURL=OrganizationManagement.js.map