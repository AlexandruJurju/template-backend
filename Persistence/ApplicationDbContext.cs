using Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    
}