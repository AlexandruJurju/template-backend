﻿namespace Domain.Users;

public sealed class EmailVerificationToken
{
    public Guid Id { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime ExpiresOnUtc { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}
