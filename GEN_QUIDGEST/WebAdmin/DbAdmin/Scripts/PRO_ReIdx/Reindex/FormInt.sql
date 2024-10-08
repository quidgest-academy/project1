*{promem}*
GO
*{propais}*
GO
*{userlogin}*
GO
*{asyncprocess}*
USE [W_GnBD]
update [s_apr] set [finished] = (case when ([s_apr].[status]='T' or [s_apr].[status]='AB' or [s_apr].[status]='C') then 1 else 0 end) from [asyncprocess] as [s_apr]
GO
*{notificationemailsignature}*
GO
*{notificationmessage}*
GO
*{procidad}*
GO
*{asyncprocessargument}*
GO
*{asyncprocessattachments}*
GO
*{userauthorization}*
USE [W_GnBD]
update [s_ua] set [naodupli] = dbo.KeyToString([s_ua].[codpsw])+[s_ua].[modulo] from [userauthorization] as [s_ua]
update [s_ua] set [nivel] = dbo.GetLevelFromRole([s_ua].[nivel],[s_ua].[role]) from [userauthorization] as [s_ua]
GO
*{proagente_imobiliario}*
GO
*{propropr}*
USE [W_GnBD]
update [propr] set [idadepro] = year(cast(floor(cast(CURRENT_TIMESTAMP as float)) as datetime))-year([propr].[dtconst]) from [propropr] as [propr]
update [propr] set [lucro] = (case when ([propr].[vendida]=1) then [propr].[preco]*dbo.ansi_N([agent].[perelucr]) else 0 end) from [propropr] as [propr] LEFT JOIN [proagente_imobiliario] as [agent] ON [propr].[codagent] = [agent].[codagent]
GO
*{proalbum}*
GO
*{procontc}*
GO
