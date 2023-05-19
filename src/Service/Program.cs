using System.Text.Json;
using System.Text.Json.Serialization;
using Infrastructure;
using Infrastructure.Hubs;
using Service.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:4200", "http://localhost:8100", "https://btcis.me", "https://fixesth.is")
            .AllowCredentials()
    )
);
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.SetApiDefaults());
builder.Services.AddSignalR().AddJsonProtocol(options => options.PayloadSerializerOptions.SetApiDefaults());
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