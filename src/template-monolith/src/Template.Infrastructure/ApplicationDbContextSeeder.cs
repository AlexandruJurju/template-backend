using Template.Common.SharedKernel.Infrastructure.Persistence.EntityFramework;
using Template.Domain.Entities.Users;
using Template.Infrastructure.Database;

namespace Template.Infrastructure;

public class ApplicationDbContextSeeder : IDbSeeder<ApplicationDbContext>
{
    public async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            await context.Users.AddAsync(User.Create("alex@gmail.com", "Alex", "J", "Hash"));
        }
    }
}
