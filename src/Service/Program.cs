using Application.Serialization;
using Infrastructure;
using Infrastructure.Hubs;
using Service.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(
                "https://fixesth.is",
                "http://fixesth.is",
                "http://localhost:4200",
                "http://localhost:8100",
                "https://btcis.me"
            )
            .AllowCredentials()
    )
);
builder.Services.AddHostedService<DeleteObsoleteSessions>();
builder.Services.AddHostedService<SessionUserUpdates>();
builder.Services.AddHostedService<SessionKeepAliveUpdates>();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.SetDefaults());
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
}).AddJsonProtocol(options => options.PayloadSerializerOptions.SetDefaults());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

InfrastructureInstaller.Install(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.MapHub<SessionHub>("/sessions-hub");

app.Run();