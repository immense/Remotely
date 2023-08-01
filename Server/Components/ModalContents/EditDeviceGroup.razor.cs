using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Remotely.Server.Services;
using Remotely.Shared.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Components.ModalContents;

[Authorize]
public partial class EditDeviceGroup : AuthComponentBase
{
    public static string DeviceGroupsPropName => nameof(DeviceGroups);
    public static string EditUserPropName => nameof(EditUser);
    [Parameter]
    public required DeviceGroup[] DeviceGroups { get; set; }

    [Parameter]
    public required RemotelyUser EditUser { get; set; }

    [Inject]
    private IDataService DataService { get; init; } = null!;

    [Inject]
    private IToastService ToastService { get; init; } = null!;


    private bool DoesGroupContainUser(DeviceGroup group)
    {
        return group.Users.Any(x => x.Id == EditUser.Id);
    }

    private async Task GroupCheckChanged(ChangeEventArgs args, DeviceGroup group)
    {
        if (!string.IsNullOrWhiteSpace(EditUser.UserName) &&
            args.Value is bool boolValue &&
            boolValue)
        {
            if (!DataService.AddUserToDeviceGroup(EditUser.OrganizationID, group.ID, EditUser.UserName, out var result))
            {
                ToastService.ShowToast(result, classString: "bg-warning");
            }
            else
            {
                ToastService.ShowToast("User added to group.");
            }

        }
        else
        {
            var result = await DataService.RemoveUserFromDeviceGroup(EditUser.OrganizationID, group.ID, EditUser.Id);
            if (!result)
            {
                ToastService.ShowToast("Failed to remove from group.", classString: "bg-warning");
            }
            else
            {
                ToastService.ShowToast("Removed user from group.");
            }
        }
    }
}
