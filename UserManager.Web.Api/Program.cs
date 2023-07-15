using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserManager.DAL.Models;
using UserManager.Web.Api.Configs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserManagerEntities>(opts => 
    opts.UseSqlServer(builder.Configuration.GetConnectionString("UserManagerEntities")));

builder.Services.AddAutoMapper(typeof(MapperConfig));

var app = builder.Build();

app.UseCors(opts => opts.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
