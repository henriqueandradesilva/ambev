using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.MessageBroker.Brokers.RabbitMQ;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Extensions;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            var builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            ConfigureServices(builder);

            ConfigureSerilog(builder);

            var app = builder.Build();

            ConfigureMiddleware(app);

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(
        WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.AddBasicHealthChecks();

        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                             .AddEnvironmentVariables();

        var connectionString = builder.Configuration["DB_CONNECTION_STRING"];

        if (string.IsNullOrEmpty(connectionString))
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        var serilogLoggerFactory = new SerilogLoggerFactory();

        builder.Services.AddDbContext<DefaultContext>(options =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM"));
            options.UseLoggerFactory(serilogLoggerFactory);
        });

        builder.Services.AddJwtAuthentication(builder.Configuration);

        builder.RegisterDependencies();

        builder.Services.AddAutoMapper(
            typeof(Program).Assembly,
            typeof(ApplicationLayer).Assembly
        );

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(Program).Assembly
            );
        });
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMqSettings"));
        builder.Services.AddSingleton<RabbitMQConnector>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
            return new RabbitMQConnector(config.HostName, config.UserName, config.Password);
        });

        builder.Services.AddSingleton<RabbitMQEventPublisher>();
    }

    private static void ConfigureSerilog(
        WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
            .WriteTo.Console()
            .WriteTo.File(
                formatter: new RenderedCompactJsonFormatter(),
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Error
            ));
    }

    private static void ConfigureMiddleware(
        WebApplication app)
    {
        app.UseMiddleware<ValidationExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.ApplyMigrations();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseBasicHealthChecks();

        app.MapControllers();
    }
}