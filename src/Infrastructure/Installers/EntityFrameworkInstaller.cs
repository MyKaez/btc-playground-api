﻿using Application.Installers;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Installers;

public class EntityFrameworkInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        InstallConnectionString(services, config);

        services.AddTransient<ISessionRepository, SessionRepository>();
    }

    private static void InstallConnectionString(IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default");

        if (string.IsNullOrEmpty(connectionString))
            throw new NotSupportedException(
                "The provided Connection argument ('Default') results in an empty connection string");

        const string c = "$$_CONNECTION_STRING_$$";

        if (connectionString == c)
            connectionString = File.ReadAllText(@"con_string.txt");

        services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));
    }
}