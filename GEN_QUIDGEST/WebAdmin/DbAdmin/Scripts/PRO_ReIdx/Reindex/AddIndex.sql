USE [W_GnBD]
if (32 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'PROMEM_CODMEM__', 'codmem', 1, 1, 0, 1
 
exec CheckIdxTmp @tabela, 'promem', 'PROMEM_CODMEM__', 'pro';
end
GO
if (41 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'PROPAIS_CODPAIS_', 'codpais', 1, 1, 0, 1
 UNION ALL
-- Index with origin: Tabela NotDup PAIS
SELECT 'PROPAIS_C12B4941FE33488C', 'pais', 0, 1, 0, 0

exec CheckIdxTmp @tabela, 'propais', 'PROPAIS_CODPAIS_', 'pro';
end
GO
if (33 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'USERLOGIN_CODPSW__', 'codpsw', 1, 1, 0, 1
UNION ALL
SELECT 'USERLOGIN_nome', 'nome', 0, 1, 0, 0
 
exec CheckIdxTmp @tabela, 'userlogin', 'USERLOGIN_CODPSW__', 'pro';
end
GO
if (37 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'ASYNCPROCESS_CODASCPR', 'codascpr', 1, 1, 0, 1
 
exec CheckIdxTmp @tabela, 'asyncprocess', 'ASYNCPROCESS_CODASCPR', 'pro';
end
GO
if (36 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'NOTIFICATIONEMAILSIGNATURE_CODSIGNA', 'codsigna', 1, 1, 0, 1
 
exec CheckIdxTmp @tabela, 'notificationemailsignature', 'NOTIFICATIONEMAILSIGNATURE_CODSIGNA', 'pro';
end
GO
if (35 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'NOTIFICATIONMESSAGE_CODMESGS', 'codmesgs', 1, 1, 0, 1
 
exec CheckIdxTmp @tabela, 'notificationmessage', 'NOTIFICATIONMESSAGE_CODMESGS', 'pro';
end
GO
if (41 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'PROCIDAD_CODCIDAD', 'codcidad', 1, 1, 0, 1
 UNION ALL
-- Index with origin: Menu PRO_321
SELECT 'PROCIDAD_89508A31C13D4341', 'cidade', 0, 1, 0, 0

exec CheckIdxTmp @tabela, 'procidad', 'PROCIDAD_CODCIDAD', 'pro';
end
GO
if (38 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'ASYNCPROCESSARGUMENT_CODARGPR', 'codargpr', 1, 1, 0, 1
 
exec CheckIdxTmp @tabela, 'asyncprocessargument', 'ASYNCPROCESSARGUMENT_CODARGPR', 'pro';
end
GO
if (39 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'ASYNCPROCESSATTACHMENTS_CODPRANX', 'codpranx', 1, 1, 0, 1
 
exec CheckIdxTmp @tabela, 'asyncprocessattachments', 'ASYNCPROCESSATTACHMENTS_CODPRANX', 'pro';
end
GO
if (34 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'USERAUTHORIZATION_CODUA___', 'codua', 1, 1, 0, 1
 UNION ALL
-- Index with origin: Tabela NotDup S_UA
SELECT 'PROS_UA_8225A1F309C44CD9', 'naodupli', 0, 1, 0, 0
UNION ALL
-- Index with origin: Tabela NotDup S_UA
SELECT 'PROS_UA_8225A1F309C44CD9', 'role', 0, 2, 0, 0

exec CheckIdxTmp @tabela, 'userauthorization', 'USERAUTHORIZATION_CODUA___', 'pro';
end
GO
if (41 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'PROAGENT_CODAGENT', 'codagent', 1, 1, 0, 1
 UNION ALL
-- Index with origin: Tabela NotDup AGENT
SELECT 'PROAGENT_A8BCC9952DE8477F', 'email', 0, 1, 0, 0

exec CheckIdxTmp @tabela, 'proagente_imobiliario', 'PROAGENT_CODAGENT', 'pro';
end
GO
if (41 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'PROPROPR_CODPROPR', 'codpropr', 1, 1, 0, 1
 UNION ALL
-- Index with origin: Formula SR AGENT
SELECT 'PROPROPR_30B90C825F824CED', 'codagent', 0, 1, 0, 0
UNION ALL
SELECT 'PROPROPR_30B90C825F824CED','zzState', 0, 0, 1, 0
UNION ALL
-- Index with origin: Menu PRO_411
SELECT 'PROPROPR_6B6A9D789A6A47AF', 'titulo', 0, 1, 0, 0
UNION ALL
-- Index with origin: Menu PRO_411
SELECT 'PROPROPR_6B6A9D789A6A47AF', 'codagent', 0, 2, 0, 0
UNION ALL
SELECT 'PROPROPR_6B6A9D789A6A47AF','vendida', 0, 0, 1, 0

exec CheckIdxTmp @tabela, 'propropr', 'PROPROPR_CODPROPR', 'pro';
end
GO
if (41 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'PROALBUM_CODALBUM', 'codalbum', 1, 1, 0, 1
 
exec CheckIdxTmp @tabela, 'proalbum', 'PROALBUM_CODALBUM', 'pro';
end
GO
if (41 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
set nocount on
DECLARE @tabela as IndexCheckType;

 INSERT INTO @tabela (idxName, columnName, uniqueIdx, ordem, include, pk)
SELECT 'PROCONTC_CODCONTC', 'codcontc', 1, 1, 0, 1
 
exec CheckIdxTmp @tabela, 'procontc', 'PROCONTC_CODCONTC', 'pro';
end
GO

if (41 > isnull((select versindx from PROcfg),0) or 'W_GnZeroTrue'='1')
begin
-- Indíces de tabelas hardcoded (continuam da forma antiga [apagar e criar])
if (exists(select top 1 name from sys.indexes where name = 'proalerta_alerta01'))
	DROP INDEX proalerta_alerta01 ON proalerta
CREATE INDEX proalerta_alerta01 ON proalerta (codaltent)

if (exists(select top 1 name from sys.indexes where name = 'propostit_postit01'))
	DROP INDEX propostit_postit01 ON propostit
CREATE INDEX propostit_postit01 ON propostit (tabela,codtabel)

if (exists(select top 1 name from sys.indexes where name = 'propostit_postit02'))
	DROP INDEX propostit_postit02 ON propostit
CREATE INDEX propostit_postit02 ON propostit (codtabel)

if (exists(select top 1 name from sys.indexes where name = 'propostit_postit03'))
	DROP INDEX propostit_postit03 ON propostit
CREATE INDEX propostit_postit03 ON propostit (postit)

if (exists(select top 1 name from sys.indexes where name = 'propostit_postit04'))
	DROP INDEX propostit_postit04 ON propostit
CREATE INDEX propostit_postit04 ON propostit (tpostit)

if (exists(select top 1 name from sys.indexes where name = 'propostit_postit05'))
	DROP INDEX propostit_postit05 ON propostit
CREATE INDEX propostit_postit05 ON propostit (codpsw)


if (exists(select top 1 name from sys.indexes where name = 'propostit_postit06'))
	DROP INDEX propostit_postit06 ON propostit
CREATE INDEX propostit_postit06 ON propostit (nivel)

if (exists(select top 1 name from sys.indexes where name = 'propostit_postit07'))
	DROP INDEX propostit_postit07 ON propostit
CREATE INDEX propostit_postit07 ON propostit (codpost1)

if (exists(select top 1 name from sys.indexes where name = 'proaltent_altent01'))
	DROP INDEX proaltent_altent01 ON proaltent
CREATE INDEX proaltent_altent01 ON proaltent (codtalert,tipo)

if (exists(select top 1 name from sys.indexes where name = 'prodelega_delega01'))
	DROP INDEX prodelega_delega01 ON prodelega
CREATE INDEX prodelega_delega01 ON prodelega (coddelega,codpswup)

if (exists(select top 1 name from sys.indexes where name = 'prodelega_delega02'))
	DROP INDEX prodelega_delega02 ON prodelega
CREATE INDEX prodelega_delega02 ON prodelega (coddelega,codpswdw)

if (exists(select top 1 name from sys.indexes where name = 'docums_docums01'))
	DROP INDEX docums_docums01 ON docums
CREATE INDEX docums_docums01 ON docums (documid,versao)

if (exists(select top 1 name from sys.indexes where name = 'docums_docums02'))
	DROP INDEX docums_docums02 ON docums
CREATE INDEX docums_docums02 ON docums (nome)


if (exists(select top 1 name from sys.indexes where name = 'docums_docums03'))
	DROP INDEX docums_docums03 ON docums
CREATE INDEX docums_docums03 ON docums (tabela,ZZSTATE)

if (exists(select top 1 name from sys.indexes where name = 'prolstusr_codpsw'))
	DROP INDEX prolstusr_codpsw ON prolstusr
CREATE INDEX prolstusr_codpsw ON prolstusr (codpsw, descric)

if (exists(select top 1 name from sys.indexes where name = 'prolstcol_codlstusr'))
	DROP INDEX prolstcol_codlstusr ON prolstcol
CREATE INDEX prolstcol_codlstusr ON prolstcol (codlstusr)

if (exists(select top 1 name from sys.indexes where name = 'prolstren_codlstusr'))
	DROP INDEX prolstren_codlstusr ON prolstren
CREATE INDEX prolstren_codlstusr ON prolstren (codlstusr)

if (exists(select top 1 name from sys.indexes where name = 'prousrwid_codlstusr'))
	DROP INDEX prousrwid_codlstusr ON prousrwid
CREATE INDEX prousrwid_codlstusr ON prousrwid (codlstusr)

if (exists(select top 1 name from sys.indexes where name = 'protblcfg_codpsw_uuid_name'))
	DROP INDEX protblcfg_codpsw_uuid_name ON protblcfg
CREATE INDEX protblcfg_codpsw_uuid_name ON protblcfg (codpsw, uuid, name)

if (exists(select top 1 name from sys.indexes where name = 'protblcfgsel_codpsw_uuid'))
	DROP INDEX protblcfgsel_codpsw_uuid ON protblcfgsel
CREATE INDEX protblcfgsel_codpsw_uuid ON protblcfgsel (codpsw, uuid)



if (exists(select top 1 name from sys.indexes where name = 'logproall_logproall01'))
	DROP INDEX logproall_logproall01 ON logproall
CREATE CLUSTERED INDEX [logproall_logproall01] ON [dbo].[logPROall]([LOGTABLE] ASC, [LOGFIELD] ASC, [OP] ASC,[DATE] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


UPDATE PROcfg set versindx = 41
end
GO