using Autofac;
using Autofac.Extensions.DependencyInjection;
using KioskBrowser.Data;
using KioskBrowser.DataService;
using KioskBrowser.DataService.Services;
using KioskBrowser.DataService.Utilities;
using KioskBrowser.WebService.Bind;
using KioskBrowser.WebService.Hubs;
using Microsoft.Extensions.FileProviders;
using Settings = KioskBrowser.WebService.Config.Settings;

var settings = SettingsLoader<Settings>.ReadConfig(new FileInfo(args[0]));
if (settings is null)
{
    Console.WriteLine("Something wrong with the config");
    Environment.Exit(-1);
}
    
var builder = WebApplication.CreateBuilder(settings!.StartArguments);

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
    builder.RegisterType<PhotoWatcherService>().AsSelf().SingleInstance()
        .WithParameter("targetFolder", settings.PictureFileWatchLocation!)
        .WithParameter("urlFolder", settings.PictureUrlLocation!);
    builder.RegisterType<GroupBindService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<MessageBindService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<ProductBindService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<StorageBindService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<PhotoBindService>().AsSelf().SingleInstance().AutoActivate();
});

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        FileUtilities.GetExecutingFile("wwwroot").FullName),
    RequestPath = new PathString("/app")
});

app.UseCors("CorsPolicy");

app.MapFallbackToFile("index.html");

app.MapHub<PingHub>("/ping");
app.MapHub<DataHub>("/data");

app.Run();