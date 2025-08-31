using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoNow.Application.Abstractions;
using MotoNow.Domain.Repositories;
using MotoNow.Infrastructure.Context;
using MotoNow.Infrastructure.Repositories;
using MotoNow.Infrastructure.Services;

namespace MotoNow.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var cs = config.GetConnectionString("Default");

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseNpgsql(cs, npgsql =>
                    npgsql.MigrationsHistoryTable("__efmigrationshistory", "motonow"));
                    opt.UseSnakeCaseNamingConvention();

            });

            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<IFileStorage, LocalFileStorage>();
            return services;
        }
    }
}
