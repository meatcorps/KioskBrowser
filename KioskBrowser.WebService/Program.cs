using Autofac;
using Autofac.Extensions.DependencyInjection;
using KioskBrowser.DataService;
using KioskBrowser.DataService.Services;
using KioskBrowser.DataService.Utilities;
using KioskBrowser.WebService.Bind;
using KioskBrowser.WebService.Hubs;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        );
});

builder.Services.AddSignalR();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterType<PersistentDataService>().AsSelf().SingleInstance().AutoActivate()
        .WithParameter("file", FileUtilities.GetExecutingFile("data.json")!)
        .OnActivated((p) => p.Instance.Load());
    builder.RegisterType<DataCollectionService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<GroupBindService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<MessageBindService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<ProductBindService>().AsSelf().SingleInstance().AutoActivate();
});

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        FileUtilities.GetExecutingFile("wwwroot").FullName),
    RequestPath = new PathString("/app")
});
// app.UseRouting();

app.UseCors("CorsPolicy");

/* app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}"); */

app.MapFallbackToFile("index.html");

app.MapHub<PingHub>("/ping");
app.MapHub<DataHub>("/data");

app.Run();