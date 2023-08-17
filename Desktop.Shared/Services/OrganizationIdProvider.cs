namespace Desktop.Shared.Services;

public interface IOrganizationIdProvider
{
    string OrganizationId { get; set; }
}
public class OrganizationIdProvider : IOrganizationIdProvider
{
    public string OrganizationId { get; set; } = string.Empty;
}
