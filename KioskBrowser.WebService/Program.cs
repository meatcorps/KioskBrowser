using Autofac;
using Autofac.Extensions.DependencyInjection;
using KioskBrowser.Core.Service;
using KioskBrowser.Data;
using KioskBrowser.DataService;
using KioskBrowser.DataService.Services;
using KioskBrowser.DataService.Utilities;
using KioskBrowser.WebService.Bind;
using KioskBrowser.WebService.Hubs;
using KioskBrowser.WebService.Services;
using Microsoft.Extensions.FileProviders;
using Settings = KioskBrowser.WebService.Config.Settings;

var settings = SettingsLoader<Settings>.ReadConfig(new FileInfo(FileUtilities.GetExecutingDirectory("settings.json")));
if (settings is null)
{
    Console.WriteLine("Something wrong with the config");
    Environment.Exit(-1);
}
    
var builder = WebApplication.CreateBuilder(settings!.StartArguments!);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

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
    builder.RegisterType<ActionBindService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<PhotoBindService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<FakeBlackoutService>().AsSelf().SingleInstance().AutoActivate();
    builder.RegisterType<KioskMqttClient>().AsSelf().SingleInstance().AutoActivate()
        .WithParameter("url", settings.BrokerUrl!)
        .OnActivated(i => Task.Run(async () => await i.Instance.Connect()));
        
    builder.RegisterType<MessagePictureImporter>().AsSelf().SingleInstance().AutoActivate()
        .WithParameter("code", settings.ExternalCode!)
        .WithParameter("adminCode", settings.ExternalAdminCode!)
        .WithParameter("url", settings.ExternalUrl!)
        .OnActivated(i => Task.Run(async () => await i.Instance.Start()));
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        FileUtilities.GetExecutingFile("wwwroot").FullName),
    RequestPath = new PathString("/app")
});


app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        FileUtilities.GetExecutingFile(Path.Combine("wwwroot", "assets")).FullName),
    RequestPath = new PathString("/assets")
});

app.UseCors("CorsPolicy");

app.MapFallbackToFile("index.html");

app.MapHub<PingHub>("/ping");
app.MapHub<DataHub>("/data");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");



app.Run();