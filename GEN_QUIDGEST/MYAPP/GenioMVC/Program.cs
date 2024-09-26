using CSGenio.framework;
using CSGenio.persistence;
using GenioServer.security;
using Microsoft.AspNetCore.Mvc;
using GenioMVC;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using log4net;
using log4net.Config;

//---------------------------------
// Setup the GenioServer services
//---------------------------------
PersistenceFactoryExtension.Use();
PersistentSupport.SetControlQueries(
    GenioServer.persistence.PersistentSupportExtra.ControlQueries,
    GenioServer.persistence.PersistentSupportExtra.ControlQueriesOverride);
GenioServer.framework.OverrideQueryDeclaring.Use();
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

//If it finds it, read the web.config as if it was a strongly typed Options provider
((IConfigurationBuilder)builder.Configuration).Add(new WebConfigConfigurationSource());

// Customize the default automatic validation behaviour to our custom on demand validation
builder.Services.AddSingleton<IObjectModelValidator>( s =>
{
    var options = s.GetRequiredService<IOptions<MvcOptions>>().Value;
    var metadataProvider = s.GetRequiredService<IModelMetadataProvider>();
    return new OnDemandObjectValidator(metadataProvider, options.ModelValidatorProviders, options);
});

// Add services to the container.
builder.Services.AddControllers(options => 
    {
        options.Filters.Add<ModuleActionFilter>();
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        //options.ModelBinderProviders.Insert(0, new GenioBindingProvider());
    }).AddSessionStateTempDataProvider()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; //leave property names unchanged

        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new BoolJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new SelectListJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new NameValueCollectionJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new ConcurrentStackJsonConverter<GenioMVC.Models.Navigation.HistoryLevel>());

        options.JsonSerializerOptions.TypeInfoResolver = new ConditionalJsonInfoResolver();
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Add Http Client for ChatbotAPI
builder.Services.AddHttpClient();

// Any controller that needs User information it can add UserContextService to its constructor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();
//TODO: Replace all controller constructors that use the concrete class and replace it with the interface
builder.Services.AddScoped<UserContextService>();


// Add a shim authentication to provide compatibility with the old code
// It would actually handle the authentication, UserContextService will deal with that
// but it will simulate what old Asp.Net Mvc webapps initialized in HttpContext.User
// https://enzonunziata.com/article/custom-aspnet-core-authentication-part-1

// Setup Session
// The Distributed Memory Cache isn't an actual distributed cache. Cached items are stored by the app instance on the server where the app is running.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    builder.Configuration.GetSection("SessionOptions").Bind(options);
});

//Authentication handlers
var schemeName = LegacyFormsAuthenticationOptions.DefaultScheme;
builder.Services.AddAuthentication(schemeName)
    .AddScheme<LegacyFormsAuthenticationOptions, LegacyFormsAuthentication>(schemeName, options => {
        builder.Configuration.GetSection("LegacyFormsAuthentication").Bind(options);
    });

//gzip compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

//Background services (messaging, scheduling, ...)
builder.WebHost.UseShutdownTimeout(TimeSpan.FromSeconds(60));
builder.Services.AddHostedService<MessagingServiceHost>();

// USE /[MANUAL PRO APP_INIT]/

var app = builder.Build();

// Log unhandled exceptions and detail the error details in http 500 responses
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
app.UseResponseCompression();

// Redirection needs to come before any routing in the pipeline
// Default will be redirecting to https.
// Set https_redirect to 'none' when reverse proxy deals already deals with https redirection.
// Set https_port when using a different https port than 443
string? https_redirect = app.Configuration["https_redirect"];
if (https_redirect == null || https_redirect == "redirect")
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

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

//This is only needed when using the [ApiController] attributes
//app.MapControllers();


//Used for user authentication with OpenId Connect. This method require one url for callback after login are made. This will permit to use schema + domain + "/OpenIdLogin" to be use on Call back.
app.MapControllerRoute("OIdAuth", "OpenIdLogin", new { culture = "en-US", system = Configuration.DefaultYear, controller = "Account", action = "OpenIdLogin", module = "Public" });
//Used for user regist your OpenId Connect Account. This method require one url for callback after login are made. This will permit to use schema + domain + "/OpenIdRegister" to be use on Call back.
app.MapControllerRoute("OIdRegist", "OpenIdRegister", new { culture = "en-US", system = Configuration.DefaultYear, controller = "Home", action = "OpenIdRegister", module = "Public" });
//Used for user authentication with CAS. This method require one url for callback after login are made. This will permit to use schema + domain + "/CASLogin" to be use on Call back.
app.MapControllerRoute("CASAuth", "CASLogin", new { culture = "en-US", system = Configuration.DefaultYear, controller = "Account", action = "CASLogin", module = "Public" });

//Chatbot API proxy endpoints
app.MapControllerRoute(
    name: "chatbotapi",
    pattern: "chatbotapi/prompt/message",
    defaults: new { controller = "ChatbotApi", action = "ChatbotApiStreamProxy" });

app.MapControllerRoute(
    name: "chatbotapi",
    pattern: "chatbotapi/{**values}",
    defaults: new { controller = "ChatbotApi", action = "ChatbotApiProxy" });

//Configuration and Antiforgery token
app.MapControllerRoute("config",
    "api/Config/{action}/{system}",
    new {
        controller = "Config",
        action = "GetConfig",
        system = Configuration.DefaultYear
        }
    );

//User profile
app.MapControllerRoute("RouteForUsersProfile",
    "{culture}/{system}/User{action}/{id}",
    new
    {
        culture = "en-US",
        system = Configuration.DefaultYear,
        controller = "Home",
        action = "Profile",
        module = "Public"
    }
);

//Default route
app.MapControllerRoute("default",
    "api/{culture}/{system}/{module}/{controller}/{action}/{id?}",
    new {
        culture = "en-US",
        system = Configuration.DefaultYear,
        module = "Public",
        controller = "Home",
        action = "Index"
        }
    );

app.Run();
