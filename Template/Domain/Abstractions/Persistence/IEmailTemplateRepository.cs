using Domain.EmailTemplates;

namespace Domain.Abstractions.Persistence;

public interface IEmailTemplateRepository
{
    Task<EmailTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
