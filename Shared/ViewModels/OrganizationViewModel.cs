using System.ComponentModel.DataAnnotations;
namespace Remotely.Shared.ViewModels;

public class OrganizationViewModel
{
  [StringLength(25)]
  public string OrganizationName { get; set; }
}
