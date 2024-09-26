using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GenioMVC.ViewModels
{
	/// <summary>
	/// Model for the 2FA
	/// </summary>
	public class TwoFAViewModel
	{
		/// <summary>
		/// Code to validate the user input
		/// </summary>
		[Required]
		public string Totp6Code { get; set; }

		/// <summary>
		/// Toggle for TOTP
		/// </summary>
		public int HasTotp { get; set; }

		/// <summary>
		/// Toggle for WebAuthN
		/// </summary>
		public int HasWebAuthN { get; set; }

		/// <summary>
		/// Url that will be transformed to a QRcode
		/// </summary>
		public string TotpUrl { get; set; }

		/// <summary>
		/// Code to be entered manually in the application
		/// </summary>
		public string TotpDisplayCode { get; set; }

		/// <summary>
		/// Show TOTP configuration
		/// </summary>
		public bool ShowTotp { get; set; } = false;

		/// <summary>
		/// Show WebAuthn configuration
		/// </summary>
		public bool ShowWebAuthN { get; set; } = false;

		public SelectList EmptySelectList
		{
			get => new SelectList(new List<SelectListItem>
			{
				new SelectListItem { Text = "", Value = "0"},
				new SelectListItem { Text = "", Value = "1"},
			}, "Value", "Text");
		}
	}
}
