namespace Template.SharedKernel.Domain;

public interface IIsSoftDeletable
{
    bool IsDeleted { get; set; }
}
