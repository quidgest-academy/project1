
-- recalculo de fórmulas para a tabela PROCONTC
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO
-- recalculo de fórmulas para a tabela PROALBUM
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO
-- recalculo de fórmulas para a tabela PROPROPR
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
-- Layer 0
update [propr]
SET [propr].[idadepro] =year(cast(floor(cast(CURRENT_TIMESTAMP as float)) as datetime))-year([propr].[dtconst])
,[propr].[lucro] =(case when ([propr].[vendida]=1) then [propr].[preco]*ISNULL([agent].[perelucr], 0) else 0 end)
FROM [propropr] AS [propr]
LEFT JOIN [proagente_imobiliario] as [agent] ON [propr].[codagent]=[agent].[codagent]


GO
GO
-- recalculo de fórmulas para a tabela PROagente_imobiliario
USE [W_GnBD]
GO
USE [W_GnBD]
GO
USE [W_GnBD]
update [proagente_imobiliario] set
     [lucro] = + (select isnull(sum(source.[LUCRO]),0) from [propropr] as source where [proagente_imobiliario].[codagent] = source.[codagent] and source.zzstate = 0)
GO
USE [W_GnBD]
GO
-- Layer 0
GO
GO
-- recalculo de fórmulas para a tabela UserAuthorization
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
-- Layer 0
update [s_ua]
SET [s_ua].[naodupli] =dbo.KeyToString([s_ua].[codpsw])+[s_ua].[modulo]
,[s_ua].[nivel] =dbo.GetLevelFromRole([s_ua].[nivel],[s_ua].[role])
FROM [userauthorization] AS [s_ua]


GO
GO
-- recalculo de fórmulas para a tabela AsyncProcessAttachments
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO
-- recalculo de fórmulas para a tabela AsyncProcessArgument
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO
-- recalculo de fórmulas para a tabela PROCIDAD
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO
-- recalculo de fórmulas para a tabela NotificationMessage
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO
-- recalculo de fórmulas para a tabela NotificationEmailSignature
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO
-- recalculo de fórmulas para a tabela AsyncProcess
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
-- Layer 0
update [s_apr]
SET [s_apr].[finished] =(case when ([s_apr].[status]='T' or [s_apr].[status]='AB' or [s_apr].[status]='C') then 1 else 0 end)
FROM [asyncprocess] AS [s_apr]


GO
GO
-- recalculo de fórmulas para a tabela UserLogin
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO
-- recalculo de fórmulas para a tabela PROPAIS
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO
-- recalculo de fórmulas para a tabela PROMEM
USE [W_GnBD]
GO
USE [W_GnBD]
GO
GO
USE [W_GnBD]
GO
GO
GO


GO