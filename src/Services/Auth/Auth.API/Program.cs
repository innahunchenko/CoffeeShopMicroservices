using Auth.API.Data;
using Auth.API.Services;
using Auth.API.Repositories;
using Foundation.Abstractions;
using Microsoft.EntityFrameworkCore;
using Foundation.Exceptions;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Auth.API.Domain.Models;
using FluentValidation;
using MediatR;
using Auth.API.Validation;
using Foundation.Abstractions.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Security.OptionsSetup;
using Security.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddIdentity<CoffeeShopUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<TokenUrlEncoderService>();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificAndDynamicOrigins", builder =>
//    {
//        builder.WithOrigins("https://4e97-188-163-68-200.ngrok-free.app")
//        .AllowAnyMethod()
//        .AllowAnyHeader()
//        .AllowCredentials();
//    });
//});

// Must be specified after AddIdentity
builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer();

//builder.WebHost.ConfigureKestrel(options =>
//{
//    var certificatePassword = builder.Configuration["Kestrel:Certificates:Default:Password"];
//    var certificatePath = builder.Configuration["Kestrel:Certificates:Default:Path"]!;
//    var defaultCertificate = new X509Certificate2(certificatePath, certificatePassword);
//    options.ListenAnyIP(8081, listenOptions =>
//    {
//        listenOptions.UseHttps(httpsOptions =>
//        {
//            httpsOptions.ServerCertificateSelector = (context, name) =>
//            {
//                if (name == "auth-api")
//                {
//                    return X509Certificate2.CreateFromPemFile(
//                        "/https/auth-api.crt",
//                        "/https/auth-api.key");
//                }
//                else
//                {
//                    return defaultCertificate;
//                }
//            };
//        });
//    });
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowSpecificAndDynamicOrigins");
app.UseExceptionHandler(options => { });
app.InitialiseDatabaseAsync<AppDbContext>();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
