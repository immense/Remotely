using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remotely.Server.Data;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.ViewModels.Organization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public interface IDataService
    {
        Task AddAlert(AlertOptions alertOptions, string organizationID);
        bool AddDeviceGroup(string orgID, DeviceGroup deviceGroup, out string deviceGroupID, out string errorMessage);
        InviteLink AddInvite(string orgID, Invite invite);
        void AddOrUpdateCommandResult(CommandResult commandResult);
        bool AddOrUpdateDevice(Device device, out Device updatedDevice);
        Task<string> AddSharedFile(IFormFile file, string organizationID);
        bool AddUserToDeviceGroup(string orgID, string groupID, string userName, out string resultMessage);
        void ChangeUserIsAdmin(string organizationID, string targetUserID, bool isAdmin);
        void CleanupOldRecords();
        Task<ApiToken> CreateApiToken(string userName, string tokenName, string secretHash);
        Task<Device> CreateDevice(DeviceSetupOptions options);
        Task<bool> CreateUser(string userEmail, bool isAdmin, string organizationID);
        Task DeleteAlert(Alert alert);
        Task DeleteApiToken(string userName, string tokenId);
        void DeleteDeviceGroup(string orgID, string deviceGroupID);
        void DeleteInvite(string orgID, string inviteID);
        void DetachEntity(object entity);
        void DeviceDisconnected(string deviceID);
        bool DoesUserExist(string userName);
        bool DoesUserHaveAccessToDevice(string deviceID, RemotelyUser remotelyUser);
        bool DoesUserHaveAccessToDevice(string deviceID, string remotelyUserID);
        string[] FilterDeviceIDsByUserPermission(string[] deviceIDs, RemotelyUser remotelyUser);
        string[] FilterUsersByDevicePermission(IEnumerable<string> userIDs, string deviceID);
        Task<Alert> GetAlert(string alertID);
        IEnumerable<Alert> GetAlerts(string userID);
        IEnumerable<ApiToken> GetAllApiTokens(string userID);
        IEnumerable<CommandResult> GetAllCommandResults(string orgID);
        IEnumerable<Device> GetAllDevices(string orgID);
        IEnumerable<EventLog> GetAllEventLogs(string orgID);
        ICollection<InviteLink> GetAllInviteLinks(string userName);
        IEnumerable<RemotelyUser> GetAllUsers(string userName);
        ApiToken GetApiToken(string apiToken);
        CommandResult GetCommandResult(string commandResultID);
        CommandResult GetCommandResult(string commandResultID, string orgID);
        string GetDefaultPrompt();
        string GetDefaultPrompt(string userName);
        Device GetDevice(string deviceID);
        Device GetDevice(string orgID, string deviceID);
        int GetDeviceCount();
        IEnumerable<DeviceGroup> GetDeviceGroups(string username);
        IEnumerable<Device> GetDevicesForUser(string userName);
        IEnumerable<EventLog> GetEventLogs(string userName, DateTimeOffset from, DateTimeOffset to, EventType? type, string message);
        int GetOrganizationCount();
        string GetOrganizationName(string userName);
        string GetOrganizationNameById(string organizationID);
        List<string> GetServerAdmins();
        SharedFile GetSharedFiled(string fileID);
        int GetTotalDevices();
        RemotelyUser GetUserByID(string userID);
        RemotelyUser GetUserByName(string userName);
        RemotelyUserOptions GetUserOptions(string userName);
        bool JoinViaInvitation(string userName, string inviteID);
        void RemoveDevices(string[] deviceIDs);
        Task<bool> RemoveUserFromDeviceGroup(string orgID, string groupID, string userID);
        Task RemoveUserFromOrganization(string orgID, string targetUserID);
        Task RenameApiToken(string userName, string tokenId, string tokenName);
        void SetAllDevicesNotOnline();
        Task SetDisplayName(RemotelyUser user, string displayName);
        void SetServerVerificationToken(string deviceID, string verificationToken);
        Task<bool> TempPasswordSignIn(string email, string password);
        Task<Device> UpdateDevice(DeviceSetupOptions deviceOptions, string organizationId);
        void UpdateDevice(string deviceID, string tag, string alias, string deviceGroupID, string notes);
        void UpdateOrganizationName(string orgID, string organizationName);
        Task UpdateServerAdmins(List<string> serverAdmins, string callerUserName);
        void UpdateTags(string deviceID, string tags);
        void UpdateUserOptions(string userName, RemotelyUserOptions options);
        bool ValidateApiToken(string apiToken, string apiSecret, string requestPath, string remoteIP);
        void WriteEvent(EventLog eventLog);
        void WriteEvent(Exception ex, string organizationID);
        void WriteEvent(string message, EventType eventType, string organizationID);
        void WriteEvent(string message, string organizationID);
        void WriteLog(LogLevel logLevel, string category, EventId eventId, string state, Exception exception, List<string> scopeStack);
    }

    public class DataService : IDataService
    {
        public DataService(ApplicationDbContext context,
            IApplicationConfig appConfig,
            IHostEnvironment hostEnvironment,
            UserManager<RemotelyUser> userManager)
        {
            RemotelyContext = context;
            AppConfig = appConfig;
            HostEnvironment = hostEnvironment;
            UserManager = userManager;
        }

        private IApplicationConfig AppConfig { get; }
        private IHostEnvironment HostEnvironment { get; }
        private ApplicationDbContext RemotelyContext { get; }
        private UserManager<RemotelyUser> UserManager { get; }

        public async Task AddAlert(AlertOptions alertOptions, string organizationID)
        {
            var users = RemotelyContext.Users
                .Include(x => x.Alerts)
                .Where(x => x.OrganizationID == organizationID);

            if (!string.IsNullOrWhiteSpace(alertOptions.AlertDeviceID))
            {
                var filteredUserIDs = FilterUsersByDevicePermission(users.Select(x => x.Id), alertOptions.AlertDeviceID);
                users = users.Where(x => filteredUserIDs.Contains(x.Id));
            }

            await users.ForEachAsync(x =>
            {
                var alert = new Alert()
                {
                    CreatedOn = DateTimeOffset.Now,
                    DeviceID = alertOptions.AlertDeviceID,
                    Message = alertOptions.AlertMessage,
                    OrganizationID = organizationID
                };
                x.Alerts.Add(alert);
            });

            await RemotelyContext.SaveChangesAsync();
        }

        public bool AddDeviceGroup(string orgID, DeviceGroup deviceGroup, out string deviceGroupID, out string errorMessage)
        {
            deviceGroupID = null;
            errorMessage = null;

            var organization = RemotelyContext.Organizations
                .Include(x => x.DeviceGroups)
                .FirstOrDefault(x => x.ID == orgID);

            if (RemotelyContext.DeviceGroups.Any(x =>
                x.OrganizationID == orgID &&
                x.Name.ToLower() == deviceGroup.Name.ToLower()))
            {
                errorMessage = "Device group already exists.";
                return false;
            }

            var newDeviceGroup = new DeviceGroup()
            {
                Name = deviceGroup.Name,
                Organization = organization,
                OrganizationID = orgID
            };

            organization.DeviceGroups.Add(newDeviceGroup);
            RemotelyContext.SaveChanges();
            deviceGroupID = newDeviceGroup.ID;
            return true;
        }

        public InviteLink AddInvite(string orgID, Invite invite)
        {
            invite.InvitedUser = invite.InvitedUser.ToLower();

            var organization = RemotelyContext.Organizations
                .Include(x => x.InviteLinks)
                .FirstOrDefault(x => x.ID == orgID);

            var newInvite = new InviteLink()
            {
                DateSent = DateTimeOffset.Now,
                InvitedUser = invite.InvitedUser,
                IsAdmin = invite.IsAdmin,
                Organization = organization,
                OrganizationID = organization.ID
            };
            organization.InviteLinks.Add(newInvite);
            RemotelyContext.SaveChanges();
            return newInvite;
        }

        public async Task<bool> TempPasswordSignIn(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var user = GetUserByName(email);

            if (user is null)
            {
                return false;
            }

            if (user.TempPassword == password)
            {
                user.TempPassword = string.Empty;
                await RemotelyContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public void AddOrUpdateCommandResult(CommandResult commandResult)
        {
            var existingContext = RemotelyContext.CommandResults.Find(commandResult.ID);
            if (existingContext != null)
            {
                var entry = RemotelyContext.Entry(existingContext);
                entry.CurrentValues.SetValues(commandResult);
                entry.State = EntityState.Modified;
            }
            else
            {
                RemotelyContext.CommandResults.Add(commandResult);
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
                existingDevice.CpuUtilization = device.CpuUtilization;
                existingDevice.UsedMemory = device.UsedMemory;
                existingDevice.UsedStorage = device.UsedStorage;
                existingDevice.Is64Bit = device.Is64Bit;
                existingDevice.IsOnline = true;
                existingDevice.OSArchitecture = device.OSArchitecture;
                existingDevice.OSDescription = device.OSDescription;
                existingDevice.Platform = device.Platform;
                existingDevice.ProcessorCount = device.ProcessorCount;
                existingDevice.PublicIP = device.PublicIP;
                existingDevice.TotalMemory = device.TotalMemory;
                existingDevice.TotalStorage = device.TotalStorage;
                existingDevice.AgentVersion = device.AgentVersion;
                existingDevice.LastOnline = DateTimeOffset.Now;
                updatedDevice = existingDevice;
            }
            else
            {
                device.LastOnline = DateTimeOffset.Now;
                if (HostEnvironment.IsDevelopment() && RemotelyContext.Organizations.Any())
                {
                    var org = RemotelyContext.Organizations.FirstOrDefault();
                    device.Organization = org;
                    device.OrganizationID = org?.ID;
                }

                updatedDevice = device;

                if (!RemotelyContext.Organizations.Any(x => x.ID == device.OrganizationID))
                {
                    WriteEvent(new EventLog()
                    {
                        EventType = EventType.Info,
                        Message = $"Unable to add device {device.DeviceName} because organization {device.OrganizationID}" +
                            $"does not exist.  Device ID: {device.ID}.",
                        Source = "DataService.AddOrUpdateDevice"
                    });
                    return false;
                }
                RemotelyContext.Devices.Add(device);
            }
            RemotelyContext.SaveChanges();
            return true;
        }

        public async Task<string> AddSharedFile(IFormFile file, string organizationID)
        {
            var expirationDate = DateTimeOffset.Now.AddDays(-AppConfig.DataRetentionInDays);
            var expiredFiles = RemotelyContext.SharedFiles.Where(x => x.Timestamp < expirationDate);
            RemotelyContext.RemoveRange(expiredFiles);

            byte[] fileContents;
            using (var stream = file.OpenReadStream())
            {
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                fileContents = ms.ToArray();
            }
            var newEntity = RemotelyContext.Add(new SharedFile()
            {
                FileContents = fileContents,
                FileName = file.FileName,
                ContentType = file.ContentType,
                OrganizationID = organizationID
            });
            await RemotelyContext.SaveChangesAsync();
            return newEntity.Entity.ID;
        }

        public bool AddUserToDeviceGroup(string orgID, string groupID, string userName, out string resultMessage)
        {
            resultMessage = string.Empty;

            var deviceGroup = RemotelyContext.DeviceGroups
                .Include(x => x.PermissionLinks)
                .FirstOrDefault(x =>
                    x.ID == groupID &&
                    x.OrganizationID == orgID);

            if (deviceGroup == null)
            {
                resultMessage = "Device group not found.";
                return false;
            }

            userName = userName.Trim().ToLower();

            var user = RemotelyContext.Users
                .Include(x => x.PermissionLinks)
                .FirstOrDefault(x =>
                    x.UserName.ToLower() == userName &&
                    x.OrganizationID == orgID);

            if (user == null)
            {
                resultMessage = "User not found.";
                return false;
            }

            deviceGroup.PermissionLinks ??= new List<UserDevicePermission>();
            user.PermissionLinks ??= new List<UserDevicePermission>();

            if (deviceGroup.PermissionLinks.Any(x => x.UserID == user.Id))
            {
                resultMessage = "User already in group.";
                return false;
            }

            var link = new UserDevicePermission()
            {
                DeviceGroup = deviceGroup,
                DeviceGroupID = deviceGroup.ID,
                User = user,
                UserID = user.Id
            };

            deviceGroup.PermissionLinks.Add(link);
            user.PermissionLinks.Add(link);
            RemotelyContext.SaveChanges();
            resultMessage = user.Id;
            return true;
        }

        public void ChangeUserIsAdmin(string organizationID, string targetUserID, bool isAdmin)
        {
            var targetUser = RemotelyContext.Users.FirstOrDefault(x =>
                                x.OrganizationID == organizationID &&
                                x.Id == targetUserID);

            if (targetUser != null)
            {
                targetUser.IsAdministrator = isAdmin;
                RemotelyContext.SaveChanges();
            }
        }

        public void CleanupOldRecords()
        {
            if (AppConfig.DataRetentionInDays > 0)
            {

                var expirationDate = DateTimeOffset.Now - TimeSpan.FromDays(AppConfig.DataRetentionInDays);

                var eventLogs = RemotelyContext.EventLogs
                                    .Where(x => x.TimeStamp < expirationDate);

                RemotelyContext.RemoveRange(eventLogs);

                var commandResults = RemotelyContext.CommandResults
                                        .Where(x => x.TimeStamp < expirationDate);

                RemotelyContext.RemoveRange(commandResults);

                var sharedFiles = RemotelyContext.SharedFiles
                                        .Where(x => x.Timestamp < expirationDate);

                RemotelyContext.RemoveRange(sharedFiles);

                RemotelyContext.SaveChanges();
            }
        }

        public async Task<ApiToken> CreateApiToken(string userName, string tokenName, string secretHash)
        {
            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName);

            var newToken = new ApiToken()
            {
                Name = tokenName,
                OrganizationID = user.OrganizationID,
                Token = Guid.NewGuid().ToString(),
                Secret = secretHash
            };
            RemotelyContext.ApiTokens.Add(newToken);
            await RemotelyContext.SaveChangesAsync();
            return newToken;
        }

        public async Task<Device> CreateDevice(DeviceSetupOptions options)
        {
            try
            {
                if (options is null ||
                    string.IsNullOrWhiteSpace(options.DeviceID) ||
                    string.IsNullOrWhiteSpace(options.OrganizationID) ||
                    RemotelyContext.Devices.Any(x => x.ID == options.DeviceID))
                {
                    return null;
                }

                var device = new Device()
                {
                    ID = options.DeviceID,
                    OrganizationID = options.OrganizationID
                };

                if (!string.IsNullOrWhiteSpace(options.DeviceAlias))
                {
                    device.Alias = options.DeviceAlias;
                }

                if (!string.IsNullOrWhiteSpace(options.DeviceGroupName))
                {
                    var group = RemotelyContext.DeviceGroups.FirstOrDefault(x =>
                        x.Name.ToLower() == options.DeviceGroupName.ToLower() &&
                        x.OrganizationID == device.OrganizationID);
                    device.DeviceGroup = group;
                }

                RemotelyContext.Devices.Add(device);

                await RemotelyContext.SaveChangesAsync();

                return device;
            }
            catch (Exception ex)
            {
                WriteEvent(ex, options.OrganizationID);
                return null;
            }

        }

        public async Task<bool> CreateUser(string userEmail, bool isAdmin, string organizationID)
        {
            try
            {
                var user = new RemotelyUser()
                {
                    UserName = userEmail.Trim().ToLower(),
                    Email = userEmail.Trim().ToLower(),
                    IsAdministrator = isAdmin,
                    OrganizationID = organizationID,
                    UserOptions = new RemotelyUserOptions()
                };
                var org = RemotelyContext.Organizations
                    .Include(x => x.RemotelyUsers)
                    .FirstOrDefault(x => x.ID == organizationID);
                org.RemotelyUsers.Add(user);
                await RemotelyContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                WriteEvent(ex, organizationID);
                return false;
            }

        }

        public async Task DeleteAlert(Alert alert)
        {
            RemotelyContext.Alerts.Remove(alert);
            await RemotelyContext.SaveChangesAsync();
        }

        public async Task DeleteApiToken(string userName, string tokenId)
        {
            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName);
            var token = RemotelyContext.ApiTokens.FirstOrDefault(x =>
                x.OrganizationID == user.OrganizationID &&
                x.ID == tokenId);

            RemotelyContext.ApiTokens.Remove(token);
            await RemotelyContext.SaveChangesAsync();
        }

        public void DeleteDeviceGroup(string orgID, string deviceGroupID)
        {
            var deviceGroup = RemotelyContext.DeviceGroups
                .Include(x => x.Devices)
                .Include(x => x.PermissionLinks)
                .ThenInclude(x => x.User)
                    .FirstOrDefault(x =>
                        x.ID == deviceGroupID &&
                        x.OrganizationID == orgID);

            deviceGroup.Devices?.ForEach(x =>
            {
                x.DeviceGroup = null;
            });

            deviceGroup.PermissionLinks?.ToList()?.ForEach(x =>
            {
                x.User = null;
                x.DeviceGroup = null;

                RemotelyContext.PermissionLinks.Remove(x);
            });

            RemotelyContext.DeviceGroups.Remove(deviceGroup);

            RemotelyContext.SaveChanges();
        }

        public void DeleteInvite(string orgID, string inviteID)
        {
            var invite = RemotelyContext.InviteLinks.FirstOrDefault(x =>
                x.OrganizationID == orgID &&
                x.ID == inviteID);

            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == invite.InvitedUser);

            if (user != null && string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                RemotelyContext.Remove(user);
            }
            RemotelyContext.Remove(invite);
            RemotelyContext.SaveChanges();
        }

        public void DetachEntity(object entity)
        {
            RemotelyContext.Entry(entity).State = EntityState.Detached;
        }

        public void DeviceDisconnected(string deviceID)
        {
            var device = RemotelyContext.Devices.Find(deviceID);
            if (device != null)
            {
                device.LastOnline = DateTimeOffset.Now;
                device.IsOnline = false;
                RemotelyContext.SaveChanges();
            }
        }

        public bool DoesUserExist(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }
            return RemotelyContext.Users.Any(x => x.UserName.Trim().ToLower() == userName.Trim().ToLower());
        }

        public bool DoesUserHaveAccessToDevice(string deviceID, RemotelyUser remotelyUser)
        {
            return RemotelyContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.PermissionLinks)
                .Any(device => device.OrganizationID == remotelyUser.OrganizationID &&
                    device.ID == deviceID &&
                    (
                        remotelyUser.IsAdministrator ||
                        device.DeviceGroup.PermissionLinks.Count == 0 ||
                        device.DeviceGroup.PermissionLinks.Any(permission => permission.UserID == remotelyUser.Id
                    )));
        }

        public bool DoesUserHaveAccessToDevice(string deviceID, string remotelyUserID)
        {
            var remotelyUser = RemotelyContext.Users.Find(remotelyUserID);

            return DoesUserHaveAccessToDevice(deviceID, remotelyUser);
        }

        public string[] FilterDeviceIDsByUserPermission(string[] deviceIDs, RemotelyUser remotelyUser)
        {
            return RemotelyContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.PermissionLinks)
                .Where(device =>
                    device.OrganizationID == remotelyUser.OrganizationID &&
                    deviceIDs.Contains(device.ID) &&
                    (
                        remotelyUser.IsAdministrator ||
                        device.DeviceGroup.PermissionLinks.Count == 0 ||
                        device.DeviceGroup.PermissionLinks.Any(permission => permission.UserID == remotelyUser.Id
                    )))
                .Select(x => x.ID)
                .ToArray();
        }

        public string[] FilterUsersByDevicePermission(IEnumerable<string> userIDs, string deviceID)
        {
            var device = RemotelyContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.PermissionLinks)
                .FirstOrDefault(x => x.ID == deviceID);

            var allowedUsers = device?.DeviceGroup?.PermissionLinks?.Select(x => x.UserID) ?? Array.Empty<string>();

            return RemotelyContext.Users
                .Include(x => x.PermissionLinks)
                .Where(user =>
                    user.OrganizationID == device.OrganizationID &&
                    userIDs.Contains(user.Id) &&
                    (
                        user.IsAdministrator ||
                        allowedUsers.Any() ||
                        allowedUsers.Contains(user.Id)
                    )
                )
                .Select(x => x.Id)
                .ToArray();
        }

        public async Task<Alert> GetAlert(string alertID)
        {
            return await RemotelyContext.Alerts
                .Include(x => x.Device)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ID == alertID);
        }

        public IEnumerable<Alert> GetAlerts(string userID)
        {
            return RemotelyContext.Alerts
                .Include(x => x.Device)
                .Include(x => x.User)
                .Where(x => x.UserID == userID)
                .OrderByDescending(x => x.CreatedOn);
        }

        public IEnumerable<ApiToken> GetAllApiTokens(string userID)
        {
            var user = RemotelyContext.Users.FirstOrDefault(x => x.Id == userID);

            return RemotelyContext.ApiTokens
                .Where(x => x.OrganizationID == user.OrganizationID)
                .OrderByDescending(x => x.LastUsed);
        }

        public IEnumerable<CommandResult> GetAllCommandResults(string orgID)
        {
            return RemotelyContext.CommandResults
                .Where(x => x.OrganizationID == orgID)
                .OrderByDescending(x => x.TimeStamp);
        }

        public IEnumerable<Device> GetAllDevices(string orgID)
        {
            return RemotelyContext.Devices.Where(x => x.OrganizationID == orgID);
        }

        public IEnumerable<EventLog> GetAllEventLogs(string orgID)
        {
            return RemotelyContext.EventLogs
                .Where(x => x.OrganizationID == orgID)
                .OrderByDescending(x => x.TimeStamp);
        }

        public ICollection<InviteLink> GetAllInviteLinks(string userName)
        {
            return RemotelyContext.Users
                   .Include(x => x.Organization)
                   .ThenInclude(x => x.InviteLinks)
                   .FirstOrDefault(x => x.UserName == userName)
                   .Organization
                   .InviteLinks ?? Array.Empty<InviteLink>();
        }

        public IEnumerable<RemotelyUser> GetAllUsers(string userName)
        {
            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName);
            return RemotelyContext.Users.Where(x => x.OrganizationID == user.OrganizationID);
        }

        public ApiToken GetApiToken(string apiToken)
        {
            return RemotelyContext.ApiTokens.FirstOrDefault(x => x.Token == apiToken);
        }

        public CommandResult GetCommandResult(string commandResultID, string orgID)
        {
            return RemotelyContext.CommandResults
                .FirstOrDefault(x =>
                    x.OrganizationID == orgID &&
                    x.ID == commandResultID);
        }

        public CommandResult GetCommandResult(string commandResultID)
        {
            return RemotelyContext.CommandResults.Find(commandResultID);
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

        public Device GetDevice(string orgID, string deviceID)
        {
            return RemotelyContext.Devices.FirstOrDefault(x =>
                            x.OrganizationID == orgID &&
                            x.ID == deviceID);
        }

        public Device GetDevice(string deviceID)
        {
            return RemotelyContext.Devices.FirstOrDefault(x => x.ID == deviceID);
        }

        public int GetDeviceCount()
        {
            return RemotelyContext.Devices.Count();
        }

        public IEnumerable<DeviceGroup> GetDeviceGroups(string username)
        {
            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == username);

            if (user is null)
            {
                return null;
            }

            return RemotelyContext.DeviceGroups
                .Include(x => x.PermissionLinks)
                .ThenInclude(x => x.User)
                .Where(x =>
                    x.OrganizationID == user.OrganizationID &&
                    (
                        user.IsAdministrator ||
                        x.PermissionLinks.Count == 0 ||
                        x.PermissionLinks.Any(x => x.UserID == user.Id)
                    )
                )
                .OrderBy(x => x.Name) ?? Enumerable.Empty<DeviceGroup>();
        }

        public IEnumerable<Device> GetDevicesForUser(string userName)
        {
            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName);
            var userID = user.Id;

            return RemotelyContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.PermissionLinks)
                .Where(x =>
                    x.OrganizationID == user.OrganizationID &&
                    (
                        user.IsAdministrator ||
                        x.DeviceGroup.PermissionLinks.Count == 0 ||
                        x.DeviceGroup.PermissionLinks.Any(permission => permission.UserID == userID
                    )));
        }

        public IEnumerable<EventLog> GetEventLogs(string userName, DateTimeOffset from, DateTimeOffset to, EventType? type, string message)
        {
            var user = RemotelyContext.Users
                        .FirstOrDefault(x => x.UserName == userName);

            var query = RemotelyContext.EventLogs.AsQueryable();
            var fromDate = from.Date;
            var toDate = to.Date.AddDays(1);

            if (user.IsServerAdmin)
            {
                query = query.Where(x => x.TimeStamp >= fromDate && x.TimeStamp <= toDate)
                            .OrderByDescending(x => x.TimeStamp);
            }
            else
            {
                var orgID = user.OrganizationID;
                query = query.Where(x => x.OrganizationID == orgID && x.TimeStamp >= fromDate && x.TimeStamp <= toDate)
                        .OrderByDescending(x => x.TimeStamp);
            }
            if (type != null)
            {
                query = query.Where(x => x.EventType == type);
            }
            if (!string.IsNullOrWhiteSpace(message))
            {
                message = message.ToLower();
                query = query.Where(x => x.Message.ToLower().Contains(message));
            }
            return query;
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

        public string GetOrganizationNameById(string organizationID)
        {
            return RemotelyContext.Organizations.FirstOrDefault(x => x.ID == organizationID)?.OrganizationName;
        }

        public List<string> GetServerAdmins()
        {
            return RemotelyContext.Users
                .Where(x => x.IsServerAdmin)
                .Select(x => x.UserName)
                .ToList();
        }

        public SharedFile GetSharedFiled(string fileID)
        {
            return RemotelyContext.SharedFiles.Find(fileID);
        }

        public int GetTotalDevices()
        {
            return RemotelyContext.Devices.Count();
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
            var invite = RemotelyContext.InviteLinks.FirstOrDefault(x =>
                            x.InvitedUser.ToLower() == userName.ToLower() &&
                            x.ID == inviteID);

            if (invite == null)
            {
                return false;
            }

            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName);
            var organization = RemotelyContext.Organizations
                                .Include(x => x.RemotelyUsers)
                                .FirstOrDefault(x => x.ID == invite.OrganizationID);

            user.Organization = organization;
            user.OrganizationID = organization.ID;
            user.IsAdministrator = invite.IsAdmin;
            organization.RemotelyUsers.Add(user);

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

        public async Task<bool> RemoveUserFromDeviceGroup(string orgID, string groupID, string userID)
        {
            var deviceGroup = RemotelyContext.DeviceGroups
                .Include(x => x.PermissionLinks)
                .ThenInclude(x => x.User)
                .FirstOrDefault(x =>
                    x.ID == groupID &&
                    x.OrganizationID == orgID);

            if (deviceGroup?.PermissionLinks?.Any(x => x.UserID == userID) == true)
            {
                var link = deviceGroup.PermissionLinks.FirstOrDefault(x => x.UserID == userID);

                link.User = null;
                link.DeviceGroup = null;

                RemotelyContext.PermissionLinks.Remove(link);

                await RemotelyContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task RemoveUserFromOrganization(string orgID, string targetUserID)
        {
            var target = RemotelyContext.Users
                .Include(x => x.PermissionLinks)
                .ThenInclude(x => x.DeviceGroup)
                .Include(x => x.Organization)
                .Include(x => x.Alerts)
                .FirstOrDefault(x =>
                    x.Id == targetUserID &&
                    x.OrganizationID == orgID);

            if (target?.PermissionLinks?.Any() == true)
            {
                foreach (var link in target.PermissionLinks.ToList())
                {
                    link.DeviceGroup = null;
                    link.User = null;

                    RemotelyContext.PermissionLinks.Remove(link);
                }
            }

            foreach (var alert in target.Alerts)
            {
                RemotelyContext.Alerts.Remove(alert);
            }

            target.OrganizationID = null;
            target.Organization = null;

            RemotelyContext
                .Organizations
                .Include(x => x.RemotelyUsers)
                .FirstOrDefault(x => x.ID == orgID)
                .RemotelyUsers.Remove(target);


            RemotelyContext.Users.Remove(target);

            await UserManager.DeleteAsync(target);

            await RemotelyContext.SaveChangesAsync();

        }

        public async Task RenameApiToken(string userName, string tokenId, string tokenName)
        {
            var user = RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName);
            var token = RemotelyContext.ApiTokens.FirstOrDefault(x =>
                x.OrganizationID == user.OrganizationID &&
                x.ID == tokenId);

            token.Name = tokenName;
            await RemotelyContext.SaveChangesAsync();
        }

        public void SetAllDevicesNotOnline()
        {
            RemotelyContext.Devices.ForEachAsync(x =>
            {
                x.IsOnline = false;
            }).Wait();
            RemotelyContext.SaveChanges();
        }



        public async Task SetDisplayName(RemotelyUser user, string displayName)
        {
            RemotelyContext.Attach(user);
            user.DisplayName = displayName;
            await RemotelyContext.SaveChangesAsync();
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

        public void UpdateDevice(string deviceID, string tag, string alias, string deviceGroupID, string notes)
        {
            var device = RemotelyContext.Devices.Find(deviceID);
            if (device == null)
            {
                return;
            }

            device.Tags = tag;
            device.DeviceGroupID = deviceGroupID;
            device.Alias = alias;
            device.Notes = notes;
            RemotelyContext.SaveChanges();
        }

        public async Task<Device> UpdateDevice(DeviceSetupOptions deviceOptions, string organizationId)
        {
            var device = RemotelyContext.Devices.Find(deviceOptions.DeviceID);
            if (device == null || device.OrganizationID != organizationId)
            {
                return null;
            }

            var group = await RemotelyContext.DeviceGroups.FirstOrDefaultAsync(x =>
              x.Name.ToLower() == deviceOptions.DeviceGroupName.ToLower() &&
              x.OrganizationID == device.OrganizationID);
            device.DeviceGroup = group;

            device.Alias = deviceOptions.DeviceAlias;
            await RemotelyContext.SaveChangesAsync();
            return device;
        }

        public void UpdateOrganizationName(string orgID, string organizationName)
        {
            RemotelyContext.Organizations
                .FirstOrDefault(x => x.ID == orgID)
                .OrganizationName = organizationName;
            RemotelyContext.SaveChanges();
        }

        public async Task UpdateServerAdmins(List<string> serverAdmins, string callerUserName)
        {
            var currentAdmins = RemotelyContext.Users.Where(x => x.IsServerAdmin).ToList();

            var removeAdmins = currentAdmins.Where(currentAdmin =>
                !serverAdmins.Contains(currentAdmin.UserName.Trim().ToLower()) &&
                currentAdmin.UserName.Trim().ToLower() != callerUserName.Trim().ToLower());

            foreach (var removeAdmin in removeAdmins)
            {
                removeAdmin.IsServerAdmin = false;
            }

            var newAdmins = RemotelyContext.Users.Where(user =>
                serverAdmins.Contains(user.UserName.Trim().ToLower()) &&
                !user.IsServerAdmin);

            foreach (var newAdmin in newAdmins)
            {
                newAdmin.IsServerAdmin = true;
            }

            await RemotelyContext.SaveChangesAsync();
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

        public void UpdateUserOptions(string userName, RemotelyUserOptions options)
        {
            RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName).UserOptions = options;
            RemotelyContext.SaveChanges();
        }

        public bool ValidateApiToken(string apiToken, string apiSecret, string requestPath, string remoteIP)
        {
            var hasher = new PasswordHasher<RemotelyUser>();
            var token = RemotelyContext.ApiTokens.FirstOrDefault(x => x.Token == apiToken);
            var isValid = token != null && hasher.VerifyHashedPassword(null, token.Secret, apiSecret) == PasswordVerificationResult.Success;

            if (token != null)
            {
                token.LastUsed = DateTimeOffset.Now;
                RemotelyContext.SaveChanges();
            }

            WriteEvent($"API token used.  Token: {apiToken}.  Path: {requestPath}.  Validated: {isValid}.  Remote IP: {remoteIP}", EventType.Info, token?.OrganizationID);

            return isValid;
        }

        public void WriteEvent(EventLog eventLog)
        {
            try
            {
                RemotelyContext.EventLogs.Add(eventLog);
                RemotelyContext.SaveChanges();
            }
            catch { }
        }

        public void WriteEvent(Exception ex, string organizationID)
        {
            try
            {
                RemotelyContext.EventLogs.Add(new EventLog()
                {
                    EventType = EventType.Error,
                    Message = ex.Message,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    TimeStamp = DateTimeOffset.Now,
                    OrganizationID = organizationID
                });
                RemotelyContext.SaveChanges();
            }
            catch { }
        }

        public void WriteEvent(string message, string organizationID)
        {
            try
            {
                RemotelyContext.EventLogs.Add(new EventLog()
                {
                    EventType = EventType.Info,
                    Message = message,
                    TimeStamp = DateTimeOffset.Now,
                    OrganizationID = organizationID
                });
                RemotelyContext.SaveChanges();
            }
            catch { }
        }

        public void WriteEvent(string message, EventType eventType, string organizationID)
        {
            try
            {
                RemotelyContext.EventLogs.Add(new EventLog()
                {
                    EventType = eventType,
                    Message = message,
                    TimeStamp = DateTimeOffset.Now,
                    OrganizationID = organizationID
                });
                RemotelyContext.SaveChanges();
            }
            catch { }
        }

        public void WriteLog(LogLevel logLevel, string category, EventId eventId, string state, Exception exception, List<string> scopeStack)
        {
            // Prevent re-entrancy.
            if (eventId.Name?.Contains("EntityFrameworkCore") == true)
            {
                return;
            }

            try
            {
                // TODO: Refactor EventLog to resemble these params.  Replace WriteEvent with ILogger<T>.

                EventType eventType = EventType.Debug;
                switch (logLevel)
                {
                    case LogLevel.None:
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                        eventType = EventType.Debug;
                        break;
                    case LogLevel.Information:
                        eventType = EventType.Debug;
                        break;
                    case LogLevel.Warning:
                        eventType = EventType.Warning;
                        break;
                    case LogLevel.Error:
                    case LogLevel.Critical:
                        eventType = EventType.Error;
                        break;
                    default:
                        break;
                }

                RemotelyContext.EventLogs.Add(new EventLog()
                {
                    StackTrace = exception?.StackTrace,
                    EventType = eventType,
                    Message = $"[{logLevel}] [{string.Join(" - ", scopeStack)} - {category}] | Message: {state} | Exception: {exception?.Message}",
                    TimeStamp = DateTimeOffset.Now
                });

                RemotelyContext.SaveChanges();
            }
            catch { }

        }
    }
}
