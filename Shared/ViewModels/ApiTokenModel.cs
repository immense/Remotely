using System.ComponentModel.DataAnnotations;

namespace Remotely.Shared.ViewModels;

#nullable enable
public record ApiTokenModel([StringLength(200)] string Name, string OrganizationID);
