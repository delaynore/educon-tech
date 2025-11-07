using EducationContentService.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true);

builder.Services.AddConfiguration(builder.Configuration);

var app = builder.Build();

app.Configure();

app.Run();
