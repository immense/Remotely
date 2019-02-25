using Remotely_Library.Models;
using Remotely_Library.ViewModels;
using Remotely_Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Remotely_Server.Data
{
    public class DataService
    {
        public DataService(
                ApplicationDbContext context,
                ApplicationConfig appConfig,
                UserManager<RemotelyUser> userManager
            )
        {
            RemotelyContext = context;
            AppConfig = appConfig;
            UserManager = userManager;
        }

        private ApplicationConfig AppConfig { get; set; }

        private ApplicationDbContext RemotelyContext { get; set; }

        private UserManager<RemotelyUser> UserManager { get; set; }

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

        public bool AddOrUpdateMachine(Machine machine)
        {
            var existingMachine = RemotelyContext.Machines.Find(machine.ID);
            if (existingMachine != null)
            {
                machine.ServerVerificationToken = existingMachine.ServerVerificationToken;
                RemotelyContext.Entry(existingMachine).CurrentValues.SetValues(machine);
            }
            else
            {
                if (!RemotelyContext.Organizations.Any(x => x.ID == machine.OrganizationID))
                {
                    WriteEvent(new EventLog()
                    {
                        EventType = EventTypes.Info,
                        Message = $"Unable to add machine {machine.MachineName} because organization {machine.OrganizationID} does not exist.",
                        Source = "DataService.AddOrUpdateMachine"
                    });
                    return false;
                }
                RemotelyContext.Machines.Add(machine);
            }
            RemotelyContext.SaveChanges();
            return true;
        }

        public void CleanupEmptyOrganizations()
        {
            var emptyOrgs = RemotelyContext.Organizations
                .Include(x => x.RemotelyUsers)
                .Include(x => x.CommandContexts)
                .Include(x => x.InviteLinks)
                .Include(x => x.Machines)
                .Include(x => x.SharedFiles)
                .Include(x => x.PermissionGroups)
                .Where(x => x.RemotelyUsers.Count == 0);

            foreach (var emptyOrg in emptyOrgs)
            {
                RemotelyContext.Remove(emptyOrg);
            }
            RemotelyContext.SaveChanges();
        }

        public bool DoesUserExist(string userName)
        {
            if (userName == null)
            {
                return false;
            }
            return RemotelyContext.Users.Any(x => x.UserName == userName);
        }

        public bool DoesUserHaveAccessToMachine(string machineID, RemotelyUser remotelyUser)
        {
            return RemotelyContext.Machines.Any(x =>
                x.OrganizationID == remotelyUser.OrganizationID &&
                    (
                        x.PermissionGroups.Count == 0 ||
                        x.PermissionGroups.Any(y => remotelyUser.PermissionGroups.Any(z => z.ID == y.ID))
                    ) &&
                x.ID == machineID);
        }

        public string[] FilterMachineIDsByUserPermission(string[] machineIDs, RemotelyUser remotelyUser)
        {
            return RemotelyContext.Machines.Where(x =>
                    x.OrganizationID == remotelyUser.OrganizationID &&
                    (
                        x.PermissionGroups.Count == 0 ||
                        x.PermissionGroups.Any(y => remotelyUser.PermissionGroups.Any(z => z.ID == y.ID))
                    ) &&
                    machineIDs.Contains(x.ID))
                .Select(x => x.ID)
                .ToArray();
        }

        public IEnumerable<CommandContext> GetAllCommandContexts(string userName)
        {
            var orgID = GetUserByName(userName).OrganizationID;
            return RemotelyContext.CommandContexts.Where(x => x.OrganizationID == orgID);
        }

        public IEnumerable<Machine> GetAllMachines(string userID)
        {
            var orgID = GetUserByID(userID).OrganizationID;

            var machines = RemotelyContext.Machines
                .Include(x => x.Drives)
                .Where(x => x.OrganizationID == orgID)
                .Select(x=> new Machine()
                {
                    CurrentUser =  x.CurrentUser,
                    Drives = x.Drives,
                    FreeMemory = x.FreeMemory,
                    FreeStorage = x.FreeStorage,
                    ID = x.ID,
                    Is64Bit = x.Is64Bit,
                    IsOnline = x.IsOnline,
                    LastOnline = x.LastOnline,
                    MachineName = x.MachineName,
                    OrganizationID = x.OrganizationID,
                    OSArchitecture = x.OSArchitecture,
                    OSDescription = x.OSDescription,
                    PermissionGroups = x.PermissionGroups,
                    Platform = x.Platform,
                    ProcessorCount = x.ProcessorCount,
                    Tags = x.Tags,
                    TotalMemory = x.TotalMemory,
                    TotalStorage = x.TotalStorage
                });

            return machines;
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
            var orgID = GetUserByName(userName).OrganizationID;
            return RemotelyContext.CommandContexts
                .FirstOrDefault(x => x.OrganizationID == orgID && x.ID == commandContextID);
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

        public Machine GetMachine(string userID, string machineID)
        {
            var orgID = GetUserByID(userID).OrganizationID;

            return RemotelyContext.Machines
                .Where(x => x.OrganizationID == orgID && x.ID == machineID)
                .Select(x => new Machine()
                {
                    CurrentUser = x.CurrentUser,
                    Drives = x.Drives,
                    FreeMemory = x.FreeMemory,
                    FreeStorage = x.FreeStorage,
                    ID = x.ID,
                    Is64Bit = x.Is64Bit,
                    IsOnline = x.IsOnline,
                    LastOnline = x.LastOnline,
                    MachineName = x.MachineName,
                    OrganizationID = x.OrganizationID,
                    OSArchitecture = x.OSArchitecture,
                    OSDescription = x.OSDescription,
                    PermissionGroups = x.PermissionGroups,
                    Platform = x.Platform,
                    ProcessorCount = x.ProcessorCount,
                    Tags = x.Tags,
                    TotalMemory = x.TotalMemory,
                    TotalStorage = x.TotalStorage
                }).FirstOrDefault();
        }

        public RemotelyUser GetUserByID(string userID)
        {
            if (userID == null)
            {
                return null;
            }
            return RemotelyContext.Users
                .Include(x => x.Organization)
                .FirstOrDefault(x => x.Id == userID);
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

        public void MachineDisconnected(string machineID)
        {
            var machine = RemotelyContext.Machines.Find(machineID);
            if (machine != null)
            {
                machine.LastOnline = DateTime.Now;
                machine.IsOnline = false;
                RemotelyContext.SaveChanges();
            }
        }

        public void RemoveMachines(string[] machineIDs)
        {
            var machines = RemotelyContext.Machines
                .Include(x => x.Drives)
                .Where(x => machineIDs.Contains(x.ID));
            foreach (var machine in machines)
            {

                if (machine?.Drives?.Count > 0)
                {
                    RemotelyContext.Drives.RemoveRange(machine.Drives);
                }
                RemotelyContext.Machines.Remove(machine);
            }
            RemotelyContext.SaveChanges();
        }

        public void SetAllMachinesNotOnline()
        {
            RemotelyContext.Machines.ForEachAsync(x =>
            {
                x.IsOnline = false;
            });
            RemotelyContext.SaveChanges();
        }

        public void WriteEvent(EventLog eventLog)
        {
            RemotelyContext.EventLogs.Add(eventLog);
            RemotelyContext.SaveChanges();
        }

        public void WriteEvent(Exception ex)
        {
            var error = ex;
            while (error != null)
            {
                RemotelyContext.EventLogs.Add(new EventLog()
                {
                    EventType = EventTypes.Error,
                    Message = error.Message,
                    Source = error.Source,
                    StackTrace = error.StackTrace,
                    TimeStamp = DateTime.Now
                });
                error = ex.InnerException;
            }
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
                Organization = requester.Organization
            };
            requester.Organization.InviteLinks.Add(newInvite);
            RemotelyContext.SaveChanges();
            return newInvite;
        }

        internal Tuple<bool, string> AddPermission(string userName, Permission permission)
        {
            var organization = RemotelyContext.Users
                .Include(x => x.Organization)
                .ThenInclude(x => x.PermissionGroups)
                .FirstOrDefault(x => x.UserName == userName)
                .Organization;
            if (organization.PermissionGroups.Exists(x => x.Name.ToLower() == permission.Name.ToLower()))
            {
                return Tuple.Create(false, "Permission group already exists.");
            }
            var newPermission = new PermissionGroup()
            {
                Name = permission.Name,
                Organization = organization
            };
            organization.PermissionGroups.Add(newPermission);
            RemotelyContext.SaveChanges();
            return Tuple.Create(true, newPermission.ID);
        }

        internal void AddPermissionToMachines(string userID, string[] machineIDs, string groupName)
        {
            var user = RemotelyContext.Users
                       .Include(x => x.Organization)
                       .Include(x => x.Organization)
                       .ThenInclude(x => x.Machines)
                       .FirstOrDefault(x => x.Id == userID);

            var group = user.Organization.PermissionGroups.FirstOrDefault(x => x.Name.ToLower() == groupName.ToLower());
            foreach (var machineID in machineIDs)
            {
                if (user.Organization.Machines.Exists(x => x.ID == machineID))
                {
                    var machine = RemotelyContext.Machines
                                .Include(x => x.PermissionGroups)
                                .FirstOrDefault(x => x.ID == machineID);
                    if (!machine.PermissionGroups.Exists(x => x.ID == group.ID))
                    {
                        machine.PermissionGroups.Add(group);
                        RemotelyContext.Entry(machine).State = EntityState.Modified;
                    }
                }
            }
            RemotelyContext.SaveChanges();
        }

        internal Tuple<bool, string> AddPermissionToUser(string requesterUserName, string targetUserID, string permissionID)
        {
            var requester = RemotelyContext.Users
                .Include(x => x.Organization)
                .FirstOrDefault(x => x.UserName == requesterUserName);

            var user = RemotelyContext.Users
                        .FirstOrDefault(x => x.OrganizationID == requester.OrganizationID && x.Id == targetUserID);

            if (user.PermissionGroups.Exists(x => x.ID == permissionID))
            {
                return Tuple.Create(false, "User is already in the permission group.");
            }

            var permissions = RemotelyContext.PermissionGroups
                .Include(x => x.Organization)
                .Where(x => x.Organization.ID == requester.Organization.ID);
            user.PermissionGroups.Add(permissions.FirstOrDefault(x => x.ID == permissionID));
            RemotelyContext.Entry(user).State = EntityState.Modified;
            RemotelyContext.SaveChanges();
            return Tuple.Create(true, "");
        }

        internal string AddSharedFile(IFormFile file)
        {
            var expiredFiles = RemotelyContext.SharedFiles.Where(x => DateTime.Now - x.Timestamp > TimeSpan.FromDays(7));
            foreach (var expiredFile in expiredFiles)
            {
                RemotelyContext.Remove(expiredFile);
            }
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
                ContentType = file.ContentType
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

            requester.Organization.RemotelyUsers.Find(x => x.Id == targetUserID).IsAdministrator = isAdmin;
            RemotelyContext.SaveChanges();
        }

        internal void CleanupOldRecords()
        {
            if (AppConfig.DataRetentionInDays > 0)
            {
                RemotelyContext.EventLogs
                    .Where(x => DateTime.Now - x.TimeStamp > TimeSpan.FromDays(AppConfig.DataRetentionInDays))
                    .ForEachAsync(x =>
                    {
                        RemotelyContext.Remove(x);
                    });

                RemotelyContext.CommandContexts
                    .Where(x => DateTime.Now - x.TimeStamp > TimeSpan.FromDays(AppConfig.DataRetentionInDays))
                    .ForEachAsync(x =>
                    {
                        RemotelyContext.Remove(x);
                    });

                RemotelyContext.Machines
                    .Where(x => DateTime.Now - x.LastOnline > TimeSpan.FromDays(AppConfig.DataRetentionInDays))
                    .ForEachAsync(x =>
                    {
                        RemotelyContext.Remove(x);
                    });
            }
        }

        internal void DeleteInvite(string requesterUserName, string inviteID)
        {
            var requester = RemotelyContext.Users
               .Include(x => x.Organization)
               .ThenInclude(x => x.InviteLinks)
               .FirstOrDefault(x => x.UserName == requesterUserName);
            var invite = requester.Organization.InviteLinks.Find(x => x.ID == inviteID);
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
            return user.Organization.PermissionGroups.Exists(x => x.Name.ToLower() == groupName.ToLower());
        }

        internal List<InviteLink> GetAllInviteLinks(string userName)
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

        internal SharedFile GetSharedFiled(string id)
        {
            return RemotelyContext.SharedFiles.Find(id);
        }

        internal RemotelyUserOptions GetUserOptions(string userName)
        {
            return RemotelyContext.Users
                    .FirstOrDefault(x => x.UserName == userName)
                    .UserOptions;
        }

        internal IEnumerable<PermissionGroup> GetUserPermissions(string requesterUserName, string targetID)
        {
            var targetUser = RemotelyContext.Users
                    .Include(x => x.Organization)
                    .ThenInclude(x => x.RemotelyUsers)
                    .FirstOrDefault(x => x.UserName == requesterUserName)
                    .Organization
                    .RemotelyUsers
                    .FirstOrDefault(x => x.Id == targetID);

            return targetUser.PermissionGroups;
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
                .FirstOrDefault(x => x.UserName == userName);

            user.Organization = invite.Organization;
            user.OrganizationID = invite.Organization.ID;
            user.IsAdministrator = invite.IsAdmin;
            user.PermissionGroups.Clear();
            invite.Organization.RemotelyUsers.Add(user);

            RemotelyContext.SaveChanges();

            RemotelyContext.InviteLinks.Remove(invite);
            RemotelyContext.SaveChanges();
            return true;
        }
        internal void RemoveFromOrganization(string requesterUserName, string targetUserID)
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

        internal void RemovePermissionFromMachines(string userID, string[] machineIDs, string groupName)
        {
            var user = RemotelyContext.Users
                      .Include(x => x.Organization)
                      .ThenInclude(x => x.Machines)
                      .FirstOrDefault(x => x.Id == userID);

            var group = user.Organization.PermissionGroups.FirstOrDefault(x => x.Name.ToLower() == groupName.ToLower());
            foreach (var machineID in machineIDs)
            {
                if (user.Organization.Machines.Exists(x => x.ID == machineID))
                {
                    var machine = RemotelyContext.Machines
                                .Include(x => x.PermissionGroups)
                                .FirstOrDefault(x => x.ID == machineID);
                    machine.PermissionGroups.RemoveAll(x => x.ID == group.ID);
                    RemotelyContext.Entry(machine).State = EntityState.Modified;
                }
            }
            RemotelyContext.SaveChanges();
        }
        internal void RemovePermissionFromUser(string requesterUserName, string targetUserID, string permissionID)
        {
            var requester = RemotelyContext.Users
              .Include(x => x.Organization)
              .ThenInclude(x => x.RemotelyUsers)
              .FirstOrDefault(x => x.UserName == requesterUserName);

            var target = requester.Organization.RemotelyUsers.FirstOrDefault(x => x.Id == targetUserID);
            target.PermissionGroups.RemoveAll(x => x.ID == permissionID);
            RemotelyContext.Entry(target).State = EntityState.Modified;
            RemotelyContext.SaveChanges();
        }

        internal void SetServerVerificationToken(string machineID, string verificationToken)
        {
            var machine = RemotelyContext.Machines.Find(machineID);
            if (machine != null)
            {
                machine.ServerVerificationToken = verificationToken;
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

        internal void UpdateTags(string machineID, string tag)
        {
            RemotelyContext.Machines.Find(machineID).Tags = tag;
            RemotelyContext.SaveChanges();
        }

        internal void UpdateUserOptions(string userName, Remotely_Library.Models.RemotelyUserOptions options)
        {
            RemotelyContext.Users.FirstOrDefault(x => x.UserName == userName).UserOptions = options;
            RemotelyContext.SaveChanges();
        }
    }
}
