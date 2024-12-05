using BusinessLayer.Interface;
using BusinessLayer.Service;
using DataLayer.Context;
using DataLayer.Interface;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using DataLayer.UtilityClass;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Preserve object references to avoid circular reference issues
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Optional: Ignore null properties
    });

// Configure DbContext with connection string
builder.Services.AddDbContext<FundooContext>(cfg =>
    cfg.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

// Register dependencies
builder.Services.AddTransient<IUserDL, UserDL>();
builder.Services.AddTransient<IUserBL, UserBL>();
builder.Services.AddTransient<INoteDL, NoteDL>();
builder.Services.AddTransient<INoteBL, NoteBL>();
builder.Services.AddTransient<TokenUtility>();
builder.Services.AddTransient<ICollaboratorBL, CollaboratorBL>();
builder.Services.AddTransient<ICollaboratorDL, CollaboratorDL>();
builder.Services.AddTransient<ILabelBL, LabelBL>();
builder.Services.AddTransient<ILabelDL, LabelDL>();
builder.Services.AddTransient<ICacheDL, CacheDL>();
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddSingleton<RegistrationEmailConsumerHostedService>();
builder.Services.AddHostedService<RegistrationEmailConsumerHostedService>();
//builder.Services.AddHostedService<RegistrationEmailConsumerHostedService>();


// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Configure Swagger for API documentation with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {your-jwt-token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            var error = new { Message = "An error occurred.", Details = exceptionHandlerPathFeature.Error.Message };
            await context.Response.WriteAsJsonAsync(error);
        }
    });
});

app.UseHttpsRedirection();

app.UseAuthentication(); // Ensure authentication middleware is called before authorization
app.UseAuthorization();

app.MapControllers();

app.Run();

