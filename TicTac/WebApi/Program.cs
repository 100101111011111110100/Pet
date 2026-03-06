using Datasource.Storage;
 using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<UserAuthenticator>();
    })
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

var app = builder.Build();

//Di.Configuration.GameRepository.MigratePostgresDb();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


