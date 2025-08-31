using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MotoNow.API.Filters;
using MotoNow.API.Middlewares;
using MotoNow.Application.Results;
using MotoNow.Application.Services;
using MotoNow.Application.Services.Consumers;
using MotoNow.Infrastructure.Extensions;
using MotoNow.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using MotoNow.Application.Mappings;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MotoNow.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(opts =>
{
    opts.Filters.Add<ApiResponseEnvelopeFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.Configure<ApiBehaviorOptions>(o =>
{
    o.InvalidModelStateResponseFactory = ctx =>
    {
        var errors = ctx.ModelState
            .Where(kv => kv.Value?.Errors.Count > 0)
            .ToDictionary(
                kv => kv.Key,
                kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var payload = new ApiError(
            Sucesso: false,
            Codigo: "model_validation",
            Mensagem: "Dados inválidos.",
            Detalhes: errors,
            TraceId: ctx.HttpContext.TraceIdentifier);

        return new BadRequestObjectResult(payload);
    };
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IMotorCycleService, MotorCycleService>();
builder.Services.AddScoped<IDeliveryDriverService, DeliveryDriverService>();
builder.Services.AddScoped<IRentalService, RentalService>();

var mq = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(mq["Host"] ?? "localhost", mq["VirtualHost"] ?? "/", h =>
        {
            h.Username(mq["Username"] ?? "admin");
            h.Password(mq["Password"] ?? "adm12346");
        });
    });
});


var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile<AppMappingProfile>();
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


var app = builder.Build();

app.UseExceptionHandler();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
