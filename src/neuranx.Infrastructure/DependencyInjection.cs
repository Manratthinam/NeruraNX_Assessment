
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using neuranx.Application.Interfaces;
using neuranx.Infrastructure.Identity;
using neuranx.Infrastructure.Persistence;
using neuranx.Infrastructure.Repositories;
using neuranx.Infrastructure.Services;

namespace neuranx.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());



        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
