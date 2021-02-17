import { ShowMessage, ShowModal } from "../Shared/UI.js";

export const OrganizationManagement = {
    Init() {


        document.getElementById("usersHelpButton").addEventListener("click", () => {
            ShowModal("Users", `All users for the organization are managed here.<br><br>
                Administrators will have access to this management screen as well as all computers.`);
        });
        document.getElementById("invitesHelpButton").addEventListener("click", () => {
            ShowModal("Invitations", `All pending invitations will be shown here and can be revoked by deleting them.<br><br>
                If a user does not exist, sending an invite will create their account and add them to the current organization.
                A password reset URL can be generated from the user table.
                <br><br>
                The Admin checkbox determines if the new user will have administrator privileges in this organization.`);
        });

        document.getElementById("deviceGroupHelpButton").addEventListener("click", () => {
            ShowModal("Device Groups", `Device groups can be used to organize and filter computers on the grid.`);
        });


        document.getElementById("defaultOrgHelp").addEventListener("click", () => {
            ShowModal("Default Organization", `This option is only available for server administrators.  When
                selected, it sets this organization as the default for the server.  If the organization can't
                be determined in the quick support apps, they will use the default organization's branding.`);
        });

        document.getElementById("relayCodeHelp").addEventListener("click", () => {
            ShowModal("Relay Code", `This relay code will be appended to EXE filenames.  If the clients
                were built from source and have the server URL embedded, they will use this code to look
                up the branding to use for your organization.`);
        });


        document.getElementById("addUsersToDeviceGroupButton").addEventListener("click", (ev) => {
            var selectList = document.getElementById("deviceGroupList") as HTMLSelectElement;
            if (selectList.selectedOptions.length == 0) {
                return;
            }
            if (selectList.selectedOptions.length > 1) {
                ShowModal("Device Group Users", "You can only edit users for 1 device group at a time.");
                return;
            }

            var groupID = selectList.selectedOptions[0].value;
            var modalDiv = document.querySelector(`.modal[group='${groupID}']`) as HTMLDivElement;
            $(modalDiv).modal("show");
        });

        document.getElementById("removeDeviceGroupButton").addEventListener("click", (ev) => {
            var selectList = document.getElementById("deviceGroupList") as HTMLSelectElement;
            var selectedValues = [];
            for (var i = 0; i < selectList.selectedOptions.length; i++) {
                selectedValues.push(selectList.selectedOptions[i].value);
            }

            selectedValues.forEach(x => {
                let xhr = new XMLHttpRequest();
                xhr.onload = (ev) => {
                    console.log(ev.srcElement);
                    if (xhr.status == 200) {
                        document.querySelector(`#deviceGroupList option[value='${x}']`).remove();
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
                xhr.send(JSON.stringify(x));
            })
        });
        document.getElementById("deviceGroupInput").addEventListener("keypress", (e) => {
            if (e.key.toLowerCase() == "enter") {
                document.getElementById("addDeviceGroupButton").click();
            }
        })

        document.querySelectorAll(".remove-user-from-device-group-button").forEach((x: HTMLElement) => {
            x.addEventListener("click", clickEv => {
                var groupID = (clickEv.currentTarget as HTMLButtonElement).getAttribute("group");
                var selectList = document.querySelector(`.modal[group='${groupID}'] select.device-group-user-list`) as HTMLSelectElement;
                var selectedValues = [];
                for (var i = 0; i < selectList.selectedOptions.length; i++) {
                    selectedValues.push(selectList.selectedOptions[i].value);
                }

                selectedValues.forEach(user => {
                    let xhr = new XMLHttpRequest();
                    xhr.onload = (ev) => {
                        console.log(ev.srcElement);
                        if (xhr.status == 200) {
                            selectList.querySelector(`option[value='${user}']`).remove()
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
                    xhr.open("delete", location.origin + `/api/OrganizationManagement/DeviceGroup/${groupID}/Users/`);
                    xhr.setRequestHeader("Content-Type", "application/json");
                    xhr.send(JSON.stringify(user));
                })
            })
        });

        document.querySelectorAll(".add-user-to-devicegroup-button").forEach((x: HTMLElement) => {
            x.addEventListener("click", (clickEv) => {
                var groupID = (clickEv.currentTarget as HTMLButtonElement).getAttribute("group");
                var modal = document.querySelector(`.modal[group='${groupID}']`);
                var selectList = modal.querySelector(`select.device-group-user-list`) as HTMLSelectElement;
                var userInput = modal.querySelector(`input.add-user-to-devicegroup-input`) as HTMLInputElement;

                let xhr = new XMLHttpRequest();
                xhr.onload = (ev) => {
                    console.log(ev.srcElement);
                    if (xhr.status == 200) {
                        var option = document.createElement("option");
                        option.value = xhr.responseText;
                        option.text = userInput.value;
                        selectList.options.add(option)
                        userInput.value = "";
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
                xhr.open("post", location.origin + `/api/OrganizationManagement/DeviceGroup/${groupID}/Users/`);
                xhr.setRequestHeader("Content-Type", "application/json");
                xhr.send(JSON.stringify(userInput.value));
            });
        })


        document.querySelectorAll(".add-user-to-devicegroup-input").forEach(x => {
            x.addEventListener("keypress", (ev: KeyboardEvent) => {
                if (ev.key.toLowerCase() == "enter") {
                    var groupID = (ev.currentTarget as HTMLInputElement).getAttribute("group");
                    (document.querySelector(`.add-user-to-devicegroup-button[group='${groupID}']`) as HTMLButtonElement).click();
                }
            })
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

        document.getElementById("defaultOrgCheckbox").addEventListener("change", (ev) => {
            var xhr = new XMLHttpRequest();
            xhr.onload = () => {
                if (xhr.status == 200) {
                    ShowMessage("Default organization set.");
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
            xhr.open("put", location.origin + "/api/OrganizationManagement/SetDefault");
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.send(JSON.stringify((ev.currentTarget as HTMLInputElement).checked));
        });


        document.querySelectorAll(".user-is-admin-checkbox").forEach((checkbox: HTMLInputElement) => {
            checkbox.addEventListener("change", (ev) => {
                var userID = checkbox.getAttribute("user");
                var xhr = new XMLHttpRequest();
                xhr.onload = () => {
                    if (xhr.status == 400) {
                        ShowModal("Invalid Request", xhr.responseText);
                    }
                    else if (xhr.status != 200) {
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

    }
}

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