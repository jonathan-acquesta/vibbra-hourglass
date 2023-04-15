using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vibbra.Hourglass.Api.Configuration;
using Vibbra.Hourglass.Api.MapperConfig;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Infra.Context;
using Vibbra.Hourglass.Infra.Interfaces;
using Vibbra.Hourglass.Infra.Repository;
using Vibbra.Hourglass.Service.Interfaces;
using Vibbra.Hourglass.Service.Services;
using AuthenticationService = Vibbra.Hourglass.Service.Services.AuthenticationService;
using IAuthenticationService = Vibbra.Hourglass.Service.Interfaces.IAuthenticationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerSetup();
builder.Services.AddApiVersioning();
builder.Services.AddAuthentication("SchemeVibbraHourglass");

ConfigureTokenJwt(builder.Configuration, builder.Services);

#region Database EF

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDb"), opt =>
    {
        opt.CommandTimeout(60 * 60);
        opt.EnableRetryOnFailure(5);
    });
});

#endregion

#region Service Dependence Injection

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITimeService, TimeService>();

#endregion

#region Repository Dependence Injection

builder.Services.AddScoped<IBaseRepository<UserDomain>, ApplicationRepository<UserDomain>>();
builder.Services.AddScoped<IBaseRepository<ProjectDomain>, ApplicationRepository<ProjectDomain>>();
builder.Services.AddScoped<IBaseRepository<TimeDomain>, ApplicationRepository<TimeDomain>>();

#endregion

#region Mapper

builder.Services.AddSingleton(new MapperConfiguration(config =>
{
    config.AddProfile<ProfileMapperConfiguration>();
}).CreateMapper());

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseApiVersioning();
app.UseSwaggerSetup();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



static void ConfigureTokenJwt(IConfiguration configuration, IServiceCollection services)
{
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
   .AddJwtBearer(options =>
   {
       options.SaveToken = true;
       options.RequireHttpsMetadata = false;
       options.TokenValidationParameters = new TokenValidationParameters()
       {
           ValidateIssuer = false,
           ValidateAudience = false,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtSecret"]))
       };
   });
    services.AddAuthorization(options =>
    {
        options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser().Build());
    });
}
