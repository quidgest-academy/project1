-----------------------------------------------------------
-- MEM
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_MEM', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_MEM AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_MEM 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de MEM
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- MEM
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_MEM', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_MEM AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_MEM 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de MEM
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_MEM', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_MEM AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_MEM @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for MEM
-----------------------------------------------------------
SET NOCOUNT ON;

END')
GO
-----------------------------------------------------------
-- PAIS
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_PAIS', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_PAIS AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_PAIS 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de PAIS
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- PAIS
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_PAIS', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_PAIS AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_PAIS 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de PAIS
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_PAIS', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_PAIS AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_PAIS @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for PAIS
-----------------------------------------------------------
SET NOCOUNT ON;

END')
GO
-----------------------------------------------------------
-- PSW
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_PSW', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_PSW AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_PSW 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de PSW
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- PSW
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_PSW', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_PSW AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_PSW 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de PSW
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_PSW', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_PSW AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_PSW @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for PSW
-----------------------------------------------------------
SET NOCOUNT ON;

UPDATE [psw] set 
	[ATTEMPTS] = COALESCE([psw].[ATTEMPTS], 0)
 ,	[STATUS] = COALESCE([psw].[STATUS], 0)
	FROM [userlogin] AS [psw]

	WHERE ([psw].[codpsw] in (select item from @x))
END')
GO
-----------------------------------------------------------
-- S_APR
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_S_APR', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_S_APR AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_S_APR 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_APR
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
-- Layer 0
update [s_apr]
SET [s_apr].[finished] =(case when ([s_apr].[status]=''T'' or [s_apr].[status]=''AB'' or [s_apr].[status]=''C'') then 1 else 0 end)
FROM [asyncprocess] AS [s_apr]

 where ([s_apr].[codascpr] = @x OR @x IS NULL)
END')
GO
-----------------------------------------------------------
-- S_APR
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_S_APR', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_S_APR AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_S_APR 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_APR
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
-- Layer 0
update [s_apr]
SET [s_apr].[finished] =(case when ([s_apr].[status]=''T'' or [s_apr].[status]=''AB'' or [s_apr].[status]=''C'') then 1 else 0 end)
FROM [asyncprocess] AS [s_apr]

 where ([s_apr].[codascpr] in (select item from @x))
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_S_APR', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_S_APR AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_S_APR @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for S_APR
-----------------------------------------------------------
SET NOCOUNT ON;

UPDATE [s_apr] set 
	[PERCENTA] = COALESCE([s_apr].[PERCENTA], 0)
	FROM [asyncprocess] AS [s_apr]

	WHERE ([s_apr].[codascpr] in (select item from @x))
END')
GO
-----------------------------------------------------------
-- S_NES
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_S_NES', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_S_NES AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_S_NES 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_NES
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- S_NES
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_S_NES', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_S_NES AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_S_NES 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_NES
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_S_NES', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_S_NES AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_S_NES @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for S_NES
-----------------------------------------------------------
SET NOCOUNT ON;

END')
GO
-----------------------------------------------------------
-- S_NM
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_S_NM', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_S_NM AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_S_NM 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_NM
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- S_NM
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_S_NM', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_S_NM AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_S_NM 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_NM
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_S_NM', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_S_NM AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_S_NM @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for S_NM
-----------------------------------------------------------
SET NOCOUNT ON;

END')
GO
-----------------------------------------------------------
-- CIDAD
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_CIDAD', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_CIDAD AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_CIDAD 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de CIDAD
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- CIDAD
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_CIDAD', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_CIDAD AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_CIDAD 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de CIDAD
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_CIDAD', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_CIDAD AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_CIDAD @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for CIDAD
-----------------------------------------------------------
SET NOCOUNT ON;

END')
GO
-----------------------------------------------------------
-- S_ARG
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_S_ARG', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_S_ARG AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_S_ARG 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_ARG
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- S_ARG
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_S_ARG', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_S_ARG AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_S_ARG 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_ARG
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_S_ARG', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_S_ARG AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_S_ARG @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for S_ARG
-----------------------------------------------------------
SET NOCOUNT ON;

END')
GO
-----------------------------------------------------------
-- S_PAX
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_S_PAX', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_S_PAX AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_S_PAX 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_PAX
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- S_PAX
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_S_PAX', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_S_PAX AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_S_PAX 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_PAX
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_S_PAX', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_S_PAX AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_S_PAX @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for S_PAX
-----------------------------------------------------------
SET NOCOUNT ON;

END')
GO
-----------------------------------------------------------
-- S_UA
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_S_UA', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_S_UA AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_S_UA 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_UA
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
-- Layer 0
update [s_ua]
SET [s_ua].[naodupli] =dbo.KeyToString([s_ua].[codpsw])+[s_ua].[modulo]
,[s_ua].[nivel] =dbo.GetLevelFromRole([s_ua].[nivel],[s_ua].[role])
FROM [userauthorization] AS [s_ua]

 where ([s_ua].[codua] = @x OR @x IS NULL)
END')
GO
-----------------------------------------------------------
-- S_UA
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_S_UA', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_S_UA AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_S_UA 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de S_UA
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
-- Layer 0
update [s_ua]
SET [s_ua].[naodupli] =dbo.KeyToString([s_ua].[codpsw])+[s_ua].[modulo]
,[s_ua].[nivel] =dbo.GetLevelFromRole([s_ua].[nivel],[s_ua].[role])
FROM [userauthorization] AS [s_ua]

 where ([s_ua].[codua] in (select item from @x))
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_S_UA', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_S_UA AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_S_UA @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for S_UA
-----------------------------------------------------------
SET NOCOUNT ON;

