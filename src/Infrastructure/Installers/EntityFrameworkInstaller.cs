﻿using Application.Installers;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Installers;

public class EntityFrameworkInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default");

        if (string.IsNullOrEmpty(connectionString))
            throw new NotSupportedException(
                "The provided Connection argument ('Default') results in an empty connection string");

        const string x = "$$_CONNECTION_STRING_$$";

        if (connectionString == x)
            connectionString = File.ReadAllText(@"con_string.txt");

        services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));
    }
}