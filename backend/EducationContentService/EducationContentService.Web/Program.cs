using System.Text.Json.Serialization;
using EducationContentService.Web.Extensions;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var env = builder.Environment;

builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true);

builder.Services.AddConfiguration(builder.Configuration);

var app = builder.Build();

app.Configure();

app.Run();
