using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Template.Common.SharedKernel.Infrastructure.Email;

[OptionsValidator]
public sealed partial class EmailOptions : IValidateOptions<EmailOptions>
{
    internal const string SectionName = "Email";

    [Required] [EmailAddress] public string SenderEmail { get; init; }
    [Required] public string Sender { get; init; }
    [Required] public int VerificationTokenExpireHours { get; init; }
}
