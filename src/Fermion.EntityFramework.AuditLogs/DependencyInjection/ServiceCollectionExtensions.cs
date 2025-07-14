using System.Reflection;
using Fermion.Domain.Shared.Conventions;
using Fermion.EntityFramework.AuditLogs.Application.Services;
using Fermion.EntityFramework.AuditLogs.Domain.Interfaces.Repositories;
using Fermion.EntityFramework.AuditLogs.Domain.Interfaces.Services;
using Fermion.EntityFramework.AuditLogs.Domain.Options;
using Fermion.EntityFramework.AuditLogs.Infrastructure.Repositories;
using Fermion.EntityFramework.AuditLogs.Presentation.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fermion.EntityFramework.AuditLogs.DependencyInjection;

/// <summary>
/// Extension methods for configuring audit logging services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds audit logging services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="AuditLogOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddFermionAuditLogServices<TContext>(this IServiceCollection services, Action<AuditLogOptions> configureOptions) where TContext : DbContext
    {
        var options = new AuditLogOptions();
        configureOptions.Invoke(options);
        services.Configure<AuditLogOptions>(configureOptions.Invoke);

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddHttpContextAccessor();

        services.AddScoped<IAuditLogRepository, AuditLogRepository<TContext>>();
        services.AddScoped<IEntityPropertyChangeRepository, EntityPropertyChangeRepository<TContext>>();
        services.AddScoped<IAuditLogAppService, AuditLogAppService>();

        if (options.AuditLogController.Enabled)
        {
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.ApplicationParts.Add(new AssemblyPart(typeof(AuditLogController).Assembly));
                });

            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerAuthorizationConvention(
                    typeof(AuditLogController),
                    options.AuditLogController.Route,
                    options.AuditLogController.GlobalAuthorization,
                    options.AuditLogController.Endpoints
                ));
            });
        }
        else
        {
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerDisablingConvention(typeof(AuditLogController)));
            });
            services.PostConfigure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerRemovalConvention(typeof(AuditLogController)));
            });
        }

        return services;
    }
}