using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ToDo_Sharp.ConfigService;
using ToDo_Sharp.Database;
using ToDo_Sharp.JwtService;

var builder = WebApplication.CreateBuilder(args);

string? configPath = Environment.GetEnvironmentVariable("CONFIG_PATH");

builder.Services.AddLogging(opts =>
{
    opts.AddConsole();
});

var configService = new ConfigService(configPath);

builder.Services.AddTransient<IConfigService>((i) => configService);

builder.Services.AddTransient<IJwtService, JwtService>();

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    IConfigService configService;
    using (var serviceBuilder = builder.Services.BuildServiceProvider())
    {
        configService = serviceBuilder.GetRequiredService<IConfigService>();
    }
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = configService.Config.ISSUER,

            ValidateAudience = true,

            ValidAudience = configService.Config.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = configService.Config.SymmetricSecurityKey,
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
}).AddCookie(options => options.LoginPath = "/LoginPage");
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Cookie,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

builder.Services.AddControllers();

var app = builder.Build();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run("https://" + configService.Config.Domain);
