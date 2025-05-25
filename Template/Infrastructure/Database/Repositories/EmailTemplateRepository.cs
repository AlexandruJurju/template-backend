using Domain.Abstractions.Persistence;
using Domain.EmailTemplates;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

public class EmailTemplateRepository(
    ApplicationDbContext dbContext
) : IEmailTemplateRepository
{
    public async Task<EmailTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await dbContext.EmailTemplates
            .SingleOrDefaultAsync(e => e.Name == name, cancellationToken);
    }
}
