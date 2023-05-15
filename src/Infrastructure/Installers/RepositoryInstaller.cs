using Application.Installers;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Installers;

public class RepositoryInstaller : IInstaller
{
    public void Install(IServiceCollection services, IConfiguration config)
    {
        if (config.GetValue<bool>("ConnectToDatabase"))
        {
            services.AddTransient<ISessionRepository, SessionRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
        }
        else
        {
            services.AddTransient<ISessionRepository, Repositories.InMemory.SessionRepository>();
            services.AddTransient<IUserRepository, Repositories.InMemory.UserRepository>();
        }
    }
}