UPDATE [s_ua] set 
	[NIVEL] = COALESCE([s_ua].[NIVEL], 0)
	FROM [userauthorization] AS [s_ua]

	WHERE ([s_ua].[codua] in (select item from @x))
END')
GO
-----------------------------------------------------------
-- AGENT
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_AGENT', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_AGENT AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_AGENT 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de AGENT
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- AGENT
-- Assumimos que as formulas dos seguintes campos estão bem calculadas:
-----------------------------------------------------------
-- [PROPR -> LUCRO]
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR
update [proagente_imobiliario] set
     [lucro] = + (select isnull(sum(source.[LUCRO]),0) from [propropr] as source where [proagente_imobiliario].[codagent] = source.[codagent] and source.zzstate = 0)
where ([proagente_imobiliario].[codagent] = @x OR @x IS NULL)

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
-- Layer 0
END')
GO
-----------------------------------------------------------
-- AGENT
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_AGENT', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_AGENT AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_AGENT 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de AGENT
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- AGENT
-- Assumimos que as formulas dos seguintes campos estão bem calculadas:
-----------------------------------------------------------
-- [PROPR -> LUCRO]
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR
update [proagente_imobiliario] set
     [lucro] = + (select isnull(sum(source.[LUCRO]),0) from [propropr] as source where [proagente_imobiliario].[codagent] = source.[codagent] and source.zzstate = 0)
where ([proagente_imobiliario].[codagent] in (select item from @x))

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
-- Layer 0
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_AGENT', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_AGENT AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_AGENT @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for AGENT
-----------------------------------------------------------
SET NOCOUNT ON;

UPDATE [agent] set 
	[PERELUCR] = COALESCE([agent].[PERELUCR], 0)
 ,	[LUCRO] = COALESCE([agent].[LUCRO], 0)
	FROM [proagente_imobiliario] AS [agent]

	WHERE ([agent].[codagent] in (select item from @x))
END')
GO
-----------------------------------------------------------
-- PROPR
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_PROPR', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_PROPR AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_PROPR 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de PROPR
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
-- Layer 0
update [propr]
SET [propr].[idadepro] =year(cast(floor(cast(CURRENT_TIMESTAMP as float)) as datetime))-year([propr].[dtconst])
,[propr].[lucro] =(case when ([propr].[vendida]=1) then [propr].[preco]*ISNULL([agent].[perelucr], 0) else 0 end)
FROM [propropr] AS [propr]
LEFT JOIN [proagente_imobiliario] as [agent] ON [propr].[codagent]=[agent].[codagent]

 where ([propr].[codpropr] = @x OR @x IS NULL)
END')
GO
-----------------------------------------------------------
-- PROPR
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_PROPR', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_PROPR AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_PROPR 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de PROPR
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
-- Layer 0
update [propr]
SET [propr].[idadepro] =year(cast(floor(cast(CURRENT_TIMESTAMP as float)) as datetime))-year([propr].[dtconst])
,[propr].[lucro] =(case when ([propr].[vendida]=1) then [propr].[preco]*ISNULL([agent].[perelucr], 0) else 0 end)
FROM [propropr] AS [propr]
LEFT JOIN [proagente_imobiliario] as [agent] ON [propr].[codagent]=[agent].[codagent]

 where ([propr].[codpropr] in (select item from @x))
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_PROPR', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_PROPR AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_PROPR @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for PROPR
-----------------------------------------------------------
SET NOCOUNT ON;

UPDATE [propr] set 
	[PRECO] = COALESCE([propr].[PRECO], 0)
 ,	[TAMANHO] = COALESCE([propr].[TAMANHO], 0)
 ,	[NR_WCS] = COALESCE([propr].[NR_WCS], 0)
 ,	[IDADEPRO] = COALESCE([propr].[IDADEPRO], 0)
 ,	[ESPEXTER] = COALESCE([propr].[ESPEXTER], 0)
 ,	[LUCRO] = COALESCE([propr].[LUCRO], 0)
	FROM [propropr] AS [propr]

	WHERE ([propr].[codpropr] in (select item from @x))
END')
GO
-----------------------------------------------------------
-- ALBUM
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_ALBUM', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_ALBUM AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_ALBUM 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de ALBUM
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- ALBUM
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_ALBUM', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_ALBUM AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_ALBUM 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de ALBUM
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_ALBUM', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_ALBUM AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_ALBUM @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for ALBUM
-----------------------------------------------------------
SET NOCOUNT ON;

END')
GO
-----------------------------------------------------------
-- CONTC
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_Calc_CONTC', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Calc_CONTC AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Calc_CONTC 
@x VARCHAR(50) = NULL
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de CONTC
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
-----------------------------------------------------------
-- CONTC
-----------------------------------------------------------
USE [W_GnBD]
IF OBJECT_ID(N'dbo.Genio_CalcBlock_CONTC', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_CalcBlock_CONTC AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_CalcBlock_CONTC 
@x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Recalcular o registo x de CONTC
-----------------------------------------------------------
SET NOCOUNT ON;
-----------------------------------------------------------
-- Formulas externas podem ser calculadas primeiro e de forma agregada
-----------------------------------------------------------
-- SR

-- UV
-----------------------------------------------------------
-- Formulas internas podem depender umas das outras
-----------------------------------------------------------
END')
GO
IF OBJECT_ID(N'dbo.Genio_Default_CONTC', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.Genio_Default_CONTC AS SET NOCOUNT ON;')

EXEC('
ALTER PROCEDURE dbo.Genio_Default_CONTC @x KeyListType READONLY
AS BEGIN
-----------------------------------------------------------
-- Defaults for CONTC
-----------------------------------------------------------
SET NOCOUNT ON;

END')
GO
