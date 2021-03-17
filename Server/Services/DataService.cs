using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Remotely.Server.Areas.Identity.Pages.Account.Manage;
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
    // TODO: Separate this into domains-specific services.
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

        Task ClearLogs(string currentUserName);

        Task<ApiToken> CreateApiToken(string userName, string tokenName, string secretHash);

        Task<Device> CreateDevice(DeviceSetupOptions options);

        Task<bool> CreateUser(string userEmail, bool isAdmin, string organizationID);

        Task DeleteAlert(Alert alert);

        Task DeleteAllAlerts(string orgID, string userName = null);

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

        Task<BrandingInfo> GetBrandingInfo(string organizationId);

        CommandResult GetCommandResult(string commandResultID);

        CommandResult GetCommandResult(string commandResultID, string orgID);

        Task<Organization> GetDefaultOrganization();

        string GetDefaultPrompt();

        string GetDefaultPrompt(string userName);

        Task<string> GetDefaultRelayCode();

        Device GetDevice(string deviceID);

        Device GetDevice(string orgID, string deviceID);

        int GetDeviceCount();

        IEnumerable<DeviceGroup> GetDeviceGroups(string username);

        IEnumerable<Device> GetDevicesForUser(string userName);

        IEnumerable<EventLog> GetEventLogs(string userName, DateTimeOffset from, DateTimeOffset to, EventType? type, string message);
        Organization GetOrganizationById(string organizationID);

        Task<Organization> GetOrganizationByRelayCode(string relayCode);
        Task<Organization> GetOrganizationByUserName(string userName);

        int GetOrganizationCount();

        string GetOrganizationNameById(string organizationID);

        string GetOrganizationNameByUserName(string userName);

        List<string> GetServerAdmins();

        SharedFile GetSharedFiled(string fileID);

        int GetTotalDevices();

        RemotelyUser GetUserByID(string userID);

        RemotelyUser GetUserByName(string userName);

        RemotelyUserOptions GetUserOptions(string userName);

        bool JoinViaInvitation(string userName, string inviteID);

        void PopulateRelayCodes();

        void RemoveDevices(string[] deviceIDs);

        Task<bool> RemoveUserFromDeviceGroup(string orgID, string groupID, string userID);

        Task RemoveUserFromOrganization(string orgID, string targetUserID);

        Task RenameApiToken(string userName, string tokenId, string tokenName);

        void SetAllDevicesNotOnline();

        Task SetDisplayName(RemotelyUser user, string displayName);

        Task SetIsDefaultOrganization(string orgID, bool isDefault);

        void SetServerVerificationToken(string deviceID, string verificationToken);

        Task<bool> TempPasswordSignIn(string email, string password);

        Task UpdateBrandingInfo(
                                                                                                                                                                                                                                                    string organizationId,
            string productName, 
            IFormFile icon,
            ColorPickerModel titleForeground, 
            ColorPickerModel titleBackground, 
            ColorPickerModel titleButtonForeground);
        Task<Device> UpdateDevice(DeviceSetupOptions deviceOptions, string organizationId);
        void UpdateDevice(string deviceID, string tag, string alias, string deviceGroupID, string notes, WebRtcSetting webRtcSetting);
        void UpdateOrganizationName(string orgID, string organizationName);
        Task UpdateServerAdmins(List<string> serverAdmins, string callerUserName);
        void UpdateTags(string deviceID, string tags);
        void UpdateUserOptions(string userName, RemotelyUserOptions options);
        bool ValidateApiToken(string apiToken, string apiSecret, string requestPath, string remoteIP);
        void WriteEvent(EventLog eventLog);
        void WriteEvent(Exception ex, string organizationID);
        void WriteEvent(string message, EventType eventType, string organizationID);
        void WriteEvent(string message, string organizationID);
        void WriteLog(LogLevel logLevel, string category, EventId eventId, string state, Exception exception, string[] scopeStack);
    }

    public class DataService : IDataService
    {
        private readonly IApplicationConfig _appConfig;

        private readonly ApplicationDbContext _dbContext;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly UserManager<RemotelyUser> _userManager;

        public DataService(ApplicationDbContext context,
                                            IApplicationConfig appConfig,
            IHostEnvironment hostEnvironment,
            UserManager<RemotelyUser> userManager)
        {
            _dbContext = context;
            _appConfig = appConfig;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }
        public async Task AddAlert(AlertOptions alertOptions, string organizationID)
        {
            var users = _dbContext.Users
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

            await _dbContext.SaveChangesAsync();
        }

        public bool AddDeviceGroup(string orgID, DeviceGroup deviceGroup, out string deviceGroupID, out string errorMessage)
        {
            deviceGroupID = null;
            errorMessage = null;

            var organization = _dbContext.Organizations
                .Include(x => x.DeviceGroups)
                .FirstOrDefault(x => x.ID == orgID);

            if (_dbContext.DeviceGroups.Any(x =>
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
            _dbContext.SaveChanges();
            deviceGroupID = newDeviceGroup.ID;
            return true;
        }

        public InviteLink AddInvite(string orgID, Invite invite)
        {
            invite.InvitedUser = invite.InvitedUser.ToLower();

            var organization = _dbContext.Organizations
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
            _dbContext.SaveChanges();
            return newInvite;
        }

        public void AddOrUpdateCommandResult(CommandResult commandResult)
        {
            var existingContext = _dbContext.CommandResults.Find(commandResult.ID);
            if (existingContext != null)
            {
                var entry = _dbContext.Entry(existingContext);
                entry.CurrentValues.SetValues(commandResult);
                entry.State = EntityState.Modified;
            }
            else
            {
                _dbContext.CommandResults.Add(commandResult);
            }
            _dbContext.SaveChanges();
        }

        public bool AddOrUpdateDevice(Device device, out Device updatedDevice)
        {
            var existingDevice = _dbContext.Devices.Find(device.ID);
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
                if (_hostEnvironment.IsDevelopment() && _dbContext.Organizations.Any())
                {
                    var org = _dbContext.Organizations.FirstOrDefault();
                    device.Organization = org;
                    device.OrganizationID = org?.ID;
                }

                updatedDevice = device;

                if (!_dbContext.Organizations.Any(x => x.ID == device.OrganizationID))
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
                _dbContext.Devices.Add(device);
            }
            _dbContext.SaveChanges();
            return true;
        }

        public async Task<string> AddSharedFile(IFormFile file, string organizationID)
        {
            var expirationDate = DateTimeOffset.Now.AddDays(-_appConfig.DataRetentionInDays);
            var expiredFiles = _dbContext.SharedFiles.Where(x => x.Timestamp < expirationDate);
            _dbContext.RemoveRange(expiredFiles);

            byte[] fileContents;
            using (var stream = file.OpenReadStream())
            {
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                fileContents = ms.ToArray();
            }
            var newEntity = _dbContext.Add(new SharedFile()
            {
                FileContents = fileContents,
                FileName = file.FileName,
                ContentType = file.ContentType,
                OrganizationID = organizationID
            });
            await _dbContext.SaveChangesAsync();
            return newEntity.Entity.ID;
        }

        public bool AddUserToDeviceGroup(string orgID, string groupID, string userName, out string resultMessage)
        {
            resultMessage = string.Empty;

            var deviceGroup = _dbContext.DeviceGroups
                .Include(x => x.Users)
                .FirstOrDefault(x =>
                    x.ID == groupID &&
                    x.OrganizationID == orgID);

            if (deviceGroup == null)
            {
                resultMessage = "Device group not found.";
                return false;
            }

            userName = userName.Trim().ToLower();

            var user = _dbContext.Users
                .Include(x => x.DeviceGroups)
                .FirstOrDefault(x =>
                    x.UserName.ToLower() == userName &&
                    x.OrganizationID == orgID);

            if (user == null)
            {
                resultMessage = "User not found.";
                return false;
            }

            deviceGroup.Devices ??= new List<Device>();
            user.DeviceGroups ??= new List<DeviceGroup>();

            if (deviceGroup.Users.Any(x => x.Id == user.Id))
            {
                resultMessage = "User already in group.";
                return false;
            }

            deviceGroup.Users.Add(user);
            user.DeviceGroups.Add(deviceGroup);
            _dbContext.SaveChanges();
            resultMessage = user.Id;
            return true;
        }

        public void ChangeUserIsAdmin(string organizationID, string targetUserID, bool isAdmin)
        {
            var targetUser = _dbContext.Users.FirstOrDefault(x =>
                                x.OrganizationID == organizationID &&
                                x.Id == targetUserID);

            if (targetUser != null)
            {
                targetUser.IsAdministrator = isAdmin;
                _dbContext.SaveChanges();
            }
        }

        public void CleanupOldRecords()
        {
            if (_appConfig.DataRetentionInDays > 0)
            {

                var expirationDate = DateTimeOffset.Now - TimeSpan.FromDays(_appConfig.DataRetentionInDays);

                var eventLogs = _dbContext.EventLogs
                                    .Where(x => x.TimeStamp < expirationDate);

                _dbContext.RemoveRange(eventLogs);

                var commandResults = _dbContext.CommandResults
                                        .Where(x => x.TimeStamp < expirationDate);

                _dbContext.RemoveRange(commandResults);

                var sharedFiles = _dbContext.SharedFiles
                                        .Where(x => x.Timestamp < expirationDate);

                _dbContext.RemoveRange(sharedFiles);

                _dbContext.SaveChanges();
            }
        }

        public async Task ClearLogs(string currentUserName)
        {
            var currentUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == currentUserName);
            if (currentUser is null)
            {
                return;
            }

            try
            {

                if (currentUser.IsServerAdmin)
                {
                    _dbContext.EventLogs.RemoveRange(_dbContext.EventLogs);
                    _dbContext.CommandResults.RemoveRange(_dbContext.CommandResults);
                }
                else
                {
                    var eventLogs = _dbContext.EventLogs.Where(x => x.OrganizationID == currentUser.OrganizationID);
                    var commandResults = _dbContext.CommandResults.Where(x => x.OrganizationID == currentUser.OrganizationID);

                    _dbContext.CommandResults.RemoveRange(commandResults);
                    _dbContext.EventLogs.RemoveRange(eventLogs);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                WriteEvent(ex, currentUser.OrganizationID);
            }
        }

        public async Task<ApiToken> CreateApiToken(string userName, string tokenName, string secretHash)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName);

            var newToken = new ApiToken()
            {
                Name = tokenName,
                OrganizationID = user.OrganizationID,
                Token = Guid.NewGuid().ToString(),
                Secret = secretHash
            };
            _dbContext.ApiTokens.Add(newToken);
            await _dbContext.SaveChangesAsync();
            return newToken;
        }

        public async Task<Device> CreateDevice(DeviceSetupOptions options)
        {
            try
            {
                if (options is null ||
                    string.IsNullOrWhiteSpace(options.DeviceID) ||
                    string.IsNullOrWhiteSpace(options.OrganizationID) ||
                    _dbContext.Devices.Any(x => x.ID == options.DeviceID))
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
                    var group = _dbContext.DeviceGroups.FirstOrDefault(x =>
                        x.Name.ToLower() == options.DeviceGroupName.ToLower() &&
                        x.OrganizationID == device.OrganizationID);
                    device.DeviceGroup = group;
                }

                _dbContext.Devices.Add(device);

                await _dbContext.SaveChangesAsync();

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
                var org = _dbContext.Organizations
                    .Include(x => x.RemotelyUsers)
                    .FirstOrDefault(x => x.ID == organizationID);
                _dbContext.Users.Add(user);
                org.RemotelyUsers.Add(user);
                await _dbContext.SaveChangesAsync();
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
            _dbContext.Alerts.Remove(alert);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAllAlerts(string orgID, string userName = null)
        {
            var alerts = _dbContext.Alerts.Where(x => x.OrganizationID == orgID);

            if (!string.IsNullOrWhiteSpace(userName))
            {
                var userId = GetUserByName(userName)?.Id;

                alerts = alerts.Where(x => x.UserID == userId);
            }

            _dbContext.Alerts.RemoveRange(alerts);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteApiToken(string userName, string tokenId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            var token = _dbContext.ApiTokens.FirstOrDefault(x =>
                x.OrganizationID == user.OrganizationID &&
                x.ID == tokenId);

            _dbContext.ApiTokens.Remove(token);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteDeviceGroup(string orgID, string deviceGroupID)
        {
            var deviceGroup = _dbContext.DeviceGroups
                .Include(x => x.Devices)
                .Include(x => x.Users)
                .ThenInclude(x => x.DeviceGroups)
                .FirstOrDefault(x =>
                    x.ID == deviceGroupID &&
                    x.OrganizationID == orgID);

            deviceGroup.Devices?.ForEach(x =>
            {
                x.DeviceGroup = null;
            });

            deviceGroup.Users?.ForEach(x =>
            {
                x.DeviceGroups.Remove(deviceGroup);
            });

            deviceGroup.Devices.Clear();
            deviceGroup.Users.Clear();

            _dbContext.DeviceGroups.Remove(deviceGroup);

            _dbContext.SaveChanges();
        }

        public void DeleteInvite(string orgID, string inviteID)
        {
            var invite = _dbContext.InviteLinks.FirstOrDefault(x =>
                x.OrganizationID == orgID &&
                x.ID == inviteID);

            var user = _dbContext.Users.FirstOrDefault(x => x.UserName == invite.InvitedUser);

            if (user != null && string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                _dbContext.Remove(user);
            }
            _dbContext.Remove(invite);
            _dbContext.SaveChanges();
        }

        public void DetachEntity(object entity)
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
        }

        public void DeviceDisconnected(string deviceID)
        {
            var device = _dbContext.Devices.Find(deviceID);
            if (device != null)
            {
                device.LastOnline = DateTimeOffset.Now;
                device.IsOnline = false;
                _dbContext.SaveChanges();
            }
        }


        public bool DoesUserExist(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }
            return _dbContext.Users.Any(x => x.UserName.Trim().ToLower() == userName.Trim().ToLower());
        }

        public bool DoesUserHaveAccessToDevice(string deviceID, RemotelyUser remotelyUser)
        {
            return _dbContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.Users)
                .Any(device => device.OrganizationID == remotelyUser.OrganizationID &&
                    device.ID == deviceID &&
                    (
                        remotelyUser.IsAdministrator ||
                        string.IsNullOrWhiteSpace(device.DeviceGroupID) ||
                        !device.DeviceGroup.Users.Any() ||
                        device.DeviceGroup.Users.Any(user => user.Id == remotelyUser.Id
                    )));
        }

        public bool DoesUserHaveAccessToDevice(string deviceID, string remotelyUserID)
        {
            var remotelyUser = _dbContext.Users.Find(remotelyUserID);

            return DoesUserHaveAccessToDevice(deviceID, remotelyUser);
        }

        public string[] FilterDeviceIDsByUserPermission(string[] deviceIDs, RemotelyUser remotelyUser)
        {
            return _dbContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.Users)
                .Where(device =>
                    device.OrganizationID == remotelyUser.OrganizationID &&
                    deviceIDs.Contains(device.ID) &&
                    (
                        remotelyUser.IsAdministrator ||
                        device.DeviceGroup.Users.Count == 0 ||
                        device.DeviceGroup.Users.Any(user => user.Id == remotelyUser.Id
                    )))
                .Select(x => x.ID)
                .ToArray();
        }

        public string[] FilterUsersByDevicePermission(IEnumerable<string> userIDs, string deviceID)
        {
            var device = _dbContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.Users)
                .FirstOrDefault(x => x.ID == deviceID);

            var orgUsers = _dbContext.Users
                .Where(user =>
                    user.OrganizationID == device.OrganizationID &&
                    userIDs.Contains(user.Id));

            if (string.IsNullOrWhiteSpace(device.DeviceGroupID) ||
                !device.DeviceGroup.Users.Any())
            {
                return orgUsers
                    .Select(x => x.Id)
                    .ToArray();
            }

            var allowedUsers = device?.DeviceGroup?.Users?.Select(x => x.Id) ?? Array.Empty<string>();

            return orgUsers
                .Where(user =>
                    user.IsAdministrator ||
                    allowedUsers.Contains(user.Id)
                )
                .Select(x => x.Id)
                .ToArray();
        }

        public async Task<Alert> GetAlert(string alertID)
        {
            return await _dbContext.Alerts
                .Include(x => x.Device)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ID == alertID);
        }

        public IEnumerable<Alert> GetAlerts(string userID)
        {
            return _dbContext.Alerts
                .Include(x => x.Device)
                .Include(x => x.User)
                .Where(x => x.UserID == userID)
                .OrderByDescending(x => x.CreatedOn);
        }

        public IEnumerable<ApiToken> GetAllApiTokens(string userID)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userID);

            return _dbContext.ApiTokens
                .Where(x => x.OrganizationID == user.OrganizationID)
                .OrderByDescending(x => x.LastUsed);
        }

        public IEnumerable<CommandResult> GetAllCommandResults(string orgID)
        {
            return _dbContext.CommandResults
                .Where(x => x.OrganizationID == orgID)
                .OrderByDescending(x => x.TimeStamp);
        }

        public IEnumerable<Device> GetAllDevices(string orgID)
        {
            return _dbContext.Devices.Where(x => x.OrganizationID == orgID);
        }

        public IEnumerable<EventLog> GetAllEventLogs(string orgID)
        {
            return _dbContext.EventLogs
                .Where(x => x.OrganizationID == orgID)
                .OrderByDescending(x => x.TimeStamp);
        }

        public ICollection<InviteLink> GetAllInviteLinks(string userName)
        {
            return _dbContext.Users
                   .Include(x => x.Organization)
                   .ThenInclude(x => x.InviteLinks)
                   .FirstOrDefault(x => x.UserName == userName)
                   .Organization
                   .InviteLinks ?? Array.Empty<InviteLink>();
        }

        public IEnumerable<RemotelyUser> GetAllUsers(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return Array.Empty<RemotelyUser>();
            }

            var user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            return _dbContext.Users.Where(x => x.OrganizationID == user.OrganizationID);
        }

        public ApiToken GetApiToken(string apiToken)
        {
            if (string.IsNullOrWhiteSpace(apiToken))
            {
                return null;
            }

            return _dbContext.ApiTokens.FirstOrDefault(x => x.Token == apiToken);
        }

        public async Task<BrandingInfo> GetBrandingInfo(string organizationId)
        {
            if (string.IsNullOrWhiteSpace(organizationId))
            {
                return null;
            }

            var organization = await _dbContext.Organizations
              .Include(x => x.BrandingInfo)
              .FirstOrDefaultAsync(x => x.ID == organizationId);

            if (organization is null)
            {
                return null;
            }

            if (organization.BrandingInfo is null)
            {
                organization.BrandingInfo = new BrandingInfo();
                await _dbContext.SaveChangesAsync();
            }
            return organization.BrandingInfo;
        }

        public CommandResult GetCommandResult(string commandResultID, string orgID)
        {
            return _dbContext.CommandResults
                .FirstOrDefault(x =>
                    x.OrganizationID == orgID &&
                    x.ID == commandResultID);
        }

        public CommandResult GetCommandResult(string commandResultID)
        {
            return _dbContext.CommandResults.Find(commandResultID);
        }

        public async Task<Organization> GetDefaultOrganization()
        {
            return await _dbContext.Organizations.FirstOrDefaultAsync(x => x.IsDefaultOrganization);
        }

        public string GetDefaultPrompt(string userName)
        {
            var userPrompt = _dbContext.Users.FirstOrDefault(x => x.UserName == userName)?.UserOptions?.ConsolePrompt;
            return userPrompt ?? _appConfig.DefaultPrompt;
        }

        public string GetDefaultPrompt()
        {
            return _appConfig.DefaultPrompt;
        }

        public async Task<string> GetDefaultRelayCode()
        {
            var relayCode = await _dbContext.Organizations
                .Where(x => x.IsDefaultOrganization)
                .Select(x => x.RelayCode)
                .FirstOrDefaultAsync();

            return relayCode;
        }

        public Device GetDevice(string orgID, string deviceID)
        {
            return _dbContext.Devices.FirstOrDefault(x =>
                            x.OrganizationID == orgID &&
                            x.ID == deviceID);
        }

        public Device GetDevice(string deviceID)
        {
            return _dbContext.Devices.FirstOrDefault(x => x.ID == deviceID);
        }

        public int GetDeviceCount()
        {
            return _dbContext.Devices.Count();
        }

        public IEnumerable<DeviceGroup> GetDeviceGroups(string username)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserName == username);

            if (user is null)
            {
                return null;
            }
            var userId = user.Id;

            return _dbContext.DeviceGroups
                .Include(x => x.Users)
                .ThenInclude(x => x.DeviceGroups)
                .Where(x =>
                    x.OrganizationID == user.OrganizationID &&
                    (
                        user.IsAdministrator ||
                        x.Users.Count == 0 ||
                        x.Users.Any(x => x.Id == userId)
                    )
                )
                .OrderBy(x => x.Name) ?? Enumerable.Empty<DeviceGroup>();
        }

        public IEnumerable<Device> GetDevicesForUser(string userName)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName);

            return _dbContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.Users)
                .Where(x =>
                    x.OrganizationID == user.OrganizationID &&
                    (
                        user.IsAdministrator ||
                        string.IsNullOrWhiteSpace(x.DeviceGroupID) ||
                        !x.DeviceGroup.Users.Any() ||
                        x.DeviceGroup.Users.Any(deviceUser => deviceUser.Id == user.Id)
                    ));
        }

        public IEnumerable<EventLog> GetEventLogs(string userName, DateTimeOffset from, DateTimeOffset to, EventType? type, string message)
        {
            var user = _dbContext.Users
                        .FirstOrDefault(x => x.UserName == userName);

            var query = _dbContext.EventLogs.AsQueryable();
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

        public Organization GetOrganizationById(string organizationID)
        {
            return _dbContext.Organizations.Find(organizationID);
        }

        public async Task<Organization> GetOrganizationByRelayCode(string relayCode)
        {
            if (string.IsNullOrWhiteSpace(relayCode))
            {
                return null;
            }

            return await _dbContext.Organizations.FirstOrDefaultAsync(x => x.RelayCode == relayCode.ToLower());
        }

        public async Task<Organization> GetOrganizationByUserName(string userName)
        {
            var user = await _dbContext
                .Users
                .Include(x => x.Organization)
                .FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());

            return user.Organization;
        }

        public int GetOrganizationCount()
        {
            return _dbContext.Organizations.Count();
        }

        public string GetOrganizationNameById(string organizationID)
        {
            return _dbContext.Organizations.FirstOrDefault(x => x.ID == organizationID)?.OrganizationName;
        }

        public string GetOrganizationNameByUserName(string userName)
        {
            return _dbContext.Users
                   .Include(x => x.Organization)
                   .FirstOrDefault(x => x.UserName == userName)
                   .Organization
                   .OrganizationName;
        }
        public List<string> GetServerAdmins()
        {
            return _dbContext.Users
                .Where(x => x.IsServerAdmin)
                .Select(x => x.UserName)
                .ToList();
        }

        public SharedFile GetSharedFiled(string fileID)
        {
            return _dbContext.SharedFiles.Find(fileID);
        }

        public int GetTotalDevices()
        {
            return _dbContext.Devices.Count();
        }

        public RemotelyUser GetUserByID(string userID)
        {
            if (userID == null)
            {
                return null;
            }
            return _dbContext.Users.FirstOrDefault(x => x.Id == userID);
        }

        public RemotelyUser GetUserByName(string userName)
        {
            if (userName == null)
            {
                return null;
            }
            return _dbContext.Users
                .Include(x => x.Organization)
                .FirstOrDefault(x => x.UserName.ToLower().Trim() == userName.ToLower().Trim());
        }

        public RemotelyUserOptions GetUserOptions(string userName)
        {
            return _dbContext.Users
                    .FirstOrDefault(x => x.UserName == userName)
                    .UserOptions;
        }

        public bool JoinViaInvitation(string userName, string inviteID)
        {
            var invite = _dbContext.InviteLinks.FirstOrDefault(x =>
                            x.InvitedUser.ToLower() == userName.ToLower() &&
                            x.ID == inviteID);

            if (invite == null)
            {
                return false;
            }

            var user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            var organization = _dbContext.Organizations
                                .Include(x => x.RemotelyUsers)
                                .FirstOrDefault(x => x.ID == invite.OrganizationID);

            user.Organization = organization;
            user.OrganizationID = organization.ID;
            user.IsAdministrator = invite.IsAdmin;
            organization.RemotelyUsers.Add(user);

            _dbContext.SaveChanges();

            _dbContext.InviteLinks.Remove(invite);
            _dbContext.SaveChanges();
            return true;
        }

        public void PopulateRelayCodes()
        {
            foreach (var organization in _dbContext.Organizations)
            {
                if (string.IsNullOrWhiteSpace(organization.RelayCode))
                {
                    do
                    {
                        organization.RelayCode = new string(Guid.NewGuid().ToString().Take(4).ToArray());
                    }
                    while (_dbContext.Organizations.Any(x => x.ID != organization.ID && x.RelayCode == organization.RelayCode));
                }
            }
            _dbContext.SaveChanges();
        }

        public void RemoveDevices(string[] deviceIDs)
        {
            var devices = _dbContext.Devices
                .Where(x => deviceIDs.Contains(x.ID));

            _dbContext.Devices.RemoveRange(devices);
            _dbContext.SaveChanges();
        }

        public async Task<bool> RemoveUserFromDeviceGroup(string orgID, string groupID, string userID)
        {
            var deviceGroup = _dbContext.DeviceGroups
                .Include(x => x.Users)
                .ThenInclude(x => x.DeviceGroups)
                .FirstOrDefault(x =>
                    x.ID == groupID &&
                    x.OrganizationID == orgID);

            if (deviceGroup?.Users?.Any(x => x.Id == userID) == true)
            {
                var user = deviceGroup.Users.FirstOrDefault(x => x.Id == userID);

                user.DeviceGroups.Remove(deviceGroup);
                deviceGroup.Users.Remove(user);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task RemoveUserFromOrganization(string orgID, string targetUserID)
        {
            var target = _dbContext.Users
                .Include(x => x.DeviceGroups)
                .ThenInclude(x => x.Devices)
                .Include(x => x.Organization)
                .Include(x => x.Alerts)
                .FirstOrDefault(x =>
                    x.Id == targetUserID &&
                    x.OrganizationID == orgID);

            if (target is null)
            {
                return;
            }

            if (target.DeviceGroups?.Any() == true)
            {
                foreach (var deviceGroup in target.DeviceGroups.ToList())
                {
                    deviceGroup.Users.Remove(target);
                }
            }

            foreach (var alert in target.Alerts)
            {
                _dbContext.Alerts.Remove(alert);
            }

            target.OrganizationID = null;
            target.Organization = null;

            _dbContext
                .Organizations
                .Include(x => x.RemotelyUsers)
                .FirstOrDefault(x => x.ID == orgID)
                .RemotelyUsers.Remove(target);


            _dbContext.Users.Remove(target);

            await _userManager.DeleteAsync(target);

            await _dbContext.SaveChangesAsync();

        }

        public async Task RenameApiToken(string userName, string tokenId, string tokenName)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            var token = _dbContext.ApiTokens.FirstOrDefault(x =>
                x.OrganizationID == user.OrganizationID &&
                x.ID == tokenId);

            token.Name = tokenName;
            await _dbContext.SaveChangesAsync();
        }

        public void SetAllDevicesNotOnline()
        {
            _dbContext.Devices.ForEachAsync(x =>
            {
                x.IsOnline = false;
            }).Wait();
            _dbContext.SaveChanges();
        }

        public async Task SetDisplayName(RemotelyUser user, string displayName)
        {
            _dbContext.Attach(user);
            user.DisplayName = displayName;
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetIsDefaultOrganization(string orgID, bool isDefault)
        {
            var organization = await _dbContext.Organizations.FindAsync(orgID);
            if (organization is null)
            {
                return;
            }

            if (isDefault)
            {
                await _dbContext.Organizations.ForEachAsync(x => x.IsDefaultOrganization = false);
            }

            organization.IsDefaultOrganization = isDefault;
            await _dbContext.SaveChangesAsync();
        }

        public void SetServerVerificationToken(string deviceID, string verificationToken)
        {
            var device = _dbContext.Devices.Find(deviceID);
            if (device != null)
            {
                device.ServerVerificationToken = verificationToken;
                _dbContext.SaveChanges();
            }
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
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task UpdateBrandingInfo(
            string organizationId,
            string productName,
            IFormFile icon,
            ColorPickerModel titleForeground,
            ColorPickerModel titleBackground,
            ColorPickerModel titleButtonForeground)
        {
            var organization = await _dbContext.Organizations
                .Include(x => x.BrandingInfo)
                .FirstOrDefaultAsync(x => x.ID == organizationId);

            if (organization is null)
            {
                return;
            }

            if (organization.BrandingInfo is null)
            {
                organization.BrandingInfo = new BrandingInfo();
            }

            organization.BrandingInfo.Product = productName;

            if (icon != null)
            {
                using var iconStream = icon.OpenReadStream();
                organization.BrandingInfo.Icon = new byte[iconStream.Length];
                iconStream.Read(organization.BrandingInfo.Icon, 0, organization.BrandingInfo.Icon.Length);
            }

            organization.BrandingInfo.TitleBackgroundRed = titleBackground.Red;
            organization.BrandingInfo.TitleBackgroundGreen = titleBackground.Green;
            organization.BrandingInfo.TitleBackgroundBlue = titleBackground.Blue;

            organization.BrandingInfo.TitleForegroundRed = titleForeground.Red;
            organization.BrandingInfo.TitleForegroundGreen = titleForeground.Green;
            organization.BrandingInfo.TitleForegroundBlue = titleForeground.Blue;

            organization.BrandingInfo.ButtonForegroundRed = titleButtonForeground.Red;
            organization.BrandingInfo.ButtonForegroundGreen = titleButtonForeground.Green;
            organization.BrandingInfo.ButtonForegroundBlue = titleButtonForeground.Blue;

            await _dbContext.SaveChangesAsync();
        }

        public void UpdateDevice(string deviceID, string tag, string alias, string deviceGroupID, string notes, WebRtcSetting webRtcSetting)
        {
            var device = _dbContext.Devices.Find(deviceID);
            if (device == null)
            {
                return;
            }

            device.Tags = tag;
            device.DeviceGroupID = deviceGroupID;
            device.Alias = alias;
            device.Notes = notes;
            device.WebRtcSetting = webRtcSetting;
            _dbContext.SaveChanges();
        }

        public async Task<Device> UpdateDevice(DeviceSetupOptions deviceOptions, string organizationId)
        {
            var device = _dbContext.Devices.Find(deviceOptions.DeviceID);
            if (device == null || device.OrganizationID != organizationId)
            {
                return null;
            }

            var group = await _dbContext.DeviceGroups.FirstOrDefaultAsync(x =>
              x.Name.ToLower() == deviceOptions.DeviceGroupName.ToLower() &&
              x.OrganizationID == device.OrganizationID);
            device.DeviceGroup = group;

            device.Alias = deviceOptions.DeviceAlias;
            await _dbContext.SaveChangesAsync();
            return device;
        }

        public void UpdateOrganizationName(string orgID, string organizationName)
        {
            _dbContext.Organizations
                .FirstOrDefault(x => x.ID == orgID)
                .OrganizationName = organizationName;
            _dbContext.SaveChanges();
        }


        public async Task UpdateServerAdmins(List<string> serverAdmins, string callerUserName)
        {
            var currentAdmins = _dbContext.Users.Where(x => x.IsServerAdmin).ToList();

            var removeAdmins = currentAdmins.Where(currentAdmin =>
                !serverAdmins.Contains(currentAdmin.UserName.Trim().ToLower()) &&
                currentAdmin.UserName.Trim().ToLower() != callerUserName.Trim().ToLower());

            foreach (var removeAdmin in removeAdmins)
            {
                removeAdmin.IsServerAdmin = false;
            }

            var newAdmins = _dbContext.Users.Where(user =>
                serverAdmins.Contains(user.UserName.Trim().ToLower()) &&
                !user.IsServerAdmin);

            foreach (var newAdmin in newAdmins)
            {
                newAdmin.IsServerAdmin = true;
            }

            await _dbContext.SaveChangesAsync();
        }

        public void UpdateTags(string deviceID, string tags)
        {
            var device = _dbContext.Devices.Find(deviceID);
            if (device == null)
            {
                return;
            }

            device.Tags = tags;
            _dbContext.SaveChanges();
        }

        public void UpdateUserOptions(string userName, RemotelyUserOptions options)
        {
            _dbContext.Users.FirstOrDefault(x => x.UserName == userName).UserOptions = options;
            _dbContext.SaveChanges();
        }

        public bool ValidateApiToken(string apiToken, string apiSecret, string requestPath, string remoteIP)
        {
            var hasher = new PasswordHasher<RemotelyUser>();
            var token = _dbContext.ApiTokens.FirstOrDefault(x => x.Token == apiToken);
            var isValid = token != null && hasher.VerifyHashedPassword(null, token.Secret, apiSecret) == PasswordVerificationResult.Success;

            if (token != null)
            {
                token.LastUsed = DateTimeOffset.Now;
                _dbContext.SaveChanges();
            }

            WriteEvent($"API token used.  Token: {apiToken}.  Path: {requestPath}.  Validated: {isValid}.  Remote IP: {remoteIP}", EventType.Info, token?.OrganizationID);

            return isValid;
        }

        public void WriteEvent(EventLog eventLog)
        {
            try
            {
                _dbContext.EventLogs.Add(eventLog);
                _dbContext.SaveChanges();
            }
            catch { }
        }

        public void WriteEvent(Exception ex, string organizationID)
        {
            try
            {
                _dbContext.EventLogs.Add(new EventLog()
                {
                    EventType = EventType.Error,
                    Message = ex.Message,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    TimeStamp = DateTimeOffset.Now,
                    OrganizationID = organizationID
                });
                _dbContext.SaveChanges();
            }
            catch { }
        }

        public void WriteEvent(string message, string organizationID)
        {
            WriteEvent(message, EventType.Info, organizationID);
        }

        public void WriteEvent(string message, EventType eventType, string organizationID)
        {
            try
            {
                _dbContext.EventLogs.Add(new EventLog()
                {
                    EventType = eventType,
                    Message = message,
                    TimeStamp = DateTimeOffset.Now,
                    OrganizationID = organizationID
                });
                _dbContext.SaveChanges();
            }
            catch { }
        }

        public void WriteLog(LogLevel logLevel, string category, EventId eventId, string state, Exception exception, string[] scopeStack)
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

                _dbContext.EventLogs.Add(new EventLog()
                {
                    StackTrace = exception?.StackTrace,
                    EventType = eventType,
                    Message = $"[{logLevel}] [{string.Join(" - ", scopeStack)} - {category}] | Message: {state} | Exception: {exception?.Message}",
                    TimeStamp = DateTimeOffset.Now
                });

                _dbContext.SaveChanges();
            }
            catch { }

        }
    }
}
