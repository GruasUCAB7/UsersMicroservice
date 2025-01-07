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
using UsersMS.src.Users.Application.Commands.UpdateUser.Types;
using UsersMS.src.Users.Application.Repositories;
using UsersMS.src.Users.Infrastructure.Repositories;
using UsersMS.Core.Infrastructure.UUID;
using UsersMS.src.Users.Infrastructure.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UsersMS.src.Users.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddTransient<IValidator<CreateUserCommand>, CreateUserValidator>();
builder.Services.AddTransient<IValidator<UpdateUserCommand>, UpdateUserValidator>();
builder.Services.AddScoped<IUserRepository, MongoUserRepository>();
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

    // Configuración de seguridad para JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el formato: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowApiGateway",
        builder => builder.WithOrigins("https://localhost:4000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")))
    };

});

builder.Services.AddScoped<JwtService>();
builder.Services.AddTransient<EmailService>();

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
app.UseRouting();
app.UseCors("AllowApiGateway");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();



app.Run();
