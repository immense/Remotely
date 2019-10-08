using Remotely.Shared.Models;
using Remotely.Shared.ViewModels;
using Remotely.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Remotely.Server.Data
{
    public class DataService
    {
        public DataService(ApplicationDbContext context, ApplicationConfig appConfig)
        {
            RemotelyContext = context;
            AppConfig = appConfig;
        }

        private ApplicationConfig AppConfig { get; set; }

        private ApplicationDbContext RemotelyContext { get; set; }

        public void AddOrUpdateCommandContext(CommandContext commandContext)
        {
            var existingContext = RemotelyContext.CommandContexts.Find(commandContext.ID);
            if (existingContext != null)
            {
                var entry = RemotelyContext.Entry(existingContext);
                entry.CurrentValues.SetValues(commandContext);
                entry.State = EntityState.Modified;
            }
            else
            {
                RemotelyContext.CommandContexts.Add(commandContext);
            }
            RemotelyContext.SaveChanges();
        }

        public bool AddOrUpdateDevice(Device device, out Device updatedDevice)
        {
            var existingDevice = RemotelyContext.Devices.Find(device.ID);
            if (existingDevice != null)
            {
                existingDevice.CurrentUser = device.CurrentUser;
                existingDevice.DeviceName = device.DeviceName;
                existingDevice.Drives = device.Drives;
                existingDevice.FreeMemory = device.FreeMemory;
                existingDevice.FreeStorage = device.FreeStorage;
                existingDevice.Is64Bit = device.Is64Bit;
                existingDevice.IsOnline = true;
                existingDevice.LastOnline = DateTime.Now;
                existingDevice.OSArchitecture = device.OSArchitecture;
                existingDevice.OSDescription = device.OSDescription;
                existingDevice.Platform = device.Platform;
                existingDevice.ProcessorCount = device.ProcessorCount;
                existingDevice.TotalMemory = device.TotalMemory;
                existingDevice.TotalStorage = device.TotalStorage;
                existingDevice.AgentVersion = device.AgentVersion;
                updatedDevice = existingDevice;
            }
            else
            {
                updatedDevice = device;
                if (!RemotelyContext.Organizations.Any(x => x.ID == device.OrganizationID))
                {
                    WriteEvent(new EventLog()
                    {
                        EventType = EventTypes.Info,
                        Message = $"Unable to add device {device.DeviceName} because organization {device.OrganizationID} does not exist.",
                        Source = "DataService.AddOrUpdateDevice"
                    });
                    return false;
                }
                RemotelyContext.Devices.Add(device);
            }
            RemotelyContext.SaveChanges();
            return true;
        }

        public void DeviceDisconnected(string deviceID)
        {
            var device = RemotelyContext.Devices.Find(deviceID);
            if (device != null)
            {
                device.LastOnline = DateTime.Now;
                device.IsOnline = false;
                RemotelyContext.SaveChanges();
            }
        }

        public bool DoesUserExist(string userName)
        {
            if (userName == null)
            {
                return false;
            }
            return RemotelyContext.Users.Any(x => x.UserName == userName);
        }

        public bool DoesUserHaveAccessToDevice(string deviceID, RemotelyUser remotelyUser)
        {
            var targetDevice = RemotelyContext.Devices
                                .Include(x => x.DevicePermissionLinks)
                                .ThenInclude(x => x.PermissionGroup)
                                .ThenInclude(x => x.UserPermissionLinks)
                                .FirstOrDefault(x => x.ID == deviceID && x.OrganizationID == remotelyUser.OrganizationID);

            return remotelyUser.IsAdministrator ||
                    targetDevice.DevicePermissionLinks.Count == 0 ||
                    targetDevice.DevicePermissionLinks.Any(x => x.PermissionGroup.UserPermissionLinks.Any(y => y.RemotelyUserID == remotelyUser.Id));
        }

        public bool DoesUserHaveAccessToDevice(string deviceID, string remotelyUserID)
        {
            var remotelyUser = RemotelyContext.Users.Find(remotelyUserID);

            var targetDevice = RemotelyContext.Devices
                                .Include(x => x.DevicePermissionLinks)
                                .ThenInclude(x => x.PermissionGroup)
                                .ThenInclude(x => x.UserPermissionLinks)
                                .FirstOrDefault(x => x.ID == deviceID && x.OrganizationID == remotelyUser.OrganizationID);

            return remotelyUser.IsAdministrator ||
                    targetDevice.DevicePermissionLinks.Count == 0 ||
                    targetDevice.DevicePermissionLinks.Any(x => x.PermissionGroup.UserPermissionLinks.Any(y => y.RemotelyUserID == remotelyUser.Id));
        }

        public string[] FilterDeviceIDsByUserPermission(string[] deviceIDs, RemotelyUser remotelyUser)
        {
            return RemotelyContext.Devices.Where(x =>
                    deviceIDs.Contains(x.ID) &&
                    x.OrganizationID == remotelyUser.OrganizationID &&
                    (
                        remotelyUser.IsAdministrator ||
                        x.DevicePermissionLinks.Count == 0 ||
                        x.DevicePermissionLinks.Any(y => y.PermissionGroup.UserPermissionLinks.Any(z => z.RemotelyUserID == remotelyUser.Id))
                    ))
                    .Select(x => x.ID)
                    .ToArray();
        }

        public IEnumerable<CommandContext> GetAllCommandContexts(string userName)
        {
            var orgID = GetUserByName(userName).OrganizationID;
            return RemotelyContext.CommandContexts.Where(x => x.OrganizationID == orgID);
        }

        public IEnumerable<Device> GetAllDevicesForUser(string userID)
        {
            var user = RemotelyContext.Users
                .Include(x => x.UserPermissionLinks)
                .FirstOrDefault(x => x.Id == userID);

            var result = RemotelyContext.Devices
                .Include(x => x.DevicePermissionLinks)
                .Where(x => x.OrganizationID == user.OrganizationID);

            if (user.IsAdministrator)
            {
                return result;
            }
            else
            {
                return result.Where(x =>
                            x.DevicePermissionLinks.Count == 0 ||
                            x.DevicePermissionLinks.Any(y => y.PermissionGroup.UserPermissionLinks.Any(z => z.RemotelyUserID == user.Id)));
            }
        }

        public IEnumerable<PermissionGroup> GetAllPermissions(string userName)
        {
            return RemotelyContext.Users
                    .Include(x => x.Organization)
                    .ThenInclude(x => x.PermissionGroups)
                    .FirstOrDefault(x => x.UserName == userName)
                    .Organization.PermissionGroups;
        }

        public CommandContext GetCommandContext(string commandContextID, string userName)
        {
            var user = GetUserByName(userName);
            return RemotelyContext.CommandContexts
                .FirstOrDefault(x =>
                    (user.IsAdministrator || x.SenderUserID == user.Id) &&
                    x.OrganizationID == user.OrganizationID &&
                    x.ID == commandContextID);
        }

        public CommandContext GetCommandContext(string commandContextID)
        {
            return RemotelyContext.CommandContexts.Find(commandContextID);
        }

        public string GetDefaultPrompt(string userName)
        {
            var userPrompt = RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName)?.UserOptions?.ConsolePrompt;
            return userPrompt ?? AppConfig.DefaultPrompt;
        }

        public string GetDefaultPrompt()
        {
            return AppConfig.DefaultPrompt;
        }

        public Device GetDeviceForUser(string userID, string deviceID)
        {
            var user = RemotelyContext.Users
                .Include(x => x.UserPermissionLinks)
                .FirstOrDefault(x => x.Id == userID);

            var result = RemotelyContext.Devices
                        .Include(x => x.DevicePermissionLinks)
                        .Where(x =>
                            x.OrganizationID == user.OrganizationID &&
                            x.ID == deviceID);

            if (!user.IsAdministrator)
            {
                result = result.Where(x =>
                        x.DevicePermissionLinks.Count == 0 ||
                        x.DevicePermissionLinks.Any(y => y.PermissionGroup.UserPermissionLinks.Any(z => z.RemotelyUserID == user.Id)));
            }

            return result.FirstOrDefault();
        }

        public IEnumerable<Device> GetDevicesAndPermissions(string userID, string[] deviceIDs)
        {
            var user = GetUserByID(userID);

            var devices = RemotelyContext.Devices
                            .Include(x => x.DevicePermissionLinks)
                            .Include("DevicePermissionLinks.PermissionGroup")
                            .Where(x => x.OrganizationID == user.OrganizationID && deviceIDs.Contains(x.ID))
                            .ToList();

            return devices.Select(x => new Device()
                    {
                        DeviceName = x.DeviceName,
                        DevicePermissionLinks = x.DevicePermissionLinks.ToList()
                    });
        }
        public RemotelyUser GetUserAndPermissionsByID(string userID)
        {
            if (userID == null)
            {
                return null;
            }
            return RemotelyContext.Users
                .Include(x => x.Organization)
                .Include(x => x.UserPermissionLinks)
                .FirstOrDefault(x => x.Id == userID);
        }

        public RemotelyUser GetUserByID(string userID)
        {
            if (userID == null)
            {
                return null;
            }
            return RemotelyContext.Users.FirstOrDefault(x => x.Id == userID);
        }

        public RemotelyUser GetUserByName(string userName)
        {
            if (userName == null)
            {
                return null;
            }
            return RemotelyContext.Users
                .Include(x => x.Organization)
                .FirstOrDefault(x => x.UserName == userName);
        }

        public List<RemotelyUser> GetUsersWithAccessToDevice(IEnumerable<string> userIDs, Device device)
        {
            var targetDevice = RemotelyContext.Devices
                                .Include(x => x.DevicePermissionLinks)
                                .ThenInclude(x => x.PermissionGroup)
                                .ThenInclude(x => x.UserPermissionLinks)
                                .FirstOrDefault(x => x.ID == device.ID && x.OrganizationID == device.OrganizationID);

            // Users within the target device's organization, where user ID is in the list of IDs passed in, and
            // user is either an admin, the target device has no permissions assigned, or the target device is in a
            // permission group that the user is also in.
            var targetUsers = RemotelyContext.Users.Where(x => 
                                    x.OrganizationID == device.OrganizationID && 
                                    userIDs.Contains(x.Id)).ToList();

            var authorizedUsers = targetUsers.Where(x =>
                                    x.IsAdministrator ||
                                    targetDevice.DevicePermissionLinks.Count == 0 ||
                                    targetDevice.DevicePermissionLinks.Any(y => y.PermissionGroup.UserPermissionLinks.Any(z => z.RemotelyUserID == x.Id)));

            return authorizedUsers.ToList();
        }
        public void RemoveDevices(string[] deviceIDs)
        {
            var devices = RemotelyContext.Devices
                .Where(x => deviceIDs.Contains(x.ID));

            RemotelyContext.Devices.RemoveRange(devices);
            RemotelyContext.SaveChanges();
        }

        public void SetAllDevicesNotOnline()
        {
            RemotelyContext.Devices.ForEachAsync(x =>
            {
                x.IsOnline = false;
            }).Wait();
            RemotelyContext.SaveChanges();
        }

        public void WriteEvent(EventLog eventLog)
        {
            RemotelyContext.EventLogs.Add(eventLog);
            RemotelyContext.SaveChanges();
        }

        public void WriteEvent(Exception ex)
        {
            RemotelyContext.EventLogs.Add(new EventLog()
            {
                EventType = EventTypes.Error,
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace,
                TimeStamp = DateTime.Now
            });
            RemotelyContext.SaveChanges();
        }

        public void WriteEvent(string message)
        {
            RemotelyContext.EventLogs.Add(new EventLog()
            {
                EventType = EventTypes.Info,
                Message = message,
                TimeStamp = DateTime.Now
            });
            RemotelyContext.SaveChanges();
        }

        internal InviteLink AddInvite(string requesterUserName, Invite invite, string requestOrigin)
        {
            invite.InvitedUser = invite.InvitedUser.ToLower();

            var requester = RemotelyContext.Users
                .Include(x => x.Organization)
                .ThenInclude(x => x.InviteLinks)
                .Include(x => x.Organization)
                .ThenInclude(x => x.RemotelyUsers)
                .FirstOrDefault(x => x.UserName == requesterUserName);

            var newInvite = new InviteLink()
            {
                DateSent = DateTime.Now,
                InvitedUser = invite.InvitedUser,
                IsAdmin = invite.IsAdmin,
                Organization = requester.Organization,
                ResetUrl = invite.ResetUrl
            };
            requester.Organization.InviteLinks.Add(newInvite);
            RemotelyContext.SaveChanges();
            return newInvite;
        }

        internal bool AddPermission(string userName, Permission permission, out string permissionID, out string errorMessage)
        {
            permissionID = null;
            errorMessage = null;
            var organization = RemotelyContext.Users
                .Include(x => x.Organization)
                .ThenInclude(x => x.PermissionGroups)
                .FirstOrDefault(x => x.UserName == userName)
                .Organization;
            if (organization.PermissionGroups.Any(x => x.Name.ToLower() == permission.Name.ToLower()))
            {
                errorMessage = "Permission group already exists.";
                return false;
            }
            var newPermission = new PermissionGroup()
            {
                Name = permission.Name,
                Organization = organization
            };
            organization.PermissionGroups.Add(newPermission);
            RemotelyContext.SaveChanges();
            permissionID = newPermission.ID;
            return true;
        }

        internal void AddPermissionToDevices(string userID, string[] deviceIDs, string groupName)
        {
            var user = RemotelyContext.Users
                       .Include(x => x.Organization)
                       .Include(x => x.Organization)
                       .ThenInclude(x => x.Devices)
                       .FirstOrDefault(x => x.Id == userID);

            var group = user.Organization.PermissionGroups.FirstOrDefault(x => x.Name.ToLower() == groupName.ToLower());
            foreach (var deviceID in deviceIDs)
            {
                if (user.Organization.Devices.Any(x => x.ID == deviceID))
                {
                    var device = RemotelyContext.Devices
                                .Include(x => x.DevicePermissionLinks)
                                .FirstOrDefault(x => x.ID == deviceID);
                    if (!device.DevicePermissionLinks.Any(x => x.PermissionGroupID == group.ID))
                    {
                        device.DevicePermissionLinks.Add(new DevicePermissionLink()
                        {
                            Device = device,
                            DeviceID = device.ID,
                            PermissionGroup = group,
                            PermissionGroupID = group.ID
                        });
                        RemotelyContext.Entry(device).State = EntityState.Modified;
                    }
                }
            }
            RemotelyContext.SaveChanges();
        }

        internal bool AddPermissionToUser(string requesterUserName, string targetUserID, string permissionID, out string errorMessage)
        {
            errorMessage = null;
            var requester = RemotelyContext.Users
                .Include(x => x.Organization)
                .FirstOrDefault(x => x.UserName == requesterUserName);

            var user = RemotelyContext.Users
						.Include(x => x.UserPermissionLinks)
						.FirstOrDefault(x => x.OrganizationID == requester.OrganizationID && x.Id == targetUserID);

            if (user.UserPermissionLinks.Any(x => x.PermissionGroupID == permissionID))
            {
                errorMessage = "User is already in the permission group.";
                return false;
            }

            var permissions = RemotelyContext.PermissionGroups
                .Include(x => x.Organization)
                .Where(x => x.Organization.ID == requester.Organization.ID);
            var permission = permissions.FirstOrDefault(x => x.ID == permissionID);
            user.UserPermissionLinks.Add(new UserPermissionLink()
            {
                PermissionGroup = permission,
                PermissionGroupID = permission.ID,
                RemotelyUser = user,
                RemotelyUserID = user.Id
            });
            RemotelyContext.SaveChanges();
            return true;
        }

        internal string AddSharedFile(IFormFile file, string organizationID)
        {
            var expirationDate = DateTime.Now.AddDays(-AppConfig.DataRetentionInDays);
            var expiredFiles = RemotelyContext.SharedFiles.Where(x => x.Timestamp < expirationDate);
            RemotelyContext.RemoveRange(expiredFiles);

            byte[] fileContents;
            using (var stream = file.OpenReadStream())
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    fileContents = ms.ToArray();
                }
            }
            var newEntity = RemotelyContext.Add(new SharedFile()
            {
                FileContents = fileContents,
                FileName = file.FileName,
                ContentType = file.ContentType,
                OrganizationID = organizationID
            });
            RemotelyContext.SaveChanges();
            return newEntity.Entity.ID;
        }

        internal void ChangeUserIsAdmin(string requesterUserName, string targetUserID, bool isAdmin)
        {
            var requester = RemotelyContext.Users
                .Include(x => x.Organization)
                .ThenInclude(x => x.RemotelyUsers)
                .FirstOrDefault(x => x.UserName == requesterUserName);

            requester.Organization.RemotelyUsers.FirstOrDefault(x => x.Id == targetUserID).IsAdministrator = isAdmin;
            RemotelyContext.SaveChanges();
        }

        internal void CleanupOldRecords()
        {
            if (AppConfig.DataRetentionInDays > 0)
            {
                var expirationDate = DateTime.Now - TimeSpan.FromDays(AppConfig.DataRetentionInDays);

                var eventLogs = RemotelyContext.EventLogs
                                    .Where(x => x.TimeStamp < expirationDate);

                RemotelyContext.RemoveRange(eventLogs);

                var commandContexts = RemotelyContext.CommandContexts
                                        .Where(x => x.TimeStamp < expirationDate);

                RemotelyContext.RemoveRange(commandContexts);

                var sharedFiles = RemotelyContext.SharedFiles
                                        .Where(x => x.Timestamp < expirationDate);

                RemotelyContext.RemoveRange(sharedFiles);

                RemotelyContext.SaveChanges();
            }
        }

        internal void DeleteInvite(string requesterUserName, string inviteID)
        {
            var requester = RemotelyContext.Users
               .Include(x => x.Organization)
               .ThenInclude(x => x.InviteLinks)
               .FirstOrDefault(x => x.UserName == requesterUserName);
            var invite = requester.Organization.InviteLinks.FirstOrDefault(x => x.ID == inviteID);
            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == invite.InvitedUser);
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                RemotelyContext.Remove(user);
            }
            RemotelyContext.Remove(invite);
            RemotelyContext.SaveChanges();
        }

        internal void DeletePermission(string userName, string permissionID)
        {
            var organization = RemotelyContext.Users
                .Include(x => x.Organization)
                .ThenInclude(x => x.PermissionGroups)
                .FirstOrDefault(x => x.UserName == userName)
                .Organization;

            var permissionGroup = organization.PermissionGroups.FirstOrDefault(x => x.ID == permissionID);
            RemotelyContext.PermissionGroups.Remove(permissionGroup);
            RemotelyContext.SaveChanges();
        }

        internal bool DoesGroupExist(string userID, string groupName)
        {
            var user = RemotelyContext.Users
                        .Include(x => x.Organization)
                        .ThenInclude(x => x.PermissionGroups)
                        .FirstOrDefault(x => x.Id == userID);
            return user.Organization.PermissionGroups.Any(x => x.Name.ToLower() == groupName.ToLower());
        }

        internal ICollection<InviteLink> GetAllInviteLinks(string userName)
        {
            return RemotelyContext.Users
                   .Include(x => x.Organization)
                   .ThenInclude(x => x.InviteLinks)
                   .FirstOrDefault(x => x.UserName == userName)
                   .Organization
                   .InviteLinks;
        }

        internal IEnumerable<RemotelyUser> GetAllUsers(string userName)
        {
            return RemotelyContext.Users
                    .Include(x => x.Organization)
                    .ThenInclude(x => x.RemotelyUsers)
                    .FirstOrDefault(x => x.UserName == userName)
                    .Organization
                    .RemotelyUsers;
        }

        internal string GetOrganizationName(string userName)
        {
            return RemotelyContext.Users
                   .Include(x => x.Organization)
                   .FirstOrDefault(x => x.UserName == userName)
                   .Organization
                   .OrganizationName;
        }

        internal SharedFile GetSharedFiled(string fileID)
        {
            return RemotelyContext.SharedFiles.Find(fileID);
        }

        internal RemotelyUserOptions GetUserOptions(string userName)
        {
            return RemotelyContext.Users
                    .FirstOrDefault(x => x.UserName == userName)
                    .UserOptions;
        }

        internal IEnumerable<UserPermissionLink> GetUserPermissions(string requesterUserName, string targetID)
        {
            var requester = GetUserByName(requesterUserName);
            var targetUser = RemotelyContext.Users
                    .Include(x => x.Organization)
                    .Include("UserPermissionLinks.PermissionGroup")
                    .FirstOrDefault(x => x.OrganizationID == requester.OrganizationID &&
                        x.Id == targetID);

            return targetUser.UserPermissionLinks;
        }

        internal bool JoinViaInvitation(string userName, string inviteID)
        {
            var invite = RemotelyContext.InviteLinks
                .Include(x => x.Organization)
                .ThenInclude(x => x.RemotelyUsers)
                .FirstOrDefault(x =>
                    x.InvitedUser.ToLower() == userName.ToLower() &&
                    x.ID == inviteID);

            if (invite == null)
            {
                return false;
            }

            var user = RemotelyContext.Users
                .Include(x => x.Organization)
				.Include(x => x.UserPermissionLinks)
                .FirstOrDefault(x => x.UserName == userName);

            user.Organization = invite.Organization;
            user.OrganizationID = invite.Organization.ID;
            user.IsAdministrator = invite.IsAdmin;
            user.UserPermissionLinks.Clear();
            invite.Organization.RemotelyUsers.Add(user);

            RemotelyContext.SaveChanges();

            RemotelyContext.InviteLinks.Remove(invite);
            RemotelyContext.SaveChanges();
            return true;
        }
        internal void RemovePermissionFromDevices(string userID, string[] deviceIDs, string groupName)
        {
            var user = RemotelyContext.Users
                      .Include(x => x.Organization)
                      .ThenInclude(x => x.Devices)
                      .FirstOrDefault(x => x.Id == userID);

            var group = user.Organization.PermissionGroups.FirstOrDefault(x => x.Name.ToLower() == groupName.ToLower());
            foreach (var deviceID in deviceIDs)
            {
                if (user.Organization.Devices.Any(x => x.ID == deviceID))
                {
                    var device = RemotelyContext.Devices
                                .Include(x => x.DevicePermissionLinks)
                                .FirstOrDefault(x => x.ID == deviceID);
                    foreach (var permission in device.DevicePermissionLinks.ToList().Where(x => x.PermissionGroupID == group.ID))
                    {
                        device.DevicePermissionLinks.Remove(permission);
                    }
                    RemotelyContext.Entry(device).State = EntityState.Modified;
                }
            }
            RemotelyContext.SaveChanges();
        }

        internal void RemovePermissionFromUser(string requesterUserName, string targetUserID, string permissionID)
        {
            var requester = RemotelyContext.Users
              .Include(x => x.Organization)
              .ThenInclude(x => x.RemotelyUsers)
              .ThenInclude(x => x.UserPermissionLinks)
              .FirstOrDefault(x => x.UserName == requesterUserName);

            var target = requester.Organization.RemotelyUsers.FirstOrDefault(x => x.Id == targetUserID);
            foreach (var permission in target.UserPermissionLinks.ToList().Where(x => x.PermissionGroupID == permissionID))
            {
                target.UserPermissionLinks.Remove(permission);
            }
            RemotelyContext.Entry(target).State = EntityState.Modified;
            RemotelyContext.SaveChanges();
        }

        internal void RemoveUserFromOrganization(string requesterUserName, string targetUserID)
        {
            var requester = RemotelyContext.Users
                .Include(x => x.Organization)
                .ThenInclude(x => x.RemotelyUsers)
                .FirstOrDefault(x => x.UserName == requesterUserName);
            var target = requester.Organization.RemotelyUsers.FirstOrDefault(x => x.Id == targetUserID);

            var newOrganization = new Organization();
            target.Organization = newOrganization;
            RemotelyContext.Organizations.Add(newOrganization);
            RemotelyContext.SaveChanges();
        }
        internal void SetServerVerificationToken(string deviceID, string verificationToken)
        {
            var device = RemotelyContext.Devices.Find(deviceID);
            if (device != null)
            {
                device.ServerVerificationToken = verificationToken;
                RemotelyContext.SaveChanges();
            }
        }

        internal void UpdateOrganizationName(string userName, string organizationName)
        {
            RemotelyContext.Users
                .Include(x => x.Organization)
                .FirstOrDefault(x => x.UserName == userName)
                .Organization
                .OrganizationName = organizationName;
            RemotelyContext.SaveChanges();
        }

        internal void UpdateTags(string deviceID, string tag)
        {
            RemotelyContext.Devices.Find(deviceID).Tags = tag;
            RemotelyContext.SaveChanges();
        }

        internal void UpdateUserOptions(string userName, RemotelyUserOptions options)
        {
            RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName).UserOptions = options;
            RemotelyContext.SaveChanges();
        }
    }
}
