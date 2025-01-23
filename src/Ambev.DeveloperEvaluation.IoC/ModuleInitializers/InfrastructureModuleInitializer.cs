using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers;
using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.IoC.ModuleInitializers.Interfaces;
using Ambev.DeveloperEvaluation.MessageBroker.Brokers.RabbitMQ;
using Ambev.DeveloperEvaluation.MessageBroker.Interfaces;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(
        WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();
        builder.Services.AddScoped<ISaleItemRepository, SaleItemRepository>();
        builder.Services.AddScoped<IEventPublisher, RabbitMQEventPublisher>();
        builder.Services.AddScoped<ISaleCancelledEventHandler, SaleCancelledEventHandler>();
        builder.Services.AddScoped<ISaleItemCancelledEventHandler, SaleItemCancelledEventHandler>();
        builder.Services.AddScoped<ISaleModifiedEventHandler, SaleModifiedEventHandler>();
        builder.Services.AddScoped<ISaleCreatedEventHandler, SaleCreatedEventHandler>();
    }
}