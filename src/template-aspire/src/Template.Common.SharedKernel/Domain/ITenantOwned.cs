namespace Template.Common.SharedKernel.Domain;

public interface ITenantOwned
{
    Guid TenantId { get; set; }
}
