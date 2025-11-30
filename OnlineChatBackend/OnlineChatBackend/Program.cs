using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OnlineChatBackend.DbContexts;
using OnlineChatBackend.Hubs;
using OnlineChatBackend.Interfaces;
using OnlineChatBackend.Repositories;
using OnlineChatBackend.Services;
using OnlineChatBackend.Settings;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    c.AddSignalRSwaggerGen();
});

builder.Services.AddScoped<IContactsRepository, ContactsDBRepository>();
builder.Services.AddScoped<ContactsDBRepository>();
builder.Services.AddScoped<IMessageRepository, MessageDBRepository>();
builder.Services.AddScoped<IDialogsRepository, DialogDBRepository>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<JwtService>();

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.AddAuth(builder.Configuration);


builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Pg");
    options.UseNpgsql(cs, npg => npg.EnableRetryOnFailure());
});



builder.Services.AddCors(opt =>
{
    opt.AddPolicy("client", b =>
    {
        b.WithOrigins("http://localhost:5173", "http://localhost:5103", "https://localhost:7050")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials();
    });
});

builder.Services.AddSignalR(o => {
    o.EnableDetailedErrors = true;
    o.ClientTimeoutInterval = TimeSpan.FromMinutes(2);  // клиент может молчать 2 мин
    o.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("client");

app.Use(async (context, next) =>
{
    Console.WriteLine($"REQ: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
    await next();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapHub<ChatHub>("/hubs/chat");

app.Run();
