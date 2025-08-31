using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MotoNow.Application.Services.Consumers;
using MotoNow.Infrastructure.Extensions;
using MotoNow.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MotorcycleRegisteredConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"]);
            h.Password(builder.Configuration["RabbitMq:Password"]);
        });

        cfg.ReceiveEndpoint(builder.Configuration["RabbitMq:Queue"]!, ep =>
        {
            ep.ConfigureConsumer<MotorcycleRegisteredConsumer>(context);
        });
    });
});

var app = builder.Build();


app.Run();
