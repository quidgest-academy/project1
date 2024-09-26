using System;
using System.IO;
using System.Drawing;
using Ganss.Xss;
using AngleSharp.Html.Dom;
using CSGenio.framework;

namespace GenioMVC.Helpers
{
    public static class HtmlSanitizerHelper
    {
        // Reusable HtmlSanitizer instance to improve performance.
        // Note: The Sanitize() and SanitizeDocument() methods are thread-safe
        private static readonly HtmlSanitizer sanitizer = new HtmlSanitizer();

        static HtmlSanitizerHelper()
        {
            // Add allowed schemes and attributes during initialization
            // https://github.com/mganss/HtmlSanitizer/wiki
            /*
            * sanitizer.AllowedSchemes.Add("mailto");
            * 
            * // Note: to prevent classjacking (https://html5sec.org/#123) and interference with classes where the sanitized fragment is to be integrated, the class attribute is not in the whitelist by default
            * sanitizer.AllowedAttributes.Add("class")
            */
            sanitizer.RemovingAttribute += SanitizeAttribute;
        }

        /// <summary>
        /// Sanitizes HTML content.
        /// </summary>
        /// <param name="plainText">The HTML content to be sanitized.</param>
        /// <param name="isDocument">Indicates whether the content is a complete HTML document.</param>
        /// <returns>Sanitized HTML content.</returns>
        public static string SanitizeHTML(string plainText, bool isDocument)
        {
            // Determine the base URL for resolving relative URLs. No resolution if empty
            string baseUrl = GetBaseUrl();

            // Sanitize the HTML content, treating it as a document if necessary.
            // Note: In the case of using TinyMCE, the content is the full HTML of a document
            return isDocument ? sanitizer.SanitizeDocument(plainText, baseUrl) : sanitizer.Sanitize(plainText, baseUrl);
        }

        /// <summary>
        /// Handles the attribute removal event, validating the "src" attribute for images with base64 content.
        /// </summary>
        private static void SanitizeAttribute(object sender, RemovingAttributeEventArgs e)
        {
            // Keep the TinyMCE images that use "src" attribute with base64 image
            if (e.Tag is IHtmlImageElement img && e.Attribute.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
            {
                string src = img.Source;

                // Validate base64-encoded images
                if (src?.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (IsValidBase64Image(src))
                    {
                        e.Cancel = true; // Retain the valid image source
                    }
                    else
                    {
                        e.Cancel = false; // Remove invalid image sources
                    }
                }
            }
        }

        /// <summary>
        /// Validates whether a base64-encoded image is a valid and safe image.
        /// </summary>
        /// <param name="dataUrl">The data URL containing the base64-encoded image.</param>
        /// <returns>True if the image is valid; otherwise, false.</returns>
        private static bool IsValidBase64Image(string dataUrl)
        {
            try
            {
                string base64Data = dataUrl.Substring(dataUrl.IndexOf(",") + 1);
                byte[] imageData = Convert.FromBase64String(base64Data);

                using (var ms = new MemoryStream(imageData))
                {
                    Image img = Image.FromStream(ms);
                    return true; // Successfully loaded the image
                }
            }
            catch (Exception ex)
            {
                Log.Error($"HTML Sanitizer - Invalid base64 image detected: {ex.Message} | Source: {dataUrl}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves the base URL for resolving relative URLs in the HTML content.
        /// </summary>
        /// <returns>The base URL as a string.</returns>
        private static string GetBaseUrl()
        {
            return Configuration.ExistsProperty("PUBLIC_BASE_URL")
                ? Configuration.GetProperty("PUBLIC_BASE_URL")
                : string.Empty;
        }
    }
}