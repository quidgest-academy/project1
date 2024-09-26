*{procontc}*
GO
USE [W_GnBD]
GO
*{proalbum}*
GO
USE [W_GnBD]
GO
*{propropr}*
GO
USE [W_GnBD]
GO
*{proagente_imobiliario}*
USE [W_GnBD]
update [proagente_imobiliario] set
     [lucro] = + (select isnull(sum(source.[LUCRO]),0) from [propropr] as source where [proagente_imobiliario].[codagent] = source.[codagent] and source.zzstate = 0)
GO
USE [W_GnBD]
GO
*{userauthorization}*
GO
USE [W_GnBD]
GO
*{asyncprocessattachments}*
GO
USE [W_GnBD]
GO
*{asyncprocessargument}*
GO
USE [W_GnBD]
GO
*{procidad}*
GO
USE [W_GnBD]
GO
*{notificationmessage}*
GO
USE [W_GnBD]
GO
*{notificationemailsignature}*
GO
USE [W_GnBD]
GO
*{asyncprocess}*
GO
USE [W_GnBD]
GO
*{userlogin}*
GO
USE [W_GnBD]
GO
*{propais}*
GO
USE [W_GnBD]
GO
*{promem}*
GO
USE [W_GnBD]
GO
