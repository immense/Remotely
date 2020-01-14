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
using Remotely.Shared.ViewModels.Organization;

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

        public bool AddDeviceGroup(string userName, DeviceGroup deviceGroup, out string deviceGroupID, out string errorMessage)
        {
            deviceGroupID = null;
            errorMessage = null;
            var organization = RemotelyContext.Users
                .Include(x => x.Organization)
                .ThenInclude(x => x.DeviceGroups)
                .FirstOrDefault(x => x.UserName == userName)
                .Organization;
            if (organization.DeviceGroups.Any(x => x.Name.ToLower() == deviceGroup.Name.ToLower()))
            {
                errorMessage = "Device group already exists.";
                return false;
            }
            var newDeviceGroup = new DeviceGroup()
            {
                Name = deviceGroup.Name,
                Organization = organization
            };
            organization.DeviceGroups.Add(newDeviceGroup);
            RemotelyContext.SaveChanges();
            deviceGroupID = newDeviceGroup.ID;
            return true;
        }

        public InviteLink AddInvite(string requesterUserName, Invite invite, string requestOrigin)
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
            device.LastOnline = DateTime.Now;

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
        public string AddSharedFile(IFormFile file, string organizationID)
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

        public void ChangeUserIsAdmin(string requesterUserName, string targetUserID, bool isAdmin)
        {
            var requester = RemotelyContext.Users
                .Include(x => x.Organization)
                .ThenInclude(x => x.RemotelyUsers)
                .FirstOrDefault(x => x.UserName == requesterUserName);

            requester.Organization.RemotelyUsers.FirstOrDefault(x => x.Id == targetUserID).IsAdministrator = isAdmin;
            RemotelyContext.SaveChanges();
        }

        public void CleanupOldRecords()
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

        public void DeleteDeviceGroup(string userName, string deviceGroupId)
        {
            var organization = RemotelyContext.Users
                .Include(x => x.Organization)
                .ThenInclude(x => x.DeviceGroups)
                .ThenInclude(x => x.Devices)
                .FirstOrDefault(x => x.UserName == userName)
                .Organization;

            var deviceGroup = organization.DeviceGroups.FirstOrDefault(x => x.ID == deviceGroupId);
            deviceGroup.Devices.ForEach(x =>
            {
                x.DeviceGroup = null;
            });
            RemotelyContext.DeviceGroups.Remove(deviceGroup);
            RemotelyContext.SaveChanges();
        }
        public void DeleteInvite(string requesterUserName, string inviteID)
        {
            var requester = RemotelyContext.Users
               .Include(x => x.Organization)
               .ThenInclude(x => x.InviteLinks)
               .FirstOrDefault(x => x.UserName == requesterUserName);
            var invite = requester.Organization.InviteLinks.FirstOrDefault(x => x.ID == inviteID);
            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == invite.InvitedUser);
            if (user != null && string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                RemotelyContext.Remove(user);
            }
            RemotelyContext.Remove(invite);
            RemotelyContext.SaveChanges();
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
            return RemotelyContext.Devices.Any(x => x.ID == deviceID && x.OrganizationID == remotelyUser.OrganizationID);
        }

        public bool DoesUserHaveAccessToDevice(string deviceID, string remotelyUserID)
        {
            var remotelyUser = RemotelyContext.Users.Find(remotelyUserID);

            return RemotelyContext.Devices.Any(x => x.ID == deviceID && x.OrganizationID == remotelyUser.OrganizationID);
        }

        public string[] FilterDeviceIDsByUserPermission(string[] deviceIDs, RemotelyUser remotelyUser)
        {
            return RemotelyContext.Devices.Where(x =>
                        deviceIDs.Contains(x.ID) &&
                        x.OrganizationID == remotelyUser.OrganizationID
                    )
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
            var user = RemotelyContext.Users.FirstOrDefault(x => x.Id == userID);

            return RemotelyContext.Devices.Where(x => x.OrganizationID == user.OrganizationID);
        }


        public ICollection<InviteLink> GetAllInviteLinks(string userName)
        {
            return RemotelyContext.Users
                   .Include(x => x.Organization)
                   .ThenInclude(x => x.InviteLinks)
                   .FirstOrDefault(x => x.UserName == userName)
                   .Organization
                   .InviteLinks;
        }

        public IEnumerable<RemotelyUser> GetAllUsers(string userName)
        {
            return RemotelyContext.Users
                    .Include(x => x.Organization)
                    .ThenInclude(x => x.RemotelyUsers)
                    .FirstOrDefault(x => x.UserName == userName)
                    .Organization
                    .RemotelyUsers;
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
            var user = RemotelyContext.Users.FirstOrDefault(x => x.Id == userID);

            return RemotelyContext.Devices.FirstOrDefault(x =>
                            x.OrganizationID == user.OrganizationID &&
                            x.ID == deviceID);
        }

        public List<DeviceGroup> GetDeviceGroupsForUserName(string username)
        {
            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == username);

            return RemotelyContext.DeviceGroups.Where(x => x.OrganizationID == user.OrganizationID).ToList();
        }

        public int GetOrganizationCount()
        {
            return RemotelyContext.Organizations.Count();
        }

        public string GetOrganizationName(string userName)
        {
            return RemotelyContext.Users
                   .Include(x => x.Organization)
                   .FirstOrDefault(x => x.UserName == userName)
                   .Organization
                   .OrganizationName;
        }

        public SharedFile GetSharedFiled(string fileID)
        {
            return RemotelyContext.SharedFiles.Find(fileID);
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

        public RemotelyUserOptions GetUserOptions(string userName)
        {
            return RemotelyContext.Users
                    .FirstOrDefault(x => x.UserName == userName)
                    .UserOptions;
        }

        public bool JoinViaInvitation(string userName, string inviteID)
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
                .FirstOrDefault(x => x.UserName == userName);

            user.Organization = invite.Organization;
            user.OrganizationID = invite.Organization.ID;
            user.IsAdministrator = invite.IsAdmin;
            invite.Organization.RemotelyUsers.Add(user);

            RemotelyContext.SaveChanges();

            RemotelyContext.InviteLinks.Remove(invite);
            RemotelyContext.SaveChanges();
            return true;
        }

        public void RemoveDevices(string[] deviceIDs)
        {
            var devices = RemotelyContext.Devices
                .Where(x => deviceIDs.Contains(x.ID));

            RemotelyContext.Devices.RemoveRange(devices);
            RemotelyContext.SaveChanges();
        }

        public void RemoveUserFromOrganization(string requesterUserName, string targetUserID)
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

        public void SetAllDevicesNotOnline()
        {
            RemotelyContext.Devices.ForEachAsync(x =>
            {
                x.IsOnline = false;
            }).Wait();
            RemotelyContext.SaveChanges();
        }

        public void SetServerVerificationToken(string deviceID, string verificationToken)
        {
            var device = RemotelyContext.Devices.Find(deviceID);
            if (device != null)
            {
                device.ServerVerificationToken = verificationToken;
                RemotelyContext.SaveChanges();
            }
        }

        public void UpdateDevice(string deviceID, string tag, string alias, string deviceGroupID)
        {
            var device = RemotelyContext.Devices.Find(deviceID);
            if (device == null)
            {
                return;
            }

            device.Tags = tag;
            device.DeviceGroupID = deviceGroupID;
            device.Alias = alias;
            RemotelyContext.SaveChanges();
        }

        public void UpdateTags(string deviceID, string tags)
        {
            var device = RemotelyContext.Devices.Find(deviceID);
            if (device == null)
            {
                return;
            }

            device.Tags = tags;
            RemotelyContext.SaveChanges();
        }

        public void UpdateOrganizationName(string userName, string organizationName)
        {
            RemotelyContext.Users
                .Include(x => x.Organization)
                .FirstOrDefault(x => x.UserName == userName)
                .Organization
                .OrganizationName = organizationName;
            RemotelyContext.SaveChanges();
        }

        public void UpdateUserOptions(string userName, RemotelyUserOptions options)
        {
            RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName).UserOptions = options;
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
    }
}
