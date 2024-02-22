using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Remotely.Server.Components.ModalContents;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using Remotely.Shared.ViewModels;
using System.Text;
using System.Text.Encodings.Web;

namespace Remotely.Server.Components.Pages;

public partial class ManageOrganization : AuthComponentBase
{
    private readonly List<DeviceGroup> _deviceGroups = new();
    private readonly List<InviteLink> _invites = new();
    private readonly List<RemotelyUser> _orgUsers = new();
    private bool _inviteAsAdmin;
    private string _inviteEmail = string.Empty;
    private bool _isLoading = true;
    private string _newDeviceGroupName = string.Empty;
    private Organization? _organization;
    private string _selectedDeviceGroupId = string.Empty;

    [Inject]
    private IDataService DataService { get; set; } = null!;

    [Inject]
    private IEmailSenderEx EmailSender { get; set; } = null!;

    [Inject]
    private IJsInterop JsInterop { get; set; } = null!;

    [Inject]
    private IModalService ModalService { get; set; } = null!;

    [Inject]
    private NavigationManager NavManager { get; set; } = null!;

    [Inject]
    private IToastService ToastService { get; set; } = null!;
    [Inject]
    private UserManager<RemotelyUser> UserManager { get; set; } = null!;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        await RefreshData();

        _isLoading = false;
    }

    private async Task CreateNewDeviceGroup()
    {
        EnsureUserSet();

        if (!User.IsAdministrator)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_newDeviceGroupName))
        {
            return;
        }

        var deviceGroup = new DeviceGroup()
        {
            Name = _newDeviceGroupName
        };

        var result = await DataService.AddDeviceGroup(User.OrganizationID, deviceGroup);
        if (!result.IsSuccess)
        {
            ToastService.ShowToast(result.Reason, classString: "bg-danger");
            return;
        }

        ToastService.ShowToast("Device group created.");
        _deviceGroups.Add(deviceGroup);
        _newDeviceGroupName = string.Empty;
    }

    private void DefaultOrgCheckChanged(ChangeEventArgs args)
    {
        if (_organization is null)
        {
            return;
        }

        EnsureUserSet();

        if (!User.IsServerAdmin)
        {
            return;
        }

        if (args.Value is not bool isDefault)
        {
            return;
        }

        DataService.SetIsDefaultOrganization(_organization.ID, isDefault);
        ToastService.ShowToast("Default organization set.");
    }

    private async Task DeleteInvite(InviteLink invite)
    {
        EnsureUserSet();

        if (!User.IsAdministrator)
        {
            return;
        }

        var result = await JsInterop.Confirm("Are you sure you want to delete this invitation?");
        if (!result)
        {
            return;
        }

        await DataService.DeleteInvite(User.OrganizationID, invite.ID);
        _invites.RemoveAll(x => x.ID == invite.ID);
        ToastService.ShowToast("Invitation deleted.");
    }

    private async Task DeleteSelectedDeviceGroup()
    {
        EnsureUserSet();

        if (!User.IsAdministrator)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_selectedDeviceGroupId))
        {
            return;
        }

        var result = await JsInterop.Confirm("Are you sure you want to delete this device group?");
        if (!result)
        {
            return;
        }

        await DataService.DeleteDeviceGroup(User.OrganizationID, _selectedDeviceGroupId);
        _deviceGroups.RemoveAll(x => x.ID == _selectedDeviceGroupId);
        _selectedDeviceGroupId = string.Empty;
    }

    private async Task DeleteUser(RemotelyUser user)
    {
        EnsureUserSet();

        if (!User.IsAdministrator)
        {
            return;
        }

        if (User.Id == user.Id)
        {
            ToastService.ShowToast("You can't delete yourself.", classString: "bg-warning");
            return;
        }

        var result = await JsInterop.Confirm("Are you sure you want to delete this user?");
        if (!result)
        {
            return;
        }

        await DataService.DeleteUser(User.OrganizationID, user.Id);
        _orgUsers.RemoveAll(x => x.Id == user.Id);
        ToastService.ShowToast("User deleted.");
    }

    private async Task EditDeviceGroups(RemotelyUser user)
    {
        void editDeviceGroupsModal(RenderTreeBuilder builder)
        {
            var deviceGroups = DataService.GetDeviceGroupsForOrganization(user.OrganizationID);

            builder.OpenComponent<EditDeviceGroup>(0);
            builder.AddAttribute(1, EditDeviceGroup.EditUserPropName, user);
            builder.AddAttribute(2, EditDeviceGroup.DeviceGroupsPropName, deviceGroups);
            builder.CloseComponent();
        }
        await ModalService.ShowModal("Device Groups", editDeviceGroupsModal);
    }

    private async Task EvaluateInviteInputKeypress(KeyboardEventArgs args)
    {
        if (args.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
        {
            await SendInvite();
        }
    }

    private async Task EvaluateNewDeviceGroupKeyPress(KeyboardEventArgs args)
    {
        if (args.Key.Equals("Enter", StringComparison.OrdinalIgnoreCase))
        {
            await CreateNewDeviceGroup();
        }
    }
    private void OrganizationNameChanged(ChangeEventArgs args)
    {
        if (_organization is null)
        {
            return;
        }

        EnsureUserSet();

        if (!User.IsAdministrator)
        {
            return;
        }

        if (args.Value is not string newName)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            return;
        }

        if (newName.Length > 25)
        {
            ToastService.ShowToast("Must be 25 characters or less.",
                classString: "bg-warning");
            return;
        }

        DataService.UpdateOrganizationName(_organization.ID, newName);
        _organization.OrganizationName = newName;
        ToastService.ShowToast("Organization name changed.");
    }

    private async Task RefreshData()
    {
        EnsureUserSet();

        var orgResult = await DataService.GetOrganizationByUserName(UserName);
        if (!orgResult.IsSuccess)
        {
            ToastService.ShowToast2(orgResult.Reason, Enums.ToastType.Warning);
            return;
        }

        _organization = orgResult.Value;
        _orgUsers.Clear();
        _invites.Clear();
        _deviceGroups.Clear();

        _invites.AddRange(DataService.GetAllInviteLinks(User.OrganizationID).OrderBy(x => x.InvitedUser));
        _deviceGroups.AddRange(DataService.GetDeviceGroups(UserName).OrderBy(x => x.Name));
        var orgUsers = await DataService.GetAllUsersInOrganization(User.OrganizationID);
        _orgUsers.AddRange(orgUsers.OrderBy(x => x.UserName));
    }
    private async Task ResetPassword(RemotelyUser user)
    {
        EnsureUserSet();

        if (!User.IsAdministrator)
        {
            return;
        }

        var code = await UserManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var resetUrl = $"{NavManager.BaseUri}Account/ResetPassword?code={code}";

        await ModalService.ShowModal("Password Reset", builder =>
        {
            builder.AddMarkupContent(0, $@"<div class=""mb-3"">Password Reset URL:</div>
                    <input readonly value=""{resetUrl}"" class=""form-control"" /> 
                    <div class=""mt-3"">NOTE: Give this URL to the user.  They must be logged out completely for it to work.</div>");
        });
    }

    private async Task SendInvite()
    {
        EnsureUserSet();

        if (!User.IsAdministrator)
        {
            return;
        }

        if (!DataService.DoesUserExist(_inviteEmail))
        {
            var result = await DataService.CreateUser(_inviteEmail, _inviteAsAdmin, User.OrganizationID);
            if (result.IsSuccess)
            {
                var userResult = await DataService.GetUserByName(_inviteEmail);
                if (!userResult.IsSuccess)
                {
                    ToastService.ShowToast2(userResult.Reason, Enums.ToastType.Warning);
                    return;
                }

                var user = userResult.Value;

                await UserManager.ConfirmEmailAsync(user, await UserManager.GenerateEmailConfirmationTokenAsync(user));

                _orgUsers.Add(user);

                _inviteAsAdmin = false;
                _inviteEmail = string.Empty;
                ToastService.ShowToast("User account created.");
                return;
            }
            else
            {
                ToastService.ShowToast("Create user failed.", classString: "bg-danger");
                return;
            }
        }
        else
        {

            var invite = new InviteViewModel()
            {
                InvitedUser = _inviteEmail,
                IsAdmin = _inviteAsAdmin
            };
            var newInvite = await DataService.AddInvite(User.OrganizationID, invite);

            if (!newInvite.IsSuccess)
            {
                ToastService.ShowToast($"Failed to create invite. {newInvite.Reason}", classString: "bg-danger");
                return;
            }

            var inviteURL = $"{NavManager.BaseUri}Invite/{newInvite.Value.ID}";
            var emailResult = await EmailSender.SendEmailAsync(invite.InvitedUser, "Invitation to Organization in Remotely",
                    $@"<img src='{NavManager.BaseUri}images/Remotely_Logo.png'/>
                            <br><br>
                            Hello!
                            <br><br>
                            You've been invited to join an organization in Remotely.
                            <br><br>
                            You can join the organization by <a href='{HtmlEncoder.Default.Encode(inviteURL)}'>clicking here</a>.",
                    User.OrganizationID);
            if (emailResult)
            {
                ToastService.ShowToast("Invitation sent.");

                _inviteAsAdmin = false;
                _inviteEmail = string.Empty;
                _invites.Add(newInvite.Value);
            }
            else
            {
                ToastService.ShowToast("Error sending invititation email.", classString: "bg-danger");
            }
        }
    }

    private async Task SetUserIsAdmin(ChangeEventArgs args, RemotelyUser orgUser)
    {
        EnsureUserSet();

        if (!User.IsAdministrator)
        {
            return;
        }

        if (args.Value is not bool isAdmin)
        {
            return;
        }

        await DataService.ChangeUserIsAdmin(User.OrganizationID, orgUser.Id, isAdmin);
        ToastService.ShowToast("Administrator value set.");
    }

    private void ShowDefaultOrgHelp()
    {
        ModalService.ShowModal("Default Organization", new[]
        {
            @"This option is only available for server administrators.  When selected,
                it sets this organization as the default for the server.  If the organization can't
                be determined in the quick support apps, they will use the default organization's branding."
        });
    }

    private void ShowDeviceGroupHelp()
    {
        ModalService.ShowModal("Device Groups", new[]
       {
            "Device groups can be used to restrict user permissions and to filter computers on " +
            "the main page.",
            "Everyone will have access to devices that are not in a group.  Only " +
            "administrators and users in a device group will have access to devices in that group."
        });
    }

    private void ShowInvitesHelp()
    {
        ModalService.ShowModal("Invitations", new[]
       {
            "All pending invitations will be shown here and can be revoked by deleting them.",

            "If a user does not exist, sending an invite will create their account and add them to the current organization. " +
            "A password reset URL can be generated from the user table.",

            "The Admin checkbox determines if the new user will have administrator privileges in this organization."
        });
    }

    private void ShowUsersHelp()
    {
        ModalService.ShowModal("Users", new[]
        {
            "All users for the organization are managed here",
            "Administrators will have access to this management screen as well as all computers."
        });
    }
}
