using Ambev.DeveloperEvaluation.Domain.Common.Messaging;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Sales.Filtering;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Common.Extensions;
using Ambev.DeveloperEvaluation.ORM.Common.Messaging;
using Ambev.DeveloperEvaluation.ORM.EventsHandlers;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISaleRepository, SalesRepository>();
        builder.Services.AddScoped<ISalesQueryParser, SalesQueryParser>();

        var network = new InMemNetwork();
        builder.Services.AddSingleton(network);
        builder.Services.AddScoped<IMessageBus, RebusMessageBus>();
        builder.Services.AddTransient<IHandleMessages<SaleCreatedEvent>, SaleCreatedEventHandler>();
        builder.Services.AddTransient<IHandleMessages<SaleUpdatedEvent>, SaleUpdatedEventHandler>();
        builder.Services.AddTransient<IHandleMessages<SaleCancelledEvent>, SaleCancelledEventHandler>();
        builder.Services.AddTransient<IHandleMessages<SaleItemCancelledEvent>, SaleItemCancelledEventHandler>();
        builder.Services.AddRebus(configure => configure
            .Transport(t => t.UseInMemoryTransport(network, "publisher"))
            .Routing(r => r.TypeBased()
                .Map<SaleCreatedEvent>(EventsConstants.SALES_CREATED_QUEUE_NAME)
                .Map<SaleUpdatedEvent>(EventsConstants.SALES_UPDATED_QUEUE_NAME)
                .Map<SaleCancelledEvent>(EventsConstants.SALES_CANCELLED_QUEUE_NAME)
                .Map<SaleItemCancelledEvent>(EventsConstants.SALES_CANCELLED_ITEM_QUEUE_NAME)));
    }

    public static async Task UseDependenciesAsync(WebApplication application)
    {
        var bus = application.Services.GetRequiredService<IBus>();
        await bus.Subscribe<SaleCreatedEvent>();
        await bus.Subscribe<SaleUpdatedEvent>();
        await bus.Subscribe<SaleCancelledEvent>();
        await bus.Subscribe<SaleItemCancelledEvent>();
    }
}