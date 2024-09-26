using System;
using CSGenio.framework;
using GenioMVC.Models.Navigation;
using GenioServer.security;
using System.ComponentModel.DataAnnotations;
using GenioMVC.ViewModels;

namespace GenioMVC.Models
{
	public interface IValidatable
	{
		CrudViewModelValidationResult Validate(UserContext userContext);
	}

	public abstract class BasicUserModel: IValidatable
	{
		public string UserName { get; set; }

		public string Password { get; set; }
		
        /// <summary>
        /// Validates form fields
        /// </summary>
        /// <returns></returns>
        public abstract CrudViewModelValidationResult Validate(UserContext userContext);
	}

	public abstract class AChangePasswordModel: IValidatable
	{
		public string NewPassword { get; set; }

		public string ConfirmPassword { get; set; }

		public abstract CrudViewModelValidationResult Validate(UserContext userContext);
	}

	public class PasswordRecoverChangeModel : AChangePasswordModel
	{
		public string UserId { get; set; }

		public override CrudViewModelValidationResult Validate(UserContext userContext)
        {
            CrudViewModelFieldValidator validator = new(userContext.User.Language);

            validator.Required("NewPassword", Resources.Resources.NOVA_PALAVRA_CHAVE09647, NewPassword);
			validator.Password("ConfirmPassword", NewPassword, ConfirmPassword);
			
            return validator.GetResult();
        }
	}

	public abstract class ChangePasswordModel : AChangePasswordModel
	{
		public string OldPassword { get; set; }

		public string Password { get; set; }

		public bool Enable2FAOptions { get; set; }
	}

	public class ProfileModel : ChangePasswordModel
	{
		public List<string> OpenIdConnAuthMethods { get; set; }

		public string ValCodpsw { get; set; }

		public string ValNome { get; set; }

		public ProfileModel()
		{
			if (OpenIdConnAuthMethods == null)
				OpenIdConnAuthMethods = new List<string>();
		}

		public override CrudViewModelValidationResult Validate(UserContext userContext)
        {
            CrudViewModelFieldValidator validator = new(userContext.User.Language);

			validator.Required("ValNome", Resources.Resources.UTILIZADOR52387, ValNome);
            validator.Required("OldPassword", Resources.Resources.PALAVRA_CHAVE_ACTUAL29965, OldPassword);
            validator.Required("NewPassword", Resources.Resources.NOVA_PALAVRA_CHAVE09647, NewPassword);
            validator.Password("NewPassword", NewPassword, ConfirmPassword);

            return validator.GetResult();
        }

		public void Save(UserContext userContext)
		{
			Models.Psw item = null;

			// Precisamos posicionar a ficha to não "estragar" o Qvalue do zzstate
			try
			{
				item = Models.Psw.Find(userContext.User.Codpsw, userContext, "FPSW");
			}
			finally
			{
				if (item == null)
					item = new Models.Psw(userContext);
			}

			item.ValPasswordDecrypted = NewPassword;
			item.ValStatus = 0;
			item.Save();
		}

		public void Apply(UserContext userContext)
		{
			Models.Psw item = null;

			// Precisamos posicionar a ficha to não "estragar" o Qvalue do zzstate
			try
			{
				item = Models.Psw.Find(userContext.User.Codpsw, userContext,"FPSW");
			}
			finally
			{
				if (item == null)
					item = new Models.Psw(userContext);
			}

			item.ValPasswordDecrypted = NewPassword;
			item.ValStatus = 0;
			item.Apply();
		}
	}

	public class LogOnModel : BasicUserModel
	{
		public List<string> OpenIdConnAuthMethods { get; set; }

		public List<string> CASAuthMethods { get; set; }

		public List<string> CMDAuthMethods { get; set; }

		public void Load()
		{
			OpenIdConnAuthMethods ??= [];
			CASAuthMethods ??= [];
			CMDAuthMethods ??= [];
		}

		/// <summary>
		/// Checks if the application is setup to allow password recovery
		/// </summary>
		/// <returns></returns>
		public bool HasPasswordRecovery
		{
			get { return SecurityFactory.HasPasswordManagement() && !string.IsNullOrEmpty(Configuration.PasswordRecoveryEmail); }
		}

		/// <summary>
		/// Determines whether username and password authentication is enabled.
		/// </summary>
		/// <remarks>
		/// This property returns true if either QuidgestIdentityProvider or LdapIdentityProvider is present in the list of identity providers.
		/// This is used to determine if username and password authentication is enabled, assuming that either QuidgestIdentityProvider
		/// or LdapIdentityProvider supports this method of authentication.
		/// </remarks>
		public bool HasUsernameAuth
		{
			get { return SecurityFactory.HasUsernameAuth(); }
		}

		public override CrudViewModelValidationResult Validate(UserContext userContext)
		{
            CrudViewModelFieldValidator validator = new(userContext.User.Language);

            validator.Required("UserName", Resources.Resources.UTILIZADOR52387, UserName);
			validator.Required("Password", Resources.Resources.PALAVRA_CHAVE39832, Password);

			return validator.GetResult();
        }
	}

	public class PasswordRecoverViewModel: IValidatable
	{
		public string Email { get; set; }

		public bool IsEmailSent { get; set; }

		public CrudViewModelValidationResult Validate(UserContext userContext)
		{
			CrudViewModelFieldValidator validator = new(userContext.User.Language);
			
			validator.Required("Email", Resources.Resources.EMAIL25170, Email);
			validator.Email("Email", Email);
					
			return validator.GetResult();
		}
	}

}
