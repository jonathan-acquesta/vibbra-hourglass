using AutoMapper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Vibbra.Hourglass.Api.Configuration;
using Vibbra.Hourglass.Api.MapperConfig;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Infra.Context;
using Vibbra.Hourglass.Infra.Interfaces;
using Vibbra.Hourglass.Infra.Repository;
using Vibbra.Hourglass.Service.Interfaces;
using Vibbra.Hourglass.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerSetup();
builder.Services.AddApiVersioning();

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
