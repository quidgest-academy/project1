using Administration;
using Administration.Models;
using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.config;
using GenioServer.security;
using log4net;
using log4net.Config;
using SoapCore;

//---------------------------------
// Setup the GenioServer services
//---------------------------------
PersistenceFactoryExtension.Use();
PersistentSupport.SetControlQueries(
    GenioServer.persistence.PersistentSupportExtra.ControlQueries, 
    GenioServer.persistence.PersistentSupportExtra.ControlQueriesOverride);
GenioServer.framework.OverrideQueryDeclaring.Use();


//Dependency injection
UserFactory.BusinessManager = new UserBusinessService();

//---------------------------------
// Setup 3rd party services
//---------------------------------
var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("web.config"));

//---------------------------------
// Setup the WebServer services
//---------------------------------
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options => 
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    }).AddSessionStateTempDataProvider()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; //leave property names unchanged
    })
    .AddXmlSerializerFormatters();


//gzip compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

//Add chaching service
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

//Add SOAP service interface
builder.Services.AddSingleton<IAdminService, WebAPI>();
builder.Services.AddSingleton<IUserManagementService, UserManagement>();

//Add configuration manager
builder.Services.AddSingleton<CSGenio.config.IConfigurationManager>(new FileConfigurationManager(AppDomain.CurrentDomain.BaseDirectory));


// Support for installing as a machine service (windows or linux)
// In the cloud just install this as a normal WebApp with Always On option.
if (OperatingSystem.IsWindows())
    builder.Host.UseWindowsService();
if (OperatingSystem.IsLinux())
    builder.Host.UseSystemd();

//Background services (messaging, scheduling, ...)
builder.WebHost.UseShutdownTimeout(TimeSpan.FromSeconds(60));
builder.Services.AddHostedService<MessagingServiceHost>();
//register the scheduler host as a normal service as well so we can access it from the controllers
builder.Services.AddSingleton<SchedulerServiceHost>();
builder.Services.AddHostedService<SchedulerServiceHost>(p => p.GetRequiredService<SchedulerServiceHost>());


// USE /[MANUAL PRO APP_INIT]/

var app = builder.Build();


app.UseRouting();

//Map SOAP endpoint
((IEndpointRouteBuilder) app).UseSoapEndpoint<IAdminService>("/WebAPI.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer, true); //cast needed to solve ambiguity
((IEndpointRouteBuilder) app).UseSoapEndpoint<IUserManagementService>("/UserManagement.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer, true);


app.UseSession();

// Configure the HTTP request pipeline.
app.UseResponseCompression();

// Redirection needs to come before any routing in the pipeline
// Default will be to use http.
// Set https_port when using a different https port than 443
string? https_redirect = app.Configuration["https_redirect"];
if (https_redirect == "redirect")
    app.UseHttpsRedirection();
if (https_redirect == "hsts")
    app.UseHsts();

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

// AspCore wrapper already does this, so its not needed
//app.UseRouting();

app.UseSession();

//This is only needed when using the [ApiController] attributes
app.MapControllers();

//Get default system
string defaultSystem = "0";

//Default route
app.MapControllerRoute("default",
    "api/{culture}/{system}/{controller}/{action}/{id?}",
    new {
        culture = Administration.AuxClass.Culture.CultureManager.DefaultCulture.Name,
        system = defaultSystem
        }
    );

app.Run();
