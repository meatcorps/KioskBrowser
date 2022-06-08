using Autofac;
using Autofac.Extensions.DependencyInjection;
using KioskBrowser.DataService;
using KioskBrowser.DataService.Services;
using KioskBrowser.DataService.Utilities;
using KioskBrowser.WebService.Bind;
using KioskBrowser.WebService.Hubs;

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("CorsPolicy");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.MapHub<PingHub>("/ping");
app.MapHub<DataHub>("/data");

app.Run();