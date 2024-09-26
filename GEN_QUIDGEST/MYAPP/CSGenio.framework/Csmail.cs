using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace CSGenio.framework
{
    /// <summary>
    /// Classe que representa um email
    /// exists a posiblidade de enviar vários ficheros em attachment, basta passar um array de string com os nomes dos ficheiros a anexar. atenção que os nomes dos ficheiros tem que ser caminhos completos.    
    /// também e possível enviar um mail to vários destinatários, basta criar uma string com os mail separados por vírgula (,)ex:"quidgest@quidgest.pt,jpedro@quidgest.pt"    
    /// </summary>
    public class CSmail
    {
        private string de;//e-mail do remetente
        private string to;//e-mail(s) do destinatário(s)
        private string subject;//subject do e-mail
        private string body;//body do e_email
        private bool bodyhtml;//indica se o body do e-mail vai em html //(FFS 2014.10.16)
        private string[] attachment;//lista com os nomes dos ficheiros anexos
        private string smtpServer; // GenioServer de mail 
        private bool ssl = false; // Ligação ssl (MA 2009.10.07)
        private int port = 25; // porta smtp (MA 2009.10.07)
        private bool auth = false;
        private string user;
        private string pass;
        private string cc; //endereços em CC (JMT 2011.04.04)
        private string bcc; //endereços em Bcc (PR 2014.10.16)
        private string textass; //text após imagem da assinatura (SF 2016.02.10)
        private string pathimg; //imagem da assinatura (SF 2016.02.10)
        private string nomeremetente; //nome a apresentar no remetente
        private Dictionary<string, Stream> dictionaryanexos; //Anexos por stream (ao invés de path)
        private List<Stream> streamimagens; //Imagens no corpo do email, por stream (ao invés de path)
        public string ReplyTo { get; set; } // Propriedade para o endereço "Reply-To"

        /// <summary>
        /// Constructor dum Qfield que nao é formula, nem array,  nem tem Qvalue default
        /// </summary>
        /// <param name="de"></param>
        /// <param name="para"></param>
        /// <param name="assunto"></param>
        /// <param name="anexo"></param>
        /// <param name="smtpServer"></param>
        public CSmail(string de,
                         string to,
                         string subject,
                         string body,
                         string[] attachment,
                         string smtpServer,
                         int port, // (MA 2009.10.07)
                         bool ssl,  // (MA 2009.10.07)
                         string cc,  // (JMT 2011.04.04)
                         string bcc,
                         string textass,
                         string pathimg,
                         bool bodyhtml//(FFS 2014.10.16)
        )
        {
            this.de = de;
            this.to = to;
            this.subject = subject;
            this.body = body;
            this.bodyhtml = bodyhtml;//(FFS 2014.10.16)
            this.attachment = attachment;
            this.smtpServer = smtpServer;
            this.port = port; // (MA 2009.10.07)
            this.ssl = ssl; // (MA 2009.10.07)
            this.cc = cc;   // (JMT 2011.04.04)
            this.bcc = bcc;
            this.textass = textass;
            this.pathimg = pathimg;
        }
		
		public CSmail(string de,
                         string to,
                         string subject,
                         string body,
                         string[] attachment,
                         string smtpServer,
                         int port, // (MA 2009.10.07)
                         bool ssl,  // (MA 2009.10.07)
                         string cc  // (JMT 2011.04.04)
        )
        {
            this.de = de;
            this.to = to;
            this.subject = subject;
            this.body = body;
            this.bodyhtml = false;//(FFS 2014.10.16)
            this.attachment = attachment;
            this.smtpServer = smtpServer;
            this.port = port; // (MA 2009.10.07)
            this.ssl = ssl; // (MA 2009.10.07)
            this.cc = cc;   // (JMT 2011.04.04)
            this.bcc = "";
            this.textass = "";
            this.pathimg = "";
        }
        public CSmail(string de,
                         string para,
                         string assunto,
                         string corpo,
                         string[] anexo,
                         string smtpServer,
                         int port, // (MA 2009.10.07)
                         bool ssl,  // (MA 2009.10.07)
                         string cc,  // (JMT 2011.04.04)
                         string nomeremetente
        )
        {
            this.de = de;
            this.to = para;
            this.subject = assunto;
            this.body = corpo;
            this.bodyhtml = false;//(FFS 2014.10.16)
            this.attachment = anexo;
            this.smtpServer = smtpServer;
            this.port = port; // (MA 2009.10.07)
            this.ssl = ssl; // (MA 2009.10.07)
            this.cc = cc;   // (JMT 2011.04.04)
            this.bcc = "";
            this.textass = "";
            this.pathimg = "";
            this.nomeremetente = nomeremetente;
        }

        public CSmail(   string nomeremetente,
                         string de,
                         string para,
                         string assunto,
                         string corpo,
                         Dictionary<string, Stream> dictionaryanexos, //nome_anexo + anexo
                         string smtpServer,
                         int port, // (MA 2009.10.07)
                         bool ssl,  // (MA 2009.10.07)
                         string cc,  // (JMT 2011.04.04)
                         string bcc,
                         List<Stream> imagens,
                         string textass,
                         bool bodyhtml
                         //(FFS 2014.10.16)
        )
        {
            this.nomeremetente = nomeremetente;
            this.de = de;
            this.to = para;
            this.subject = assunto;
            this.body = corpo;
            this.dictionaryanexos = dictionaryanexos;
            this.smtpServer = smtpServer;
            this.port = port; // (MA 2009.10.07)
            this.ssl = ssl; // (MA 2009.10.07)
            this.cc = cc;   // (JMT 2011.04.04)
            this.bcc = bcc;
            this.streamimagens = imagens;
            this.textass = textass;
            this.bodyhtml = bodyhtml;
        }

        /// <summary>
        /// Constructor dum Qfield que nao é formula, nem array,  nem tem Qvalue default
        /// </summary>
        public CSmail()
        {
            de = "quidgest@quidgest.pt";
            to = "quidmail@quidgest.pt";
            subject = "";
            body = "E-mail enviado pelo programa RQW";
            bodyhtml = false;//(FFS 2014.10.16)
            attachment = new string[1] { "" };
            smtpServer = "cp99.webserver.pt";
            port = 25;  // (MA 2009.10.07)
            ssl = false; // (MA 2009.10.07)
            cc = "";    //(JMT 2011.04.04)
            bcc = ""; //(PR 2012.04.03)
            textass = "";//(SF 2016.02.10)
            pathimg = "";//(SF 2012.02.10)
        }


			
        /// <summary>
        /// Método que envia o email
        /// </summary>        
        public bool Send()
        {

            if (validate())
            {
                // To turn on 1.2 without affecting other protocols. It is preferred that it be configured at application startup.
                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
				
                string host = smtpServer;
                System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient(host, port);

                System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(de, nomeremetente, System.Text.Encoding.Default);
                using (System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage())
                {
                    msg.From = from;

                    string[] listaMail = to.Split(new char[] { (';'), (',') });
                    for (int j = 0; j < listaMail.Length; j++)
                    {
                        if (validateMail(listaMail[j]))
                        {
                            System.Net.Mail.MailAddress endereco = new System.Net.Mail.MailAddress(listaMail[j], null, System.Text.Encoding.Default);
                            msg.To.Add(endereco);
                        }
                    }

                    //(CH 2024.01.26) - 
                    if (!string.IsNullOrEmpty(this.ReplyTo)) // Verifica se o campo 'Reply-To' foi definido
                    {
                        msg.ReplyToList.Add(new MailAddress(this.ReplyTo)); // Adiciona o endereço ao campo 'Reply-To'
                    }

                    //(SF 2016.02.10) - Acrecentar imagem da assinatura no body do email
                    if (!string.IsNullOrEmpty(pathimg) || streamimagens != null) //extensão para uma lista com stream de imagens
                    {
                        // body = body + "<img src=\"cid:image1\">" + textass;
                        System.Net.Mail.AlternateView av = null;
                        System.Net.Mail.LinkedResource lr = null;
                        if (!string.IsNullOrEmpty(pathimg))
                        {
                            body = body + "<img src=\"cid:image1\">" + textass;
                            lr = new System.Net.Mail.LinkedResource(pathimg, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                            av = System.Net.Mail.AlternateView.CreateAlternateViewFromString(body, null, System.Net.Mime.MediaTypeNames.Text.Html);
                            lr.ContentId = "image1";
                            av.LinkedResources.Add(lr);
                            msg.AlternateViews.Add(av);
                        }
                        else
                        {
                            bodyhtml = true;
                            foreach (var imagem in streamimagens)
                            {
                                if (imagem != null)
                                {
                                    var image = new LinkedResource(imagem);
                                    image.ContentId = Guid.NewGuid().ToString();

                                    body += string.Format(@"<br/><img src=""cid:{0}"" />", image.ContentId);
                                    body += "<br/>" + textass;
                                    body = body.Replace("\r\n", "<br/>");

                                    AlternateView view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                                    view.LinkedResources.Add(image);
                                    msg.AlternateViews.Add(view);

                                }
                            }
                        }
                        msg.Body = body;
                    }
                    else
                        msg.Body = body + textass;

                    msg.Subject = subject;
                    msg.SubjectEncoding = System.Text.Encoding.Default;

                    for (int i = 0; i < attachment.Length; i++)
                    {
                        if (!attachment[i].Equals("") && File.Exists(attachment[i]))
                        {
                            System.Net.Mail.Attachment fanexo = new System.Net.Mail.Attachment(attachment[i]);
                            msg.Attachments.Add(fanexo);
                        }
                    }

                    //extensão para anexos em dictionary <string,stream>
                    if (dictionaryanexos != null)
                        foreach (var anexo in dictionaryanexos)
                        {
                            System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(anexo.Value, anexo.Key);
                            msg.Attachments.Add(attach);
                        }

                    msg.BodyEncoding = System.Text.Encoding.Default;

                    //(FFS 2014.10.16)
                    if (bodyhtml)
                        msg.IsBodyHtml = true;
                    cliente.Port = port;      // (MA 2009.10.07)
                    cliente.EnableSsl = ssl;  // (MA 2009.10.07)

                    //(JMT 2011.04.04) - Acrescentado to tratar os endereços
                    string[] listaMailCC = cc.Split(new char[] { (';'), (',') });
                    foreach (string mailCC in listaMailCC)
                    {
                        if (validateMail(mailCC))
                        {
                            System.Net.Mail.MailAddress enderecoCC = new System.Net.Mail.MailAddress(mailCC, null, System.Text.Encoding.Default);
                            msg.CC.Add(enderecoCC);
                        }
                    }
                    //

                    //(PR 2012.10.16) - Acrescentado to tratar os endereços em Bcc
                    string[] listaMailBcc = bcc.Split(new char[] { (';'), (',') });
                    foreach (string mailBcc in listaMailBcc)
                    {
                        if (validateMail(mailBcc))
                        {
                            System.Net.Mail.MailAddress enderecoBcc = new System.Net.Mail.MailAddress(mailBcc, null, System.Text.Encoding.Default);
                            msg.Bcc.Add(enderecoBcc);
                        }
                    }

                    if (auth)
                    {
                        cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
                        cliente.Credentials = new NetworkCredential(user, pass);
                    }

                    cliente.Send(msg);
                }
				
                return true;

            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Método que dado um array de strings preenche os destinatario ( DQ - 14072006)
        /// </summary>
        /// <param name="destin"></param>
        public void fillRecipient(object[] destin)
        {
            this.to = "";
            for (int i = 0; i < destin.Length; i++)
            {
                if (validateMail(destin[i].ToString()))
                    this.to += destin[i].ToString() + ",";
            }
            this.to = this.to.Remove(this.to.LastIndexOf(","));
        }

        /// <summary>
        /// English Version - Fills multiple mail destination addresses
        /// </summary>
        /// <param name="destin"></param>
        public void fillDestinations(object[] destin)
        {
            fillRecipient(destin);
        }

        /// <summary>
        /// Método que verifica se o email é válido.
        /// </summary>
        /// <param name="inputEmail"></param>
        public static bool validateMail(string inputEmail)
        {
            string strRegex = @"^[a-zA-Z0-9_+&*-]+(?>\.[a-zA-Z0-9_+&*-]+)*@(?>[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,7}$";
            Regex re = new Regex(strRegex, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        /// <summary>
        /// Método que faz as validações dos parâmetros do email são válidos.
        /// </summary>
        public bool validate()
        {
            if (validateMail(de))
            {
                if (smtpServer.Equals(""))
                    return false;

            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Método que devolve ou coloca o remetente da mensagem
        /// </summary>
        public string From
        {
            get { return de; }
            set { de = value; }
        }


        /// <summary>
        /// Método que devolve ou coloca o(s) destinatários da mensagem
        /// </summary>
        public string To
        {
            get { return to; }
            set { to = value; }
        }


        /// <summary>
        /// Método que devolve ou coloca o subject da mensagem
        /// </summary>          
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }


        /// <summary>
        /// Método que devolve ou coloca o body da mensagem
        /// </summary>
        public string Body
        {
            get { return body; }
            set { body = value; }
        }


        /// <summary>
        /// Método que devolve e coloca a lista de ficheiros anexos
        /// </summary>
        public string[] Attachment
        {
            get { return attachment; }
            set { attachment = value; }
        }

        /// <summary>
        /// Método que devolve e coloca o servidor smtp
        /// </summary>
        public string SmtpServer
        {
            get { return smtpServer; }
            set { smtpServer = value; }
        }

        /// <summary>
        /// Método que define se a ligação é ssl - (MA 2009.10.07)
        /// </summary>
        public bool SSL
        {
            get { return ssl; }
            set { ssl = value; }
        }

        /// <summary>
        /// Método que define a porta smtp - (MA 2009.10.07)
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Método que define se a ligação deve ser autenticada - (MA 2009.10.07)
        /// </summary>
        public bool Auth
        {
            get { return auth; }
            set { auth = value; }
        }
        /// <summary>
        /// Método que devolve e coloca o servidor smtp
        /// </summary>
        public string User
        {
            get { return user; }
            set { user = value; }
        }
        /// <summary>
        /// Método que devolve e coloca o servidor smtp
        /// </summary>
        public string Pass
        {
            get { return pass; }
            set { pass = value; }
        }

        /// <summary>
        /// Método que devolve e coloca os endereços em CC - (JMT 2011.04.04)
        /// </summary>
        public string CC
        {
            get { return cc; }
            set { cc = value; }
        }

        /// <summary>
        /// Método que devolve e coloca o body do e-mail em html - (FFS 2014.10.16)
        /// </summary>
        public bool BodyHtml
        {
            get { return bodyhtml; }
            set { bodyhtml = value; }
        }

        /// <summary>
        /// Método que devolve e coloca os endereços em Bcc - (PR 2012.10.16)
        /// </summary>
        public string Bcc
        {
            get { return bcc; }
            set { bcc = value; }
        }

        /// <summary>
        /// Método que devolve e coloca a pasta da imagem to a assinatura - (SF 2016.02.10)
        /// </summary>
        public string Pathimg
        {
            get { return pathimg; }
            set { pathimg = value; }
        }
        
        /// <summary>
        /// Método que devolve e coloca o text após a imagem da assinatura - (SF 2016.02.10)
        /// </summary>
        public string Textass
        {
            get { return textass; }
            set { textass = value; }
        }

        /// <summary>
        /// Método que devolve e coloca o nome do remetente a apresentar no email
        /// </summary>
        public string NomeRemetente
        {
            get { return nomeremetente; }
            set { nomeremetente = value; }
        }

        /// <summary>
        /// Método que devolve e preenche a lista de imagens a adicionar ao corpo do email
        /// </summary>
        public List<Stream> StreamImagens
        {
            get { return streamimagens; }
            set { streamimagens = value; }
        }

        /// <summary>
        /// Método que devolve e preenche o dicionário de dados com os anexos a adicionar ao email
        /// </summary>
        public Dictionary<string, Stream> DictionaryAnexos
        {
            get { return dictionaryanexos; }
            set { dictionaryanexos = value; }
        }
    }
}