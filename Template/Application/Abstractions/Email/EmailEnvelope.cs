namespace Application.Abstractions.Email;

public record EmailEnvelope(string ToMail, string Subject, string Body);
