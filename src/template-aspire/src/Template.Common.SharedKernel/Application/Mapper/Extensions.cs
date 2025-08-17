using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Common.SharedKernel.Application.Mapper;

public static class Extensions
{
    public static void AddMappersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(classes =>
                    classes.AssignableTo(typeof(IMapper<,>)).Where(t => !t.IsAbstract)
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
    }
}
