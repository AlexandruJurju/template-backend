namespace Template.Infrastructure.Email;

public class EmailOptions
{
    public string SenderEmail { get; init; }
    public string Sender { get; init; }
    public int VerificationTokenExpireHours { get; init; }
}
