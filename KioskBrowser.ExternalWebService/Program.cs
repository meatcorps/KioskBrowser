using Autofac;
using Autofac.Extensions.DependencyInjection;
using KioskBrowser.Data;
using KioskBrowser.DataService.Utilities;
using KioskBrowser.ExternalWebService.Config;
using KioskBrowser.ExternalWebService.Hubs;
using KioskBrowser.ExternalWebService.Services;

var settings = SettingsLoader<Settings>.ReadConfig(new FileInfo(FileUtilities.GetExecutingDirectory("settings.json")));

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();

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

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(autoFacBuilder =>
{
    autoFacBuilder.RegisterType<CodeService>().SingleInstance().AsSelf().AutoActivate();
    autoFacBuilder.RegisterType<TransferService>().SingleInstance().AsSelf()
        .WithParameter("chunkSize", settings!.ChunkSize!);
    autoFacBuilder.RegisterType<SaveIncomingImagesService>().SingleInstance().AsSelf().AutoActivate();
    autoFacBuilder.RegisterType<MessageService>().SingleInstance().AsSelf();
    autoFacBuilder.RegisterType<VerifyService>().SingleInstance().AsSelf();
    autoFacBuilder.RegisterType<PushService>().SingleInstance().AsSelf()
        .WithParameter("privateKey", settings!.VapidPushPrivate!)
        .WithParameter("publicKey", settings!.VapidPushPublic!);
    autoFacBuilder.RegisterType<UpdateAdminService>().SingleInstance().AsSelf().AutoActivate();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapFallbackToFile("index.html");

app.UseCors("CorsPolicy");

app.MapHub<PingHub>("/ping");
app.MapHub<TransferHub>("/transfer");
app.MapHub<DataHub>("/data");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");


app.Run();