using UsersMS.Core.Application.Crypto;
using UsersMS.Core.Infrastructure.Crypto;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICrypto, BcryptCryptoService>();

var app = builder.Build();

app.Run();
