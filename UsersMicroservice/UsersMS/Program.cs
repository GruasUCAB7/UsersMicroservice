using DotNetEnv;
using FluentValidation;
using Microsoft.OpenApi.Models;
using UsersMS.Core.Application.Crypto;
using UsersMS.Core.Application.IdGenerator;
using UsersMS.Core.Application.Logger;
using UsersMS.Core.Infrastructure.Crypto;
using UsersMS.Core.Infrastructure.Data;
using UsersMS.Core.Infrastructure.Logger;
using UsersMS.src.Users.Application.Commands.CreateUser.Types;
using UsersMS.src.Users.Application.Commands.CreateDepto.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Infrastructure.Repositories;
using UsersMS.src.Users.Infrastructure.UUID;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddTransient<IValidator<CreateUserCommand>, CreateUserCommandValidator>();
builder.Services.AddTransient<IValidator<CreateDeptoCommand>, CreateDeptoCommandValidator>();
builder.Services.AddScoped<IUserRepository, MongoUserRepository>();
builder.Services.AddScoped<IDeptoRepository, MongoDeptoRepository>();
builder.Services.AddTransient<IdGenerator<string>, GuidGenerator>();
builder.Services.AddTransient<ICrypto, BcryptCryptoService>();
builder.Services.AddScoped<ILoggerContract, LoggerAspect>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API UsersMicroservice",
        Description = "Endpoints de UsersMicroservice",
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API UsersMicroservice");
        c.RoutePrefix = string.Empty;
    });

    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
