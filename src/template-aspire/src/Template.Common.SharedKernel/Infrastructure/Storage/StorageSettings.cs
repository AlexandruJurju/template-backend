using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Template.Common.SharedKernel.Infrastructure.Storage;

[OptionsValidator]
public sealed partial class StorageSettings : IValidateOptions<StorageSettings>
{
    public const string SectionName = "Storage";

    [Required] public string ContainerName { get; init; }
}
