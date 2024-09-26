using CSGenio.framework;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers
{
	public class ConfigController : ControllerExtension
	{
		public ConfigController(UserContextService userContextService) : base(userContextService) { }

		private object getConfig()
		{
			// User
			var user = UserContext.Current.User;

			var defaultSystem = Configuration.DefaultYear;
			var years = Configuration.Years;
			/*
				When the user is already authenticated, we need to validate whether the year
					in which the request was made is among the years to which they have permission.
				This is necessary to be able to correctly validate permissions.
				At the end, the year on which the validation was performed will be returned to change the client-side if necessary (currentSystem).
				The same applies to the default year; if it is not one of the years the user has access to,
					the first year in the allowed list will be assigned as the default.
			*/
			if (!user.IsGuest() && user.Years?.Count > 0)
			{
				if (!user.Years.Contains(defaultSystem))
					defaultSystem = user.Years.First();
				if (!user.Years.Contains(user.Year))
					user.Year = defaultSystem;
				years = user.Years;
			}

			// Modules
			var availableModulesMenus = Helpers.Menus.Menus.AvailableModules(UserContext.Current);
			var availableModules = availableModulesMenus.Select(m => new {
				id = m.ID,
				title = m.Title,
				vector = m.Vector,
				font = m.Font,
				image = m.ImageVUE
			}).ToDictionary(m => m.id, m => m);

			var defaultModule = availableModules.FirstOrDefault().Key ?? "Public";
			var currentModule = user.CurrentModule;
			if (currentModule == null || !availableModules.ContainsKey(currentModule))
				currentModule = defaultModule;

			// Number format
			var numberFormat = new
			{
				DecimalSeparator = Configuration.NumberFormat.DecimalSeparator,
				GroupSeparator = Configuration.NumberFormat.GroupSeparator
			};

			// DateTime format's
			var dateFormat = new
			{
				Time = Configuration.DateFormat.Time,
				Date = Configuration.DateFormat.Date,
				DateTime = Configuration.DateFormat.DateTime,
				DateTimeSeconds = Configuration.DateFormat.DateTimeSeconds
			};

			// Full Calendar license
			var schedulerLicense = Configuration.ExistsProperty("SchedulerLicense") ? Configuration.GetProperty("SchedulerLicense") : null;

			// Home page
			var isGuestUser = user.IsGuest();
			var homePages = new ViewModels.Home.HomePage_ViewModel(UserContext.Current, isGuestUser);

			// Password Recover
			var hasPasswordRecovery = GenioServer.security.SecurityFactory.HasPasswordManagement() && !String.IsNullOrEmpty(Configuration.PasswordRecoveryEmail);

			// Authentification
			var hasUsernameAuth = GenioServer.security.SecurityFactory.HasUsernameAuth();

			var conf = new
			{
				availableModules,
				defaultModule,
				currentModule,
				years,
				defaultSystem,
				currentSystem = user.Year,
				defaultListRows = Configuration.NrRegDBedit,
				userName = user.Name ?? "guest",
				numberFormat,
				dateFormat,
				schedulerLicense,
				homePages = homePages.GetAvaibleHomePages(availableModules.Keys.ToList()),
				hasPasswordRecovery,
				hasUsernameAuth,
				eventTracking = Configuration.EventTracking
			};
			return conf;
		}

		[HttpGet]
		[AllowAnonymous]
		public JsonResult GetConfig()
		{
			var conf = getConfig();

			return JsonOK(conf);
		}

		[HttpGet]
		[AllowAnonymous]
		public JsonResult GetAFToken()
		{
			/*
			AntiForgery.GetTokens(null, out string cookieToken, out string formToken);
			Response.SetCookie(new System.Web.HttpCookie(AntiForgeryConfig.CookieName, cookieToken));
			return JsonOK(new { formToken });
			*/

			return JsonOK();
		}
	}
}
