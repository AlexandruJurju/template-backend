using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Template.Common.SharedKernel.Infrastructure.Storage;

[OptionsValidator]
public sealed partial class StorageOptions : IValidateOptions<StorageOptions>
{
    public const string SectionName = "Storage";

    [Required] public string ContainerName { get; init; }
}
