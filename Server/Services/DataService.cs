using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Remotely.Server.Data;
using Remotely.Server.Models;
using Remotely.Shared.Enums;
using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using Remotely.Shared.ViewModels;
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
        Task AddAlert(string deviceID, string organizationID, string alertMessage, string details = null);
        bool AddDeviceGroup(string orgID, DeviceGroup deviceGroup, out string deviceGroupID, out string errorMessage);

        InviteLink AddInvite(string orgID, InviteViewModel invite);

        bool AddOrUpdateDevice(Device device, out Device updatedDevice);

        Task AddOrUpdateSavedScript(SavedScript script, string userId);

        void AddOrUpdateScriptResult(ScriptResult scriptResult);

        Task AddOrUpdateScriptSchedule(ScriptSchedule schedule);

        Task AddScriptRun(ScriptRun scriptRun);

        Task<string> AddSharedFile(IBrowserFile file, string organizationID, Action<double, string> progressCallback);

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

        Task DeleteSavedScript(Guid scriptId);

        Task DeleteScriptSchedule(int scriptScheduleId);

        Task DeleteUser(string orgID, string targetUserID);

        void DetachEntity(object entity);

        void DeviceDisconnected(string deviceID);

        bool DoesUserExist(string userName);

        bool DoesUserHaveAccessToDevice(string deviceID, RemotelyUser remotelyUser);

        bool DoesUserHaveAccessToDevice(string deviceID, string remotelyUserID);

        string[] FilterDeviceIDsByUserPermission(string[] deviceIDs, RemotelyUser remotelyUser);
        Task AddScriptResultToScriptRun(string scriptResultId, int scriptRunId);
        string[] FilterUsersByDevicePermission(IEnumerable<string> userIDs, string deviceID);

        Task<Alert> GetAlert(string alertID);

        Alert[] GetAlerts(string userID);

        ApiToken[] GetAllApiTokens(string userID);

        ScriptResult[] GetAllCommandResults(string orgID);

        ScriptResult[] GetAllCommandResultsForUser(string orgId, string userName, string deviceId);

        Device[] GetAllDevices(string orgID);

        EventLog[] GetAllEventLogs(string orgID);

        InviteLink[] GetAllInviteLinks(string userName);

        ScriptResult[] GetAllScriptResults(string orgId, string deviceId);

        ScriptResult[] GetAllScriptResultsForUser(string orgId, string userName);

        RemotelyUser[] GetAllUsersForServer();

        Task<RemotelyUser[]> GetAllUsersInOrganization(string orgId);

        ApiToken GetApiKey(string keyId);

        Task<BrandingInfo> GetBrandingInfo(string organizationId);

        Task<Organization> GetDefaultOrganization();

        Task<string> GetDefaultRelayCode();

        Device GetDevice(string deviceID);

        Device GetDevice(string orgID, string deviceID);

        int GetDeviceCount();

        int GetDeviceCount(RemotelyUser user);

        DeviceGroup[] GetDeviceGroups(string username);

        DeviceGroup[] GetDeviceGroupsForOrganization(string organizationId);

        List<Device> GetDevices(IEnumerable<string> deviceIds);

        Device[] GetDevicesForUser(string userName);

        EventLog[] GetEventLogs(string userName, DateTimeOffset from, DateTimeOffset to, EventType? type, string message);

        Organization GetOrganizationById(string organizationID);

        Task<Organization> GetOrganizationByRelayCode(string relayCode);

        Task<Organization> GetOrganizationByUserName(string userName);

        int GetOrganizationCount();

        string GetOrganizationNameById(string organizationID);

        string GetOrganizationNameByUserName(string userName);

        Task<List<ScriptRun>> GetPendingScriptRuns(string deviceId);

        Task<List<SavedScript>> GetQuickScripts(string userId);

        Task<SavedScript> GetSavedScript(Guid scriptId);

        Task<SavedScript> GetSavedScript(string userId, Guid scriptId);

        Task<List<SavedScript>> GetSavedScriptsWithoutContent(string userId, string organizationId);

        ScriptResult GetScriptResult(string scriptResultId);

        ScriptResult GetScriptResult(string scriptResultId, string orgID);

        Task<List<ScriptSchedule>> GetScriptSchedules(string organizationID);

        Task<List<ScriptSchedule>> GetScriptSchedulesDue();
        List<string> GetServerAdmins();

        SharedFile GetSharedFiled(string fileID);

        int GetTotalDevices();

        Task<RemotelyUser> GetUserAsync(string username);

        RemotelyUser GetUserByID(string userID);

        RemotelyUser GetUserByNameWithOrg(string userName);

        RemotelyUserOptions GetUserOptions(string userName);

        bool JoinViaInvitation(string userName, string inviteID);

        void PopulateRelayCodes();

        void RemoveDevices(string[] deviceIDs);

        Task<bool> RemoveUserFromDeviceGroup(string orgID, string groupID, string userID);
        Task RenameApiToken(string userName, string tokenId, string tokenName);

        void SetAllDevicesNotOnline();

        Task SetDisplayName(RemotelyUser user, string displayName);

        Task SetIsDefaultOrganization(string orgID, bool isDefault);

        Task SetIsServerAdmin(string targetUserId, bool isServerAdmin, string callerUserId);

        void SetServerVerificationToken(string deviceID, string verificationToken);

        Task<bool> TempPasswordSignIn(string email, string password);

        Task UpdateBrandingInfo(
                                                                                                                                                                                                                                                    string organizationId,
            string productName, 
            byte[] iconBytes,
            ColorPickerModel titleForeground, 
            ColorPickerModel titleBackground, 
            ColorPickerModel titleButtonForeground);
        Task<Device> UpdateDevice(DeviceSetupOptions deviceOptions, string organizationId);
        void UpdateDevice(string deviceID, string tag, string alias, string deviceGroupID, string notes, WebRtcSetting webRtcSetting);
        void UpdateOrganizationName(string orgID, string organizationName);
        void UpdateTags(string deviceID, string tags);
        void UpdateUserOptions(string userName, RemotelyUserOptions options);
        bool ValidateApiKey(string keyId, string apiSecret, string requestPath, string remoteIP);
        void WriteEvent(EventLog eventLog);
        void WriteEvent(Exception ex, string organizationID);
        void WriteEvent(string message, EventType eventType, string organizationID);
        void WriteEvent(string message, string organizationID);
        void WriteLog(LogLevel logLevel, string category, EventId eventId, string state, Exception exception, string[] scopeStack);
    }

    public class DataService : IDataService
    {
        private readonly IApplicationConfig _appConfig;

        private readonly IDbContextFactory<AppDb> _dbFactory;
        private readonly IHostEnvironment _hostEnvironment;

        public DataService(IDbContextFactory<AppDb> dbFactory,
            IApplicationConfig appConfig,
            IHostEnvironment hostEnvironment)
        {
            _dbFactory = dbFactory;
            _appConfig = appConfig;
            _hostEnvironment = hostEnvironment;
        }

        public async Task AddAlert(string deviceId, string organizationID, string alertMessage, string details = null)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var users = dbContext.Users
               .Include(x => x.Alerts)
               .Where(x => x.OrganizationID == organizationID);

            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                var filteredUserIDs = FilterUsersByDevicePermissionInternal(
                    dbContext,
                    users.Select(x => x.Id),
                    deviceId);

                users = users.Where(x => filteredUserIDs.Contains(x.Id));
            }

            await users.ForEachAsync(x =>
            {
                var alert = new Alert()
                {
                    CreatedOn = DateTimeOffset.Now,
                    DeviceID = deviceId,
                    Message = alertMessage,
                    OrganizationID = organizationID,
                    Details = details
                };
                x.Alerts.Add(alert);
            });

            await dbContext.SaveChangesAsync();
        }

        public bool AddDeviceGroup(string orgID, DeviceGroup deviceGroup, out string deviceGroupID, out string errorMessage)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            deviceGroupID = null;
            errorMessage = null;

            var organization = dbContext.Organizations
                .Include(x => x.DeviceGroups)
                .FirstOrDefault(x => x.ID == orgID);

            if (dbContext.DeviceGroups.Any(x =>
                x.OrganizationID == orgID &&
                x.Name.ToLower() == deviceGroup.Name.ToLower()))
            {
                errorMessage = "Device group already exists.";
                return false;
            }

            dbContext.Attach(deviceGroup);
            deviceGroup.Organization = organization;
            deviceGroup.OrganizationID = orgID;

            organization.DeviceGroups.Add(deviceGroup);
            dbContext.SaveChanges();
            deviceGroupID = deviceGroup.ID;
            return true;
        }

        public InviteLink AddInvite(string orgID, InviteViewModel invite)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var organization = dbContext.Organizations
                .Include(x => x.InviteLinks)
                .FirstOrDefault(x => x.ID == orgID);

            var inviteLink = new InviteLink()
            {
                InvitedUser = invite.InvitedUser.ToLower(),
                DateSent = DateTimeOffset.Now,
                IsAdmin = invite.IsAdmin,
                Organization = organization,
                OrganizationID = organization.ID,
            };

            organization.InviteLinks.Add(inviteLink);
            dbContext.SaveChanges();
            return inviteLink;
        }

        public bool AddOrUpdateDevice(Device device, out Device updatedDevice)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var existingDevice = dbContext.Devices.Find(device.ID);
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
                if (_hostEnvironment.IsDevelopment() && dbContext.Organizations.Any())
                {
                    var org = dbContext.Organizations.FirstOrDefault();
                    device.Organization = org;
                    device.OrganizationID = org?.ID;
                }

                updatedDevice = device;

                if (!dbContext.Organizations.Any(x => x.ID == device.OrganizationID))
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
                dbContext.Devices.Add(device);
            }
            dbContext.SaveChanges();
            return true;
        }

        public async Task AddOrUpdateSavedScript(SavedScript script, string userId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            dbContext.SavedScripts.Update(script);
            script.CreatorId = userId;
            script.Creator = dbContext.Users.Find(userId);
            await dbContext.SaveChangesAsync();
        }

        public void AddOrUpdateScriptResult(ScriptResult result)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var device = dbContext.Devices.Find(result.DeviceID);

            if (device is null)
            {
                return;
            }

            result.OrganizationID = device.OrganizationID;

            var existingResult = dbContext.ScriptResults.Find(result.ID);
            if (existingResult is not null)
            {
                var entry = dbContext.Entry(existingResult);
                entry.CurrentValues.SetValues(result);
                entry.State = EntityState.Modified;
            }
            else
            {
                dbContext.ScriptResults.Add(result);
            }
            dbContext.SaveChanges();
        }

        public async Task AddOrUpdateScriptSchedule(ScriptSchedule schedule)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            if (schedule.Devices is not null)
            {
                dbContext.AttachRange(schedule.Devices);
            }

            if (schedule.DeviceGroups is not null)
            {
                dbContext.AttachRange(schedule.DeviceGroups);
            }
           
            if (schedule.Creator is not null)
            {
                dbContext.Attach(schedule.Creator);
            }

            dbContext.ScriptSchedules.Update(schedule);

            await dbContext.SaveChangesAsync();
        }

        public async Task AddScriptResultToScriptRun(string scriptResultId, int scriptRunId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var run = await dbContext.ScriptRuns
                .Include(x => x.Results)
                .Include(x => x.DevicesCompleted)
                .FirstOrDefaultAsync(x => x.Id == scriptRunId);

            var result = await dbContext.ScriptResults.FindAsync(scriptResultId);

            if (run is not null && result is not null)
            {
                run.Results.Add(result);

                var device = await dbContext.Devices
                    .Include(x => x.ScriptRunsCompleted)
                    .FirstOrDefaultAsync(x => x.ID == result.DeviceID);

                if (device is not null)
                {
                    run.DevicesCompleted.Add(device);
                    device.ScriptRunsCompleted.Add(run);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task AddScriptRun(ScriptRun scriptRun)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            dbContext.Attach(scriptRun);
            dbContext.ScriptRuns.Add(scriptRun);
            await dbContext.SaveChangesAsync();
        }

        public async Task<string> AddSharedFile(IBrowserFile file, string organizationID, Action<double, string> progressCallback)
        {
            var fileContents = new byte[file.Size];
            using var stream = file.OpenReadStream(AppConstants.MaxUploadFileSize);

            for (var i = 0; i < file.Size; i += 5_000)
            {
                var readSize = (int)Math.Min(5_000, file.Size - i);
                await stream.ReadAsync(fileContents.AsMemory(i, readSize));

                progressCallback.Invoke((double)stream.Position / stream.Length, file.Name);
            }

            return await AddSharedFileInternal(file.Name, fileContents, file.ContentType, organizationID);
        }

        public async Task<string> AddSharedFile(IFormFile file, string organizationID)
        {
            var fileContents = new byte[file.Length];
            using var stream = file.OpenReadStream();
            await stream.ReadAsync(fileContents.AsMemory(0, (int)file.Length));

            return await AddSharedFileInternal(file.Name, fileContents, file.ContentType, organizationID);
        }

        public bool AddUserToDeviceGroup(string orgID, string groupID, string userName, out string resultMessage)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            resultMessage = string.Empty;

            var deviceGroup = dbContext.DeviceGroups
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

            var user = dbContext.Users
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
            dbContext.SaveChanges();
            resultMessage = user.Id;
            return true;
        }

        public void ChangeUserIsAdmin(string organizationID, string targetUserID, bool isAdmin)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var targetUser = dbContext.Users.FirstOrDefault(x =>
                                x.OrganizationID == organizationID &&
                                x.Id == targetUserID);

            if (targetUser != null)
            {
                targetUser.IsAdministrator = isAdmin;
                dbContext.SaveChanges();
            }
        }

        public void CleanupOldRecords()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            if (_appConfig.DataRetentionInDays > -1)
            {

                var expirationDate = DateTimeOffset.Now - TimeSpan.FromDays(_appConfig.DataRetentionInDays);

                var scriptRuns = dbContext.ScriptRuns
                    .Include(x=>x.Results)
                    .Include(x=>x.Devices)
                    .Include(x=>x.DevicesCompleted)
                    .Where(x => x.RunAt < expirationDate);

                foreach (var run in scriptRuns)
                {
                    run.Devices?.Clear();
                    run.DevicesCompleted?.Clear();
                    run.Results?.Clear();
                }

                dbContext.RemoveRange(scriptRuns);

                var eventLogs = dbContext.EventLogs
                                    .Where(x => x.TimeStamp < expirationDate);

                dbContext.RemoveRange(eventLogs);

                var commandResults = dbContext.ScriptResults
                                        .Where(x => x.TimeStamp < expirationDate);

                dbContext.RemoveRange(commandResults);

                var sharedFiles = dbContext.SharedFiles
                                        .Where(x => x.Timestamp < expirationDate);

                dbContext.RemoveRange(sharedFiles);

                dbContext.SaveChanges();
            }
        }

        public async Task ClearLogs(string currentUserName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == currentUserName);
            if (currentUser is null)
            {
                return;
            }

            try
            {

                if (currentUser.IsServerAdmin)
                {
                    dbContext.EventLogs.RemoveRange(dbContext.EventLogs);
                    dbContext.ScriptResults.RemoveRange(dbContext.ScriptResults);
                }
                else
                {
                    var eventLogs = dbContext.EventLogs.Where(x => x.OrganizationID == currentUser.OrganizationID);
                    var commandResults = dbContext.ScriptResults.Where(x => x.OrganizationID == currentUser.OrganizationID);

                    dbContext.ScriptResults.RemoveRange(commandResults);
                    dbContext.EventLogs.RemoveRange(eventLogs);
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                WriteEvent(ex, currentUser.OrganizationID);
            }
        }

        public async Task<ApiToken> CreateApiToken(string userName, string tokenName, string secretHash)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var user = dbContext.Users.FirstOrDefault(x => x.UserName == userName);

            var newToken = new ApiToken()
            {
                Name = tokenName,
                OrganizationID = user.OrganizationID,
                Secret = secretHash
            };
            dbContext.ApiTokens.Add(newToken);
            await dbContext.SaveChangesAsync();
            return newToken;
        }

        public async Task<Device> CreateDevice(DeviceSetupOptions options)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            try
            {
                if (options is null ||
                    string.IsNullOrWhiteSpace(options.DeviceID) ||
                    string.IsNullOrWhiteSpace(options.OrganizationID) ||
                    dbContext.Devices.Any(x => x.ID == options.DeviceID))
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
                    var group = dbContext.DeviceGroups.FirstOrDefault(x =>
                        x.Name.ToLower() == options.DeviceGroupName.ToLower() &&
                        x.OrganizationID == device.OrganizationID);
                    device.DeviceGroup = group;
                }

                dbContext.Devices.Add(device);

                await dbContext.SaveChangesAsync();

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
            using var dbContext = _dbFactory.CreateDbContext();

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
                var org = dbContext.Organizations
                    .Include(x => x.RemotelyUsers)
                    .FirstOrDefault(x => x.ID == organizationID);
                dbContext.Users.Add(user);
                org.RemotelyUsers.Add(user);
                await dbContext.SaveChangesAsync();
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
            using var dbContext = _dbFactory.CreateDbContext();

            dbContext.Alerts.Remove(alert);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAllAlerts(string orgID, string userName = null)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var alerts = dbContext.Alerts.Where(x => x.OrganizationID == orgID);

            if (!string.IsNullOrWhiteSpace(userName))
            {
                var userId = GetUserByNameWithOrg(userName)?.Id;

                alerts = alerts.Where(x => x.UserID == userId);
            }

            dbContext.Alerts.RemoveRange(alerts);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteApiToken(string userName, string tokenId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var user = dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            var token = dbContext.ApiTokens.FirstOrDefault(x =>
                x.OrganizationID == user.OrganizationID &&
                x.ID == tokenId);

            dbContext.ApiTokens.Remove(token);
            await dbContext.SaveChangesAsync();
        }

        public void DeleteDeviceGroup(string orgID, string deviceGroupID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var deviceGroup = dbContext.DeviceGroups
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

            dbContext.DeviceGroups.Remove(deviceGroup);

            dbContext.SaveChanges();
        }

        public void DeleteInvite(string orgID, string inviteID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var invite = dbContext.InviteLinks.FirstOrDefault(x =>
                x.OrganizationID == orgID &&
                x.ID == inviteID);

            var user = dbContext.Users.FirstOrDefault(x => x.UserName == invite.InvitedUser);

            if (user != null && string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                dbContext.Remove(user);
            }
            dbContext.Remove(invite);
            dbContext.SaveChanges();
        }

        public async Task DeleteSavedScript(Guid scriptId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var script = dbContext.SavedScripts.Find(scriptId);
            if (script is not null)
            {
                dbContext.SavedScripts.Remove(script);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteScriptSchedule(int scriptScheduleId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var schedule = dbContext.ScriptSchedules.Find(scriptScheduleId);
            if (schedule is not null)
            {
                dbContext.ScriptSchedules.Remove(schedule);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteUser(string orgID, string targetUserID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var target = dbContext.Users
                .Include(x => x.DeviceGroups)
                .ThenInclude(x => x.Devices)
                .Include(x => x.Organization)
                .Include(x => x.Alerts)
                .Include(x => x.SavedScripts)
                .Include(x => x.ScriptSchedules)
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
                dbContext.Alerts.Remove(alert);
            }

            target.OrganizationID = null;
            target.Organization = null;

            dbContext
                .Organizations
                .Include(x => x.RemotelyUsers)
                .FirstOrDefault(x => x.ID == orgID)
                .RemotelyUsers.Remove(target);


            dbContext.Users.Remove(target);


            await dbContext.SaveChangesAsync();

        }

        public void DetachEntity(object entity)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            dbContext.Entry(entity).State = EntityState.Detached;
        }

        public void DeviceDisconnected(string deviceID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var device = dbContext.Devices.Find(deviceID);
            if (device != null)
            {
                device.LastOnline = DateTimeOffset.Now;
                device.IsOnline = false;
                dbContext.SaveChanges();
            }
        }

        public bool DoesUserExist(string userName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }
            return dbContext.Users.Any(x => x.UserName.Trim().ToLower() == userName.Trim().ToLower());
        }

        public bool DoesUserHaveAccessToDevice(string deviceID, RemotelyUser remotelyUser)
        {
            if (remotelyUser is null)
            {
                return false;
            }

            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Devices
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
            using var dbContext = _dbFactory.CreateDbContext();

            var remotelyUser = dbContext.Users.Find(remotelyUserID);

            return DoesUserHaveAccessToDevice(deviceID, remotelyUser);
        }

        public string[] FilterDeviceIDsByUserPermission(string[] deviceIDs, RemotelyUser remotelyUser)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Devices
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
            using var dbContext = _dbFactory.CreateDbContext();

            return FilterUsersByDevicePermissionInternal(dbContext, userIDs, deviceID);
        }

        public async Task<Alert> GetAlert(string alertID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return await dbContext.Alerts
                .Include(x => x.Device)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ID == alertID);
        }

        public Alert[] GetAlerts(string userID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Alerts
                .Include(x => x.Device)
                .Include(x => x.User)
                .Where(x => x.UserID == userID)
                .OrderByDescending(x => x.CreatedOn)
                .ToArray();
        }

        public ApiToken[] GetAllApiTokens(string userID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var user = dbContext.Users.FirstOrDefault(x => x.Id == userID);

            return dbContext.ApiTokens
                .Where(x => x.OrganizationID == user.OrganizationID)
                .OrderByDescending(x => x.LastUsed)
                .ToArray();
        }

        public ScriptResult[] GetAllCommandResults(string orgID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.ScriptResults
                .Where(x => x.OrganizationID == orgID)
                .OrderByDescending(x => x.TimeStamp)
                .ToArray();
        }

        public ScriptResult[] GetAllCommandResultsForUser(string orgId, string userName, string deviceId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.ScriptResults
                .Where(x => x.OrganizationID == orgId &&
                    x.SenderUserName == userName &&
                    x.DeviceID == deviceId)
                .OrderByDescending(x => x.TimeStamp)
                .ToArray();
        }

        public Device[] GetAllDevices(string orgID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Devices.Where(x => x.OrganizationID == orgID).ToArray();
        }

        public EventLog[] GetAllEventLogs(string orgID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.EventLogs
                .Where(x => x.OrganizationID == orgID)
                .OrderByDescending(x => x.TimeStamp)
                .ToArray();
        }

        public InviteLink[] GetAllInviteLinks(string userName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Users
                   .Include(x => x.Organization)
                   .ThenInclude(x => x.InviteLinks)
                   .FirstOrDefault(x => x.UserName == userName)
                   .Organization
                   .InviteLinks.ToArray() ?? Array.Empty<InviteLink>();
        }

        public ScriptResult[] GetAllScriptResults(string orgId, string deviceId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.ScriptResults
                .Where(x => x.OrganizationID == orgId && x.DeviceID == deviceId)
                .OrderByDescending(x => x.TimeStamp)
                .ToArray();
        }

        public ScriptResult[] GetAllScriptResultsForUser(string orgId, string userName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.ScriptResults
                .Where(x => x.OrganizationID == orgId && x.SenderUserName == userName)
                .OrderByDescending(x => x.TimeStamp)
                .ToArray();
        }

        public RemotelyUser[] GetAllUsersForServer()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Users.ToArray();
        }

        public async Task<RemotelyUser[]> GetAllUsersInOrganization(string orgId)
        {
            if (string.IsNullOrWhiteSpace(orgId))
            {
                return Array.Empty<RemotelyUser>();
            }

            using var dbContext = _dbFactory.CreateDbContext();

            var organization = await dbContext.Organizations
                .Include(x => x.RemotelyUsers)
                .FirstOrDefaultAsync(x => x.ID == orgId);

            return organization.RemotelyUsers.ToArray();
        }

        public ApiToken GetApiKey(string keyId)
        {
            if (string.IsNullOrWhiteSpace(keyId))
            {
                return null;
            }

            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.ApiTokens.FirstOrDefault(x => x.ID == keyId);
        }

        public async Task<BrandingInfo> GetBrandingInfo(string organizationId)
        {
            if (string.IsNullOrWhiteSpace(organizationId))
            {
                return null;
            }

            using var dbContext = _dbFactory.CreateDbContext();

            var organization = await dbContext.Organizations
              .Include(x => x.BrandingInfo)
              .FirstOrDefaultAsync(x => x.ID == organizationId);

            if (organization is null)
            {
                return null;
            }

            if (organization.BrandingInfo is null)
            {
                organization.BrandingInfo = new BrandingInfo();
                await dbContext.SaveChangesAsync();
            }
            return organization.BrandingInfo;
        }

        public async Task<Organization> GetDefaultOrganization()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return await dbContext.Organizations.FirstOrDefaultAsync(x => x.IsDefaultOrganization);
        }

        public async Task<string> GetDefaultRelayCode()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var relayCode = await dbContext.Organizations
                .Where(x => x.IsDefaultOrganization)
                .Select(x => x.RelayCode)
                .FirstOrDefaultAsync();

            return relayCode;
        }

        public Device GetDevice(string orgID, string deviceID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Devices.FirstOrDefault(x =>
                            x.OrganizationID == orgID &&
                            x.ID == deviceID);
        }

        public Device GetDevice(string deviceID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Devices.FirstOrDefault(x => x.ID == deviceID);
        }

        public int GetDeviceCount()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Devices.Count();
        }

        public int GetDeviceCount(RemotelyUser user)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.Users)
                .Count(x =>
                    x.OrganizationID == user.OrganizationID &&
                    (
                        user.IsAdministrator ||
                        string.IsNullOrWhiteSpace(x.DeviceGroupID) ||
                        !x.DeviceGroup.Users.Any() ||
                        x.DeviceGroup.Users.Any(deviceUser => deviceUser.Id == user.Id)
                    ));
        }

        public DeviceGroup[] GetDeviceGroups(string username)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var user = dbContext.Users.FirstOrDefault(x => x.UserName == username);

            if (user is null)
            {
                return null;
            }
            var userId = user.Id;

            return dbContext.DeviceGroups
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
                .OrderBy(x => x.Name).ToArray() ?? Array.Empty<DeviceGroup>();
        }

        public DeviceGroup[] GetDeviceGroupsForOrganization(string organizationId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.DeviceGroups
                .Include(x => x.Users)
                .ThenInclude(x => x.DeviceGroups)
                .Where(x => x.OrganizationID == organizationId)
                .OrderBy(x => x.Name)
                .ToArray();
        }

        public List<Device> GetDevices(IEnumerable<string> deviceIds)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Devices
                .Where(x => deviceIds.Contains(x.ID))
                .ToList();

        }

        public Device[] GetDevicesForUser(string userName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            if (string.IsNullOrWhiteSpace(userName))
            {
                return Array.Empty<Device>();
            }

            var user = dbContext.Users.FirstOrDefault(x => x.UserName == userName);

            if (user is null)
            {
                return Array.Empty<Device>();
            }

            return dbContext.Devices
                .Include(x => x.DeviceGroup)
                .ThenInclude(x => x.Users)
                .Where(x =>
                    x.OrganizationID == user.OrganizationID &&
                    (
                        user.IsAdministrator ||
                        string.IsNullOrWhiteSpace(x.DeviceGroupID) ||
                        !x.DeviceGroup.Users.Any() ||
                        x.DeviceGroup.Users.Any(deviceUser => deviceUser.Id == user.Id)
                    ))
                .ToArray();
        }

        public EventLog[] GetEventLogs(string userName, DateTimeOffset from, DateTimeOffset to, EventType? type, string message)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var user = dbContext.Users
                        .FirstOrDefault(x => x.UserName == userName);

            var query = dbContext.EventLogs.AsQueryable();
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
            return query.ToArray();
        }

        public Organization GetOrganizationById(string organizationID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Organizations.Find(organizationID);
        }

        public async Task<Organization> GetOrganizationByRelayCode(string relayCode)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            if (string.IsNullOrWhiteSpace(relayCode))
            {
                return null;
            }

            return await dbContext.Organizations.FirstOrDefaultAsync(x => x.RelayCode == relayCode.ToLower());
        }

        public async Task<Organization> GetOrganizationByUserName(string userName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var user = await dbContext
                .Users
                .Include(x => x.Organization)
                .FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());

            return user.Organization;
        }

        public int GetOrganizationCount()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Organizations.Count();
        }

        public string GetOrganizationNameById(string organizationID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Organizations.FirstOrDefault(x => x.ID == organizationID)?.OrganizationName;
        }

        public string GetOrganizationNameByUserName(string userName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Users
                   .Include(x => x.Organization)
                   .FirstOrDefault(x => x.UserName == userName)
                   .Organization
                   .OrganizationName;
        }

        public async Task<List<ScriptRun>> GetPendingScriptRuns(string deviceId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var pendingRuns = new List<ScriptRun>();

            var now = Time.Now;
            var device = await dbContext.Devices.FindAsync(deviceId);

            var scriptRunGroups = dbContext.ScriptRuns
                .Include(x => x.Devices)
                .Include(x => x.DevicesCompleted)
                .Where(scriptRun =>
                    scriptRun.RunOnNextConnect &&
                    dbContext.SavedScripts.Any(savedScript => savedScript.Id == scriptRun.SavedScriptId) &&
                    scriptRun.Devices.Any(device => device.ID == deviceId) &&
                    !scriptRun.DevicesCompleted.Any(deviceCompleted => deviceCompleted.ID == deviceId) &&
                    scriptRun.RunAt < now)
                .AsEnumerable()
                .GroupBy(x => x.SavedScriptId);

            foreach (var group in scriptRunGroups)
            {
                var latestRun = group
                    .OrderByDescending(x => x.RunAt)
                    .FirstOrDefault();

                pendingRuns.Add(latestRun);
            }

            await dbContext.SaveChangesAsync();


            return pendingRuns;
        }

        public async Task<List<SavedScript>> GetQuickScripts(string userId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return await dbContext.SavedScripts
                .Where(x => x.CreatorId == userId && x.IsQuickScript)
                .ToListAsync();
        }

        public async Task<SavedScript> GetSavedScript(string userId, Guid scriptId)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            
            return await dbContext.SavedScripts
                .Include(x => x.Creator)
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == scriptId &&
                    (x.IsPublic || x.CreatorId == userId));
        }

        public async Task<SavedScript> GetSavedScript(Guid scriptId)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            return await dbContext.SavedScripts.FirstOrDefaultAsync(x => x.Id == scriptId);
        }

        public async Task<List<SavedScript>> GetSavedScriptsWithoutContent(string userId, string organizationId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var query = dbContext.SavedScripts
                    .Include(x => x.Creator)
                    .Where(x => x.Creator.OrganizationID == organizationId &&
                        (x.IsPublic || x.CreatorId == userId));

            return await query.Select(x => new SavedScript()
            {
                Creator = x.Creator,
                CreatorId = x.CreatorId,
                FolderPath = x.FolderPath,
                Id = x.Id,
                IsPublic = x.IsPublic,
                IsQuickScript = x.IsQuickScript,
                Name = x.Name,
                OrganizationID = x.OrganizationID
            }).ToListAsync();
        }

        public ScriptResult GetScriptResult(string commandResultID, string orgID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.ScriptResults
                .FirstOrDefault(x =>
                    x.OrganizationID == orgID &&
                    x.ID == commandResultID);
        }

        public ScriptResult GetScriptResult(string commandResultID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.ScriptResults.Find(commandResultID);
        }

        public async Task<List<ScriptSchedule>> GetScriptSchedules(string organizationId)
        {
            using var dbContext = _dbFactory.CreateDbContext();
            return await dbContext.ScriptSchedules
                .Include(x => x.Creator)
                .Include(x => x.Devices)
                .Include(x => x.DeviceGroups)
                .Where(x => x.OrganizationID == organizationId)
                .ToListAsync();
        }

        public async Task<List<ScriptSchedule>> GetScriptSchedulesDue()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var now = Time.Now;

            return await dbContext.ScriptSchedules
                .Include(x => x.Devices)
                .Include(x => x.DeviceGroups)
                .ThenInclude(x => x.Devices)
                .Where(x => x.NextRun < now)
                .ToListAsync();
        }
        public List<string> GetServerAdmins()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Users
                .Where(x => x.IsServerAdmin)
                .Select(x => x.UserName)
                .ToList();
        }

        public SharedFile GetSharedFiled(string fileID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.SharedFiles.Find(fileID);
        }

        public int GetTotalDevices()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Devices.Count();
        }

        public async Task<RemotelyUser> GetUserAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            using var dbContext = _dbFactory.CreateDbContext();

            return await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public RemotelyUser GetUserByID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
            {
                return null;
            }
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Users.FirstOrDefault(x => x.Id == userID);
        }

        public RemotelyUser GetUserByNameWithOrg(string userName)
        {
            if (userName == null)
            {
                return null;
            }

            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Users
                .Include(x => x.Organization)
                .FirstOrDefault(x => x.UserName.ToLower().Trim() == userName.ToLower().Trim());
        }

        public RemotelyUserOptions GetUserOptions(string userName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Users
                    .FirstOrDefault(x => x.UserName == userName)
                    .UserOptions;
        }

        public bool JoinViaInvitation(string userName, string inviteID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var invite = dbContext.InviteLinks.FirstOrDefault(x =>
                            x.InvitedUser.ToLower() == userName.ToLower() &&
                            x.ID == inviteID);

            if (invite == null)
            {
                return false;
            }

            var user = dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            var organization = dbContext.Organizations
                                .Include(x => x.RemotelyUsers)
                                .FirstOrDefault(x => x.ID == invite.OrganizationID);

            user.Organization = organization;
            user.OrganizationID = organization.ID;
            user.IsAdministrator = invite.IsAdmin;
            organization.RemotelyUsers.Add(user);

            dbContext.SaveChanges();

            dbContext.InviteLinks.Remove(invite);
            dbContext.SaveChanges();
            return true;
        }

        public void PopulateRelayCodes()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            foreach (var organization in dbContext.Organizations)
            {
                if (string.IsNullOrWhiteSpace(organization.RelayCode))
                {
                    do
                    {
                        organization.RelayCode = new string(Guid.NewGuid().ToString().Take(4).ToArray());
                    }
                    while (dbContext.Organizations.Any(x => x.ID != organization.ID && x.RelayCode == organization.RelayCode));
                }
            }
            dbContext.SaveChanges();
        }

        public void RemoveDevices(string[] deviceIDs)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var devices = dbContext.Devices
                .Include(x => x.ScriptResults)
                .Include(x => x.ScriptRuns)
                .Include(x => x.ScriptSchedules)
                .Include(x => x.ScriptRunsCompleted)
                .Include(x => x.DeviceGroup)
                .Include(x => x.Alerts)
                .Where(x => deviceIDs.Contains(x.ID));

            dbContext.Devices.RemoveRange(devices);
            dbContext.SaveChanges();
        }

        public async Task<bool> RemoveUserFromDeviceGroup(string orgID, string groupID, string userID)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var deviceGroup = dbContext.DeviceGroups
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

                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task RenameApiToken(string userName, string tokenId, string tokenName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var user = dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            var token = dbContext.ApiTokens.FirstOrDefault(x =>
                x.OrganizationID == user.OrganizationID &&
                x.ID == tokenId);

            token.Name = tokenName;
            await dbContext.SaveChangesAsync();
        }

        public void SetAllDevicesNotOnline()
        {
            using var dbContext = _dbFactory.CreateDbContext();

            dbContext.Devices.ForEachAsync(x =>
            {
                x.IsOnline = false;
            }).Wait();
            dbContext.SaveChanges();
        }

        public async Task SetDisplayName(RemotelyUser user, string displayName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            dbContext.Attach(user);
            user.UserOptions.DisplayName = displayName;
            await dbContext.SaveChangesAsync();
        }

        public async Task SetIsDefaultOrganization(string orgID, bool isDefault)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var organization = await dbContext.Organizations.FindAsync(orgID);
            if (organization is null)
            {
                return;
            }

            if (isDefault)
            {
                await dbContext.Organizations.ForEachAsync(x => x.IsDefaultOrganization = false);
            }

            organization.IsDefaultOrganization = isDefault;
            await dbContext.SaveChangesAsync();
        }

        public async Task SetIsServerAdmin(string targetUserId, bool isServerAdmin, string callerUserId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var caller = await dbContext.Users.FindAsync(callerUserId);
            if (caller?.IsServerAdmin != true)
            {
                return;
            }

            var targetUser = await dbContext.Users.FindAsync(targetUserId);

            if (targetUser is null)
            {
                return;
            }

            if (caller.Id == targetUser.Id)
            {
                // A server admin can't change themselves.
                return;
            }

            targetUser.IsServerAdmin = isServerAdmin;
            await dbContext.SaveChangesAsync();
        }

        public void SetServerVerificationToken(string deviceID, string verificationToken)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var device = dbContext.Devices.Find(deviceID);
            if (device != null)
            {
                device.ServerVerificationToken = verificationToken;
                dbContext.SaveChanges();
            }
        }

        public async Task<bool> TempPasswordSignIn(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var user = GetUserByNameWithOrg(email);

            if (user is null)
            {
                return false;
            }
            using var dbContext = _dbFactory.CreateDbContext();

            if (user.TempPassword == password)
            {
                user.TempPassword = string.Empty;
                await dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task UpdateBrandingInfo(
            string organizationId,
            string productName,
            byte[] iconBytes,
            ColorPickerModel titleForeground,
            ColorPickerModel titleBackground,
            ColorPickerModel titleButtonForeground)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var organization = await dbContext.Organizations
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

            if (iconBytes?.Any() == true)
            {
                organization.BrandingInfo.Icon = iconBytes;
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

            await dbContext.SaveChangesAsync();
        }

        public void UpdateDevice(string deviceID, string tag, string alias, string deviceGroupID, string notes, WebRtcSetting webRtcSetting)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var device = dbContext.Devices.Find(deviceID);
            if (device == null)
            {
                return;
            }

            device.Tags = tag;
            device.DeviceGroupID = deviceGroupID;
            device.Alias = alias;
            device.Notes = notes;
            device.WebRtcSetting = webRtcSetting;
            dbContext.SaveChanges();
        }

        public async Task<Device> UpdateDevice(DeviceSetupOptions deviceOptions, string organizationId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var device = dbContext.Devices.Find(deviceOptions.DeviceID);
            if (device == null || device.OrganizationID != organizationId)
            {
                return null;
            }

            var group = await dbContext.DeviceGroups.FirstOrDefaultAsync(x =>
              x.Name.ToLower() == deviceOptions.DeviceGroupName.ToLower() &&
              x.OrganizationID == device.OrganizationID);
            device.DeviceGroup = group;

            device.Alias = deviceOptions.DeviceAlias;
            await dbContext.SaveChangesAsync();
            return device;
        }

        public void UpdateOrganizationName(string orgID, string organizationName)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            dbContext.Organizations
                .FirstOrDefault(x => x.ID == orgID)
                .OrganizationName = organizationName;
            dbContext.SaveChanges();
        }

        public void UpdateTags(string deviceID, string tags)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var device = dbContext.Devices.Find(deviceID);
            if (device == null)
            {
                return;
            }

            device.Tags = tags;
            dbContext.SaveChanges();
        }

        public void UpdateUserOptions(string userName, RemotelyUserOptions options)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            dbContext.Users.FirstOrDefault(x => x.UserName == userName).UserOptions = options;
            dbContext.SaveChanges();
        }

        public bool ValidateApiKey(string keyId, string apiSecret, string requestPath, string remoteIP)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var hasher = new PasswordHasher<RemotelyUser>();
            var token = dbContext.ApiTokens.FirstOrDefault(x => x.ID == keyId);


            var isValid = token is not null &&
                hasher.VerifyHashedPassword(null, token.Secret, apiSecret) == PasswordVerificationResult.Success;


            if (token is not null)
            {
                token.LastUsed = DateTimeOffset.Now;
                dbContext.SaveChanges();
            }

            WriteEvent($"API token used.  Token: {keyId}.  Path: {requestPath}.  Validated: {isValid}.  Remote IP: {remoteIP}", EventType.Info, token?.OrganizationID);

            return isValid;
        }

        public void WriteEvent(EventLog eventLog)
        {
            try
            {
                using var dbContext = _dbFactory.CreateDbContext();

                dbContext.EventLogs.Add(eventLog);
                dbContext.SaveChanges();
            }
            catch { }
        }

        public void WriteEvent(Exception ex, string organizationID)
        {
            try
            {
                using var dbContext = _dbFactory.CreateDbContext();

                dbContext.EventLogs.Add(new EventLog()
                {
                    EventType = EventType.Error,
                    Message = ex.Message,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    TimeStamp = DateTimeOffset.Now,
                    OrganizationID = organizationID
                });
                dbContext.SaveChanges();
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
                using var dbContext = _dbFactory.CreateDbContext();

                dbContext.EventLogs.Add(new EventLog()
                {
                    EventType = eventType,
                    Message = message,
                    TimeStamp = DateTimeOffset.Now,
                    OrganizationID = organizationID
                });
                dbContext.SaveChanges();
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
                using var dbContext = _dbFactory.CreateDbContext();

                EventType eventType = EventType.Debug;
                switch (logLevel)
                {
                    case LogLevel.None:
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                        eventType = EventType.Debug;
                        break;
                    case LogLevel.Information:
                        eventType = EventType.Info;
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

                dbContext.EventLogs.Add(new EventLog()
                {
                    StackTrace = exception?.StackTrace,
                    EventType = eventType,
                    Message = $"[{logLevel}] [{string.Join(" - ", scopeStack)} - {category}] | Message: {state} | Exception: {exception?.Message}",
                    TimeStamp = DateTimeOffset.Now
                });

                dbContext.SaveChanges();
            }
            catch { }

        }

        private async Task<string> AddSharedFileInternal(
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            string fileName,
            byte[] fileContents,
            string contentType,
            string organizationId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var expirationDate = DateTimeOffset.Now.AddDays(-_appConfig.DataRetentionInDays);
            var expiredFiles = dbContext.SharedFiles.Where(x => x.Timestamp < expirationDate);
            dbContext.RemoveRange(expiredFiles);

            var sharedFile = new SharedFile()
            {
                FileContents = fileContents,
                FileName = fileName,
                ContentType = contentType,
                OrganizationID = organizationId
            };

            dbContext.SharedFiles.Add(sharedFile);

            await dbContext.SaveChangesAsync();
            return sharedFile.ID;
        }
        private string[] FilterUsersByDevicePermissionInternal(AppDb dbContext, IEnumerable<string> userIDs, string deviceID)
        {
            var device = dbContext.Devices
                 .Include(x => x.DeviceGroup)
                 .ThenInclude(x => x.Users)
                 .FirstOrDefault(x => x.ID == deviceID);

            var orgUsers = dbContext.Users
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
    }
}
