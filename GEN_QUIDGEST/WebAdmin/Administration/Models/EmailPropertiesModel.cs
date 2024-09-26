using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using System.Text.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;

namespace Administration.Models
{
    /// <summary>
    /// Interface EmailProperties
    /// </summary>
    public class EmailPropertiesModel : ModelBase
    {

        /// <summary>
        /// Class to store information on Email properties to be used when sending messages through email
        /// </summary>
        [Key]
		/// <summary>Campo : "PK da tabela" Tipo: "+" Formula:  ""</summary>
        public string ValCodpmail { get; set; } //Properties identifier

        [Display(Name = "ID36840", ResourceType = typeof(Resources.Resources))]
        public string ValId { get; set; } //Properties identifier

        [Display(Name = "NOME_DO_REMETENTE60175", ResourceType = typeof(Resources.Resources))]
        public string ValDispname { get; set; } //sender name

        [Display(Name = "REMETENTE47685", ResourceType = typeof(Resources.Resources))]
        public string ValFrom { get; set; } //sender email

        [Display(Name = "SERVIDOR_DE_SMTP00309", ResourceType = typeof(Resources.Resources))]
        public string ValSmtpserver { get; set; } // email server

        [Display(Name = "PORTA55707", ResourceType = typeof(Resources.Resources))]
        public int ValPort { get; set; } // porta smtp 

        [Display(Name = "SSL")] 
        public bool ValSsl { get; set; } // ssl connection required

        [Display(Name = "REQUER_AUTENTICACAO_31938", ResourceType = typeof(Resources.Resources))]
        public bool ValAuth { get; set; } //authentication required

        [Display(Name = "UTILIZADOR52387", ResourceType = typeof(Resources.Resources))]
        public string ValUsername { get; set; }

        [Display(Name = "PASSWORD09467", ResourceType = typeof(Resources.Resources))]
        public string ValPassword { get; set; }

        public bool HasPassword { get; set; }
        public string OldId { get; set; }
        public string FormMode { get; set; }
        public string ResultMsg { get; set; }

        public void MapToModel(EmailServer m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (EmailPropertiesModel) to Model (EmailServer) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                m.Name = ValDispname;
                m.From = ValFrom;
                
                m.SMTPServer = ValSmtpserver;
                m.Port = ValPort;
                m.Auth = ValAuth;
                m.SSL = ValSsl;
                m.Username = ValUsername ?? "";
                
                if(ValAuth)
                {
                    if (string.IsNullOrEmpty(ValUsername))
                        throw new BusinessException("Username field is empty.", "EmailPropertiesModel.MapToModel", "Username field is empty.");

                    // Decript current password to check if user changed it
                    string oldPassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(m.Password ?? ""));

                    // Change password if its different or if it wasn't inserted before
                    if (!HasPassword || oldPassword != ValPassword)
                    {
                        if (string.IsNullOrEmpty(ValPassword))
                            throw new BusinessException("Password field is empty.", "EmailPropertiesModel.MapToModel", "Password field is empty.");

                        // Convert new password to base64
                        byte[] pass_bytes = System.Text.Encoding.UTF8.GetBytes(ValPassword ?? "");
                        m.Password = Convert.ToBase64String(pass_bytes, Base64FormattingOptions.None);
                    }
                }

                m.Id = ValId;
				m.Codpmail = ValCodpmail;
            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (EmailPropertiesModel) to Model (EmailServer) - Error during mapping");
                throw;
            }
        }

        public void MapFromModel(EmailServer m)
        {
            if (m == null)
            {
                Log.Error("Map ViewModel (EmailPropertiesModel) to Model (EmailServer) - Model is a null reference");
                throw new Exception("Model not found");
            }
            try
            {
                ValDispname = m.Name;
                ValFrom = m.From;

                ValSmtpserver = m.SMTPServer;
                ValPort = m.Port;
                ValAuth = m.Auth;
                ValSsl = m.SSL;
                ValUsername = m.Username;
                ValPassword = null;

                if (string.IsNullOrEmpty(m.Password)) HasPassword = false;
                else HasPassword = true;

                ValId = m.Id;
				ValCodpmail = m.Codpmail;
            }
            catch (Exception)
            {
                Log.Error("Map ViewModel (EmailPropertiesModel) to Model (EmailServer) - Error during mapping");
                throw;
            }
        }
    }


}

