



		
 
USE [W_GnBD]

if exists(SELECT top 1 name FROM sysobjects where name = 'Codigos_Int_Modulos')
	DROP TABLE Codigos_Int_Modulos
if exists(SELECT top 1 name FROM sysobjects where name = 'Codigos_Internos')
	DROP TABLE Codigos_Internos
	declare @NewTable as bit;
	exec CriarTabelaTmp 'Codigos_Sequenciais','id_objecto','0', 50, @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'Codigos_Sequenciais','PRO','id_objecto'
	exec AlterarCamposTmp 'Codigos_Sequenciais', 'id_objecto', 'varchar', 'VARCHAR(50) NOT NULL', 50
	if not exists (select * from sysobjects where xtype = 'PK' and parent_obj = (select id from sysobjects where name = 'Codigos_Sequenciais'))
		ALTER TABLE Codigos_Sequenciais ADD CONSTRAINT CODIGOS_SEQUENCIAIS_PK PRIMARY KEY(id_objecto)
	exec AlterarCamposTmp 'Codigos_Sequenciais', 'proximo', 'bigint', 'BIGINT', '10'
GO

 
declare @NewTable as bit;
exec CriarTabelaTmp 'PROmem','codmem','I', 8, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROmem','PRO','codmem'
exec AlterarCamposTmp 'PROmem', 'codmem', 'int', 'INT NOT NULL', '8'
if (@NewTable = 1)
	ALTER TABLE [PROmem] ADD CONSTRAINT [PROMEM_CODMEM__] PRIMARY KEY(codmem)
exec AlterarCamposTmp 'PROmem', 'login', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROmem', 'altura', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROmem', 'rotina', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROmem', 'obs', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROmem', 'hostid', 'varchar', 'VARCHAR(20)', '20'
exec AlterarCamposTmp 'PROmem', 'clientid', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROmem', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO
declare @NewTable as bit;
exec CriarTabelaTmp 'PROcfg','codcfg','I', 6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROcfg','PRO','codcfg'
exec AlterarCamposTmp 'PROcfg', 'codcfg', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROcfg] ADD CONSTRAINT [PROCFG_CODCFG__] PRIMARY KEY(codcfg)
exec AlterarCamposTmp 'PROcfg', 'checkdat', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROcfg', 'versao', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROcfg', 'versindx', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROcfg', 'manutdat', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROcfg', 'upgrindx', 'int', 'INT', '5'
exec AlterarCamposTmp 'PROcfg', 'usrsetv', 'int', 'INT', '5'
exec AlterarCamposTmp 'PROcfg', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROtblcfg','CODTBLCFG','I', 8 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROtblcfg','PRO','CODTBLCFG'
exec AlterarCamposTmp 'PROtblcfg', 'CODTBLCFG', 'int', 'INT NOT NULL', '8'
if (@NewTable = 1)
	ALTER TABLE [PROtblcfg] ADD CONSTRAINT [PROTBLCFG_CODTBLCFG] PRIMARY KEY(CODTBLCFG)

exec AlterarCamposTmp 'PROtblcfg', 'CODPSW', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROtblcfg', 'UUID', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtblcfg', 'NAME', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtblcfg', 'CONFIGID', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROtblcfg', 'CONFIG', 'varchar', 'VARCHAR(MAX)', 'MAX'
exec AlterarCamposTmp 'PROtblcfg', 'DATE', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROtblcfg', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROtblcfgsel','CODTBLCFGSEL','I', 8 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROtblcfgsel','PRO','CODTBLCFGSEL'
exec AlterarCamposTmp 'PROtblcfgsel', 'CODTBLCFGSEL', 'int', 'INT NOT NULL', '8'
if (@NewTable = 1)
	ALTER TABLE [PROtblcfgsel] ADD CONSTRAINT [PROTBLCFGSEL_CODTBLCFGSEL] PRIMARY KEY(CODTBLCFGSEL)

exec AlterarCamposTmp 'PROtblcfgsel', 'CODPSW', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROtblcfgsel', 'UUID', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtblcfgsel', 'CODTBLCFG', 'int', 'INT', '6'
exec AlterarCamposTmp 'PROtblcfgsel', 'DATE', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROtblcfgsel', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROlstusr','CODLSTUSR','I', 8 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROlstusr','PRO','CODLSTUSR'
exec AlterarCamposTmp 'PROlstusr', 'CODLSTUSR', 'int', 'INT NOT NULL', '8'
if (@NewTable = 1)
	ALTER TABLE [PROlstusr] ADD CONSTRAINT [PROLSTUSR_CODLSTUSR] PRIMARY KEY(CODLSTUSR)

exec AlterarCamposTmp 'PROlstusr', 'CODPSW', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROlstusr', 'DESCRIC', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROlstusr', 'IDLIST', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROlstusr', 'MODULO', 'varchar', 'VARCHAR(3)', '3'
exec AlterarCamposTmp 'PROlstusr', 'SISTEMA', 'varchar', 'VARCHAR(3)', '3'
exec AlterarCamposTmp 'PROlstusr', 'ORDERCOL', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROlstusr', 'ORDERTYPE', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROlstusr', 'DATA', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROlstusr', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROlstcol','CODLSTCOL','I', 8 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROlstcol','PRO','CODLSTCOL'
exec AlterarCamposTmp 'PROlstcol', 'CODLSTCOL', 'int', 'INT NOT NULL', '8'
if (@NewTable = 1)
	ALTER TABLE [PROlstcol] ADD CONSTRAINT [PROLSTCOL_CODLSTCOL] PRIMARY KEY(CODLSTCOL)
exec AlterarCamposTmp 'PROlstcol', 'CODLSTUSR', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROlstcol', 'TABELA', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROlstcol', 'ALIAS', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROlstcol', 'CAMPO', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROlstcol', 'VISIVEL', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROlstcol', 'POSICAO', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROlstcol', 'OPERACAO', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROlstcol', 'TIPO', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROlstcol', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROlstren','CODLSTREN','I', 8 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROlstren','PRO','CODLSTREN'
exec AlterarCamposTmp 'PROlstren', 'CODLSTREN', 'int', 'INT NOT NULL', '8'
if (@NewTable = 1)
	ALTER TABLE [PROlstren] ADD CONSTRAINT [PROLSTCOL_CODLSTREN] PRIMARY KEY(CODLSTREN)
exec AlterarCamposTmp 'PROlstren', 'CODLSTUSR', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROlstren', 'RENDERIZACAO', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROlstren', 'VISIVEL', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROlstren', 'POSICAO', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROlstren', 'OPERACAO', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROlstren', 'TIPO', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROlstren', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROusrwid','CODUSRWID','I', 8 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROusrwid','PRO','CODUSRWID'
exec AlterarCamposTmp 'PROusrwid', 'CODUSRWID', 'int', 'INT NOT NULL', '8'
if (@NewTable = 1)
	ALTER TABLE [PROusrwid] ADD CONSTRAINT [PROLSTCOL_CODUSRWID] PRIMARY KEY(CODUSRWID)
exec AlterarCamposTmp 'PROusrwid', 'CODLSTUSR', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROusrwid', 'WIDGET', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROusrwid', 'ROWKEY', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROusrwid', 'VISIBLE', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROusrwid', 'HPOSITION', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROusrwid', 'VPOSITION', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROusrwid', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROusrcfg','codusrcfg','I', 6 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROusrcfg','PRO','codusrcfg'

exec AlterarCamposTmp 'PROusrcfg', 'codusrcfg', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROusrcfg] ADD CONSTRAINT [PROUSRCFG_CODUSRCFG] PRIMARY KEY(codusrcfg)
exec AlterarCamposTmp 'PROusrcfg', 'modulo', 'varchar', 'VARCHAR(3)', '3'

exec AlterarCamposTmp 'PROusrcfg', 'codpsw', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROusrcfg', 'tipo', 'varchar', 'VARCHAR(2)', '2'
exec AlterarCamposTmp 'PROusrcfg', 'id', 'varchar', 'VARCHAR(15)', '15'
exec AlterarCamposTmp 'PROusrcfg', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROusrset','codusrset','I', 6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROusrset','PRO','codusrset'

exec AlterarCamposTmp 'PROusrset', 'codusrset', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROusrset] ADD CONSTRAINT [PROUSRSET_CODUSRSET] PRIMARY KEY(codusrset)
exec AlterarCamposTmp 'PROusrset', 'modulo', 'varchar', 'VARCHAR(3)', '3'
exec AlterarCamposTmp 'PROusrset', 'codpsw', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROusrset', 'chave', 'varchar', 'VARCHAR(30)', '30'
exec AlterarCamposTmp 'PROusrset', 'valor', 'varchar', 'VARCHAR(4000)', '4000'
exec AlterarCamposTmp 'PROusrset', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROwkfact','codwkfact','I',  6 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROwkfact','PRO','codwkfact'
exec AlterarCamposTmp 'PROwkfact', 'codwkfact', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROwkfact] ADD CONSTRAINT [PROWKFACT_CODWKFACT] PRIMARY KEY(codwkfact)
exec AlterarCamposTmp 'PROwkfact', 'wfid', 'varchar', 'VARCHAR(6)', '6'
exec AlterarCamposTmp 'PROwkfact', 'actid', 'int', 'INT', ''
exec AlterarCamposTmp 'PROwkfact', 'tpactid', 'varchar', 'VARCHAR(6)', '6'
exec AlterarCamposTmp 'PROwkfact', 'descrica', 'varchar', 'VARCHAR(40)', '40'
exec AlterarCamposTmp 'PROwkfact', 'duracao', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkfact', 'perfil', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkfact', 'perfilu', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkfact', 'x', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkfact', 'y', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkfact', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROwkfcon','codwkfcon','I', 6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROwkfcon','PRO','codwkfcon'
exec AlterarCamposTmp 'PROwkfcon', 'codwkfcon', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROwkfcon] ADD CONSTRAINT [PROWKFCON_CODWKFCON] PRIMARY KEY(codwkfcon)
exec AlterarCamposTmp 'PROwkfcon', 'wfid', 'varchar', 'VARCHAR(6)', '6'
exec AlterarCamposTmp 'PROwkfcon', 'condid', 'int', 'INT', ''
exec AlterarCamposTmp 'PROwkfcon', 'tpcondid', 'varchar', 'VARCHAR(6)', '6'
exec AlterarCamposTmp 'PROwkfcon', 'descrica', 'varchar', 'VARCHAR(40)', '40'
exec AlterarCamposTmp 'PROwkfcon', 'tiporegr', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkfcon', 'sinal', 'varchar', 'VARCHAR(6)', '6'
exec AlterarCamposTmp 'PROwkfcon', 'x', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkfcon', 'y', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkfcon', 'valor', 'varchar', 'VARCHAR(30)', '30'
exec AlterarCamposTmp 'PROwkfcon', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROwkflig','codwkflig','I',6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROwkflig','PRO','codwkflig'
exec AlterarCamposTmp 'PROwkflig', 'codwkflig', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROwkflig] ADD CONSTRAINT [PROWKFLIG_CODWKFLIG] PRIMARY KEY(codwkflig)
exec AlterarCamposTmp 'PROwkflig', 'wfid', 'varchar', 'VARCHAR(6)', '6'
exec AlterarCamposTmp 'PROwkflig', 'parentid', 'int', 'INT', '6'
exec AlterarCamposTmp 'PROwkflig', 'childid', 'int', 'INT', '6'
exec AlterarCamposTmp 'PROwkflig', 'ptoy', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkflig', 'ptox', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkflig', 'ptor', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkflig', 'tipo', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkflig', 'pfromx', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkflig', 'pfromy', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkflig', 'pfromr', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROwkflig', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROwkflow','codwkflow','I', 6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROwkflow','PRO','codwkflow'
exec AlterarCamposTmp 'PROwkflow', 'codwkflow', 'int', 'INT NOT NULL', ''
if (@NewTable = 1)
	ALTER TABLE [PROwkflow] ADD CONSTRAINT [PROWKFLOW_CODWKFLOW] PRIMARY KEY(codwkflow)
exec AlterarCamposTmp 'PROwkflow', 'descrica', 'varchar', 'VARCHAR(40)', '40'
exec AlterarCamposTmp 'PROwkflow', 'modulo', 'varchar', 'VARCHAR(3)', '3'
exec AlterarCamposTmp 'PROwkflow', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROnotifi','codnotifi','I', 6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROnotifi','PRO','codnotifi'
exec AlterarCamposTmp 'PROnotifi', 'codnotifi', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROnotifi] ADD CONSTRAINT [PRONOTIFI_CODNOTIFI] PRIMARY KEY(codnotifi)
exec AlterarCamposTmp 'PROnotifi', 'modulo', 'varchar', 'VARCHAR(3)', '3'
exec AlterarCamposTmp 'PROnotifi', 'descrica', 'varchar', 'VARCHAR(120)', '120'
exec AlterarCamposTmp 'PROnotifi', 'activid', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROnotifi', 'menuid', 'varchar', 'VARCHAR(15)', '15'
exec AlterarCamposTmp 'PROnotifi', 'usernivl', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROnotifi', 'wfid', 'varchar', 'VARCHAR(6)', '6'
exec AlterarCamposTmp 'PROnotifi', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROprmfrm','codprmfrm','I',6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROprmfrm','PRO','codprmfrm'
exec AlterarCamposTmp 'PROprmfrm', 'codprmfrm', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROprmfrm] ADD CONSTRAINT [PROPRMFRM_CODPRMFRM] PRIMARY KEY(codprmfrm)
exec AlterarCamposTmp 'PROprmfrm', 'desform', 'varchar', 'VARCHAR(8)', '8'
exec AlterarCamposTmp 'PROprmfrm', 'perfil', 'varchar', 'VARCHAR(2)', '2'
exec AlterarCamposTmp 'PROprmfrm', 'autoriza', 'varchar', 'VARCHAR(1)', '1'
exec AlterarCamposTmp 'PROprmfrm', 'sevalida', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROprmfrm', 'prfvalid', 'varchar', 'VARCHAR(2)', '2'
exec AlterarCamposTmp 'PROprmfrm', 'prazodia', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROprmfrm', 'comprova', 'varchar', 'VARCHAR(30)', '30'
exec AlterarCamposTmp 'PROprmfrm', 'prazohor', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROprmfrm', 'secompro', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROprmfrm', 'mensag1', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROprmfrm', 'mensag2', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROprmfrm', 'mensag3', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROprmfrm', 'mensag4', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROprmfrm', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROscrcrd','codscrcrd','I', 6 ,  @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROscrcrd','PRO','codscrcrd'
exec AlterarCamposTmp 'PROscrcrd', 'codscrcrd', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROscrcrd] ADD CONSTRAINT [PROSCRCRD_CODSCRCRD] PRIMARY KEY(codscrcrd)
exec AlterarCamposTmp 'PROscrcrd', 'descrica', 'varchar', 'VARCHAR(60)', '60'
exec AlterarCamposTmp 'PROscrcrd', 'valor', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROscrcrd', 'cor', 'varchar', 'VARCHAR(1)', '1'
exec AlterarCamposTmp 'PROscrcrd', 'seta', 'varchar', 'VARCHAR(1)', '1'
exec AlterarCamposTmp 'PROscrcrd', 'usernivl', 'float', 'FLOAT(53)', '53'
exec AlterarCamposTmp 'PROscrcrd', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

if exists(select name from sysobjects where xtype = 'u' and name = 'PROdocums')
BEGIN
declare @NewTable as bit;
	exec CriarTabelaTmp 'PROdocums','coddocums','I', 6 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'PROdocums','PRO','coddocums'

	exec AlterarCamposTmp 'PROdocums', 'coddocums', 'int', 'INT NOT NULL', '8'
	exec AlinharCmpDireitaTmp 'PROdocums', 'coddocums', '8', 'FF_DIREITA', 'dbText'
	if (not exists(select * from sysobjects where xtype = 'PK' and parent_obj = (select id from sysobjects where name = 'PROdocums') and name = 'PRODOCUMS_CODDOCUMS'))
		ALTER TABLE [PROdocums] ADD CONSTRAINT [PRODOCUMS_CODDOCUMS] PRIMARY KEY(coddocums)
	
	exec AlterarCamposTmp 'PROdocums', 'documid', 'int', 'INT', '8'

--Caso a versão anterior tenha sido gerada para sql2005, a coluna document será do tipo IMAGE.
--Ao passar de uma versão para a outra, forçar a filestream numa coluna já existente dá erro por limitação do próprio sql.
--Tem de ser criada uma nova coluna e os dados passados para ela
--Dropar a image e rename a nova coluna para document
IF (SELECT [DATA_TYPE] FROM [INFORMATION_SCHEMA].[COLUMNS] WHERE [TABLE_NAME] = 'PROdocums' AND [COLUMN_NAME] = 'document') = 'image'
BEGIN
	SET ANSI_WARNINGS OFF;
	exec sp_RENAME 'PROdocums.document', 'document_temp' , 'COLUMN';
	exec AlterarCamposTmp 'PROdocums', 'document', 'varbinary', 'VARBINARY(MAX)', '';
	EXECUTE sp_executesql N'UPDATE [PROdocums] set document = document_temp; ALTER TABLE [PROdocums] DROP COLUMN document_temp;';
	SET ANSI_WARNINGS ON;
END
ELSE
BEGIN 
	IF (SELECT [DATA_TYPE] FROM [INFORMATION_SCHEMA].[COLUMNS] WHERE [TABLE_NAME] = 'PROdocums' AND [COLUMN_NAME] = 'document') = 'varchar'
	BEGIN
		SET ANSI_WARNINGS OFF	;
		exec sp_RENAME 'PROdocums.document', 'document_temp' , 'COLUMN';
		exec AlterarCamposTmp 'PROdocums', 'document', 'varbinary', 'VARBINARY(MAX)', '';
		exec AlterarCamposTmp 'PROdocums', 'docpath', 'varchar', 'VARCHAR(260)', '260';
		EXECUTE sp_executesql N'UPDATE [PROdocums] set docpath = document_temp; ALTER TABLE [PROdocums] DROP COLUMN document_temp;';
	END
	ELSE
	BEGIN
		exec AlterarCamposTmp 'PROdocums', 'document', 'varbinary', 'VARBINARY(MAX)', ''
	END
END

exec AlterarCamposTmp 'PROdocums', 'docpath', 'varchar', 'VARCHAR(260)', '260'

	exec AlterarCamposTmp 'PROdocums', 'tabela', 'varchar', 'VARCHAR(30)', '30'
	exec AlterarCamposTmp 'PROdocums', 'campo', 'varchar', 'VARCHAR(30)', '30'
	exec AlterarCamposTmp 'PROdocums', 'chave', 'varchar', 'VARCHAR(36)', '36'
	exec AlterarCamposTmp 'PROdocums', 'form', 'varchar', 'VARCHAR(8)', '8'
	exec AlterarCamposTmp 'PROdocums', 'nome', 'varchar', 'VARCHAR(255)', '255'
	exec AlterarCamposTmp 'PROdocums', 'versao', 'varchar', 'VARCHAR(10)', '10'
	exec AlterarCamposTmp 'PROdocums', 'tamanho', 'float', 'FLOAT(53)', '10'
	exec AlterarCamposTmp 'PROdocums', 'extensao', 'varchar', 'VARCHAR(5)', '5'
	exec AlterarCamposTmp 'PROdocums', 'opercria', 'varchar', 'VARCHAR(100)', '100'
	exec AlterarCamposTmp 'PROdocums', 'datacria', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'PROdocums', 'opermuda', 'varchar', 'VARCHAR(100)', '100'
	exec AlterarCamposTmp 'PROdocums', 'datamuda', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'PROdocums', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
END
GO

--A tabela docums deixou de ter o prefixo do sistema, e passa a chamar-se apenas "docums" de forma a poder ser reutilizada por sistemas partilhados.
--Caso esta bd tenha mais do que um sistema, é necessário migrar os documentos de todos os sistemas, excepto do primeiro que reindexar a bd. Ou seja,
--o primeiro faz o rename da tabela (visto não mudar nada a nivel da estrutura da tabela) e os restantes fazem inserts.
--EDIT:Adaptei e retirei o código do ficheiro ActStoredProcFIM e coloquei aqui de forma a conseguir evitar migração / duplicação de dados para BD's com um único sistema.
--Como este script corre primeiro que o ActStoredProcFIM, a docums já vai estar criada nessa altura, com os respectivos indices e filestream, pelo que apagar a docums e renomear a PRODocums 
--iria originar uma segunda reindexação para corrigir os indices.
IF EXISTS(SELECT top 1 name FROM sysobjects WHERE name = 'PROdocums')
BEGIN
    IF EXISTS(SELECT top 1 name FROM sysobjects WHERE name = 'docums')
    BEGIN
        INSERT INTO DOCUMS ([coddocums],[documid],[document],[docpath],[tabela],[campo],[chave],[form],[nome],[versao],[tamanho],[extensao],[opercria],[datacria],[opermuda],[datamuda],[ZZSTATE])
        SELECT [coddocums],[documid],[document],[docpath],[tabela],[campo],[chave],[form],[nome],[versao],[tamanho],[extensao],[opercria],[datacria],[opermuda],[datamuda],[ZZSTATE] FROM [PRODOCUMS]
        EXEC sp_rename 'PROdocums', 'backupPROdocums'
    END
    ELSE
    BEGIN
        EXEC sp_rename 'PROdocums', 'docums'
        EXEC sp_rename 'PROdocums_CODDOCUMS', 'DOCUMS_CODDOCUMS'
    END
END
GO

	declare @NewTable as bit;
	exec CriarTabelaTmp 'docums','coddocums','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'docums','PRO','coddocums'

	exec AlterarCamposTmp 'docums', 'coddocums', 'int', 'INT NOT NULL', '8'
	if (not exists(select * from sysobjects where xtype = 'PK' and parent_obj = (select id from sysobjects where name = 'docums') and name = 'DOCUMS_CODDOCUMS'))
		ALTER TABLE docums ADD CONSTRAINT [DOCUMS_CODDOCUMS] PRIMARY KEY(coddocums)
	
	exec AlterarCamposTmp 'docums', 'documid', 'int', 'INT', '8'

--Caso a versão anterior tenha sido gerada para sql2005, a coluna document será do tipo IMAGE.
--Ao passar de uma versão para a outra, forçar a filestream numa coluna já existente dá erro por limitação do próprio sql.
--Tem de ser criada uma nova coluna e os dados passados para ela
--Dropar a image e rename a nova coluna para document
IF (SELECT [DATA_TYPE] FROM [INFORMATION_SCHEMA].[COLUMNS] WHERE [TABLE_NAME] = 'docums' AND [COLUMN_NAME] = 'document') = 'image'
BEGIN
	SET ANSI_WARNINGS OFF	;
	exec sp_RENAME 'docums.document', 'document_temp' , 'COLUMN';
	exec AlterarCamposTmp 'docums', 'document', 'varbinary', 'VARBINARY(MAX)', '';
	EXECUTE sp_executesql N'UPDATE docums set document = document_temp; ALTER TABLE docums DROP COLUMN document_temp;';
	SET ANSI_WARNINGS ON;
END
ELSE
BEGIN 
	IF (SELECT [DATA_TYPE] FROM [INFORMATION_SCHEMA].[COLUMNS] WHERE [TABLE_NAME] = 'docums' AND [COLUMN_NAME] = 'document') = 'varchar' or (SELECT [DATA_TYPE] FROM [INFORMATION_SCHEMA].[COLUMNS] WHERE [TABLE_NAME] = 'docums' AND [COLUMN_NAME] = 'document') = 'nvarchar'
	BEGIN
		SET ANSI_WARNINGS OFF	;
		exec sp_RENAME 'docums.document', 'document_temp' , 'COLUMN';
		exec AlterarCamposTmp 'docums', 'document', 'varbinary', 'VARBINARY(MAX)', '';
		exec AlterarCamposTmp 'docums', 'docpath', 'varchar', 'VARCHAR(260)', '260';
		EXECUTE sp_executesql N'UPDATE docums set docpath = document_temp; ALTER TABLE docums DROP COLUMN document_temp;';
	END
	ELSE
	BEGIN
		exec AlterarCamposTmp 'docums', 'document', 'varbinary', 'VARBINARY(MAX)', ''
	END
END

	exec AlterarCamposTmp 'docums', 'docpath', 'varchar', 'VARCHAR(260)', '260'

	exec AlterarCamposTmp 'docums', 'tabela', 'varchar', 'VARCHAR(30)', '30'
	exec AlterarCamposTmp 'docums', 'campo', 'varchar', 'VARCHAR(30)', '30'
	exec AlterarCamposTmp 'docums', 'chave', 'varchar', 'VARCHAR(36)', '36'
	exec AlterarCamposTmp 'docums', 'form', 'varchar', 'VARCHAR(8)', '8'
	exec AlterarCamposTmp 'docums', 'nome', 'varchar', 'VARCHAR(255)', '255'
	exec AlterarCamposTmp 'docums', 'versao', 'varchar', 'VARCHAR(10)', '10'
	exec AlterarCamposTmp 'docums', 'tamanho', 'float', 'FLOAT(53)', '10'
	exec AlterarCamposTmp 'docums', 'extensao', 'varchar', 'VARCHAR(5)', '5'
	exec AlterarCamposTmp 'docums', 'opercria', 'varchar', 'VARCHAR(100)', '100'
	exec AlterarCamposTmp 'docums', 'datacria', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'docums', 'opermuda', 'varchar', 'VARCHAR(100)', '100'
	exec AlterarCamposTmp 'docums', 'datamuda', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'docums', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROpostit','codpostit','I', 6,  @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROpostit','PRO','codpostit'
exec AlterarCamposTmp 'PROpostit', 'codpostit', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROpostit] ADD CONSTRAINT [PROPOSTIT_CODPOSTIT] PRIMARY KEY(codpostit)
exec AlterarCamposTmp 'PROpostit', 'tabela', 'varchar', 'VARCHAR(6)', '6'
exec AlterarCamposTmp 'PROpostit', 'codtabel', 'varchar', 'VARCHAR(32)', '32'
exec AlterarCamposTmp 'PROpostit', 'postit', 'varchar', 'VARCHAR(255)', '255'
exec AlterarCamposTmp 'PROpostit', 'tpostit', 'varchar', 'VARCHAR(1)', '1'
exec AlterarCamposTmp 'PROpostit', 'datacria', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROpostit', 'opercria', 'varchar', 'VARCHAR(100)', '100'

exec AlterarCamposTmp 'PROpostit', 'codpsw', 'int', 'int', '8'
exec AlterarCamposTmp 'PROpostit', 'lido', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROpostit', 'apagado', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROpostit', 'validade', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROpostit', 'nivel', 'float', 'FLOAT(53)', '53'

exec AlterarCamposTmp 'PROpostit', 'codpost1', 'int', 'INT', '6'
exec AlterarCamposTmp 'PROpostit', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'hashcd','codhashcd','I', 6 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'hashcd','PRO','codhashcd'
exec AlterarCamposTmp 'hashcd', 'codhashcd', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE hashcd ADD CONSTRAINT [HASHCD_CODHASHCD] PRIMARY KEY(codhashcd)
exec AlterarCamposTmp 'hashcd', 'tabela', 'varchar', 'VARCHAR(30)', '30'
exec AlterarCamposTmp 'hashcd', 'campos', 'varchar', 'VARCHAR(900)', '900'
exec AlterarCamposTmp 'hashcd', 'datacria', 'datetime', 'DATETIME', '6'
exec AlterarCamposTmp 'hashcd', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROalerta','codalerta','I', 6 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROalerta','PRO','codalerta'
exec AlterarCamposTmp 'PROalerta', 'codalerta', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROalerta] ADD CONSTRAINT [PROALERTA_CODALERTA] PRIMARY KEY(codalerta)
exec AlterarCamposTmp 'PROalerta', 'codaltent', 'int', 'INT', '6'
exec AlterarCamposTmp 'PROalerta', 'mensagem', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROalerta', 'tratado', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROalerta', 'activo', 'float', 'FLOAT(53)', '1'
exec AlterarCamposTmp 'PROalerta', 'datacria', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROalerta', 'datareso', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROalerta', 'menu', 'varchar', 'VARCHAR(20)', '20'
exec AlterarCamposTmp 'PROalerta', 'cor', 'varchar', 'VARCHAR(1)', '1'
exec AlterarCamposTmp 'PROalerta', 'interno', 'float', 'FLOAT(53)', '1'
exec AlterarCamposTmp 'PROalerta', 'backgrou', 'int', 'INT', '1'
exec AlterarCamposTmp 'PROalerta', 'sms', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROalerta', 'email', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROalerta', 'emailenv', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROalerta', 'smsenvia', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROalerta', 'codigo', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROalerta', 'codigotp', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROalerta', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROaltent','codaltent','I', 6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROaltent','PRO','codaltent'
exec AlterarCamposTmp 'PROaltent', 'codaltent', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROaltent] ADD CONSTRAINT [PROALTENT_CODALTENT] PRIMARY KEY(codaltent)
exec AlterarCamposTmp 'PROaltent', 'codtalert', 'int', 'INT', '6'

exec AlterarCamposTmp 'PROaltent', 'codpsw', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROaltent', 'grupo', 'varchar', 'VARCHAR(30)', '30'
exec AlterarCamposTmp 'PROaltent', 'contador', 'float', 'FLOAT(53)', '5'
exec AlterarCamposTmp 'PROaltent', 'tipo', 'varchar', 'VARCHAR(1)', '1'
exec AlterarCamposTmp 'PROaltent', 'mensagem', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROaltent', 'antecede', 'float', 'FLOAT(53)', '3'
exec AlterarCamposTmp 'PROaltent', 'datainic', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROaltent', 'datafina', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROaltent', 'dtmodifi', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROaltent', 'todos', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROaltent', 'backgrou', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROaltent', 'sms', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROaltent', 'email', 'tinyint', 'TINYINT', '1'

exec AlterarCamposTmp 'PROaltent', 'codtpgru', 'int', 'INT  NOT NULL ', '8'
exec AlterarCamposTmp 'PROaltent', 'codtpess', 'int', 'INT  NOT NULL ', '8'

exec AlterarCamposTmp 'PROaltent', 'menu', 'varchar', 'VARCHAR(20)', '20'
exec AlterarCamposTmp 'PROaltent', 'incluian', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROaltent', 'activo', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROaltent', 'individu', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROaltent', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROtalert','codtalert','I',  6 , @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROtalert','PRO','codtalert'
exec AlterarCamposTmp 'PROtalert', 'codtalert', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROtalert] ADD CONSTRAINT [PROTALERT_CODTALERT] PRIMARY KEY(codtalert)
exec AlterarCamposTmp 'PROtalert', 'nome', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtalert', 'metodo', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtalert', 'priorida', 'varchar', 'VARCHAR(1)', '1'
exec AlterarCamposTmp 'PROtalert', 'cor', 'varchar', 'VARCHAR(1)', '1'
exec AlterarCamposTmp 'PROtalert', 'texto', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtalert', 'menu', 'varchar', 'VARCHAR(20)', '20'
exec AlterarCamposTmp 'PROtalert', 'cmpnome', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtalert', 'daltinic', 'varchar', 'VARCHAR(20)', '20'
exec AlterarCamposTmp 'PROtalert', 'daltfina', 'varchar', 'VARCHAR(20)', '20'
exec AlterarCamposTmp 'PROtalert', 'incluian', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROtalert', 'diferenc', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROtalert', 'anodifer', 'float', 'FLOAT(53)', '2'
exec AlterarCamposTmp 'PROtalert', 'mesdifer', 'float', 'FLOAT(53)', '2'
exec AlterarCamposTmp 'PROtalert', 'diadifer', 'float', 'FLOAT(53)', '2'
exec AlterarCamposTmp 'PROtalert', 'ntabmae', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtalert', 'ntabfilh', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtalert', 'formid', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtalert', 'tabela', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtalert', 'campo', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROtalert', 'tipo', 'float', 'FLOAT(53)', '4'
exec AlterarCamposTmp 'PROtalert', 'modulo', 'varchar', 'VARCHAR(5)', '5'
exec AlterarCamposTmp 'PROtalert', 'condicao', 'varchar', 'VARCHAR(8000)', '8000'
exec AlterarCamposTmp 'PROtalert', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROdelega','coddelega','I',  6,  @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROdelega','PRO','coddelega'
exec AlterarCamposTmp 'PROdelega', 'coddelega', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROdelega] ADD CONSTRAINT [PRODELEGA_CODDELEGA] PRIMARY KEY(coddelega)
exec AlterarCamposTmp 'PROdelega', 'codpswup', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROdelega', 'codpswdw', 'int', 'INT', '8'
exec AlterarCamposTmp 'PROdelega', 'dateini', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROdelega', 'dateend', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROdelega', 'message', 'varchar', 'VARCHAR(4000)', '4000'
exec AlterarCamposTmp 'PROdelega', 'revoked', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROdelega', 'auditusr', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROdelega', 'opercria', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROdelega', 'datacria', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROdelega', 'opermuda', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROdelega', 'datamuda', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROdelega', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO



declare @NewTable as bit;
exec CriarTabelaTmp 'PROTABDINAMIC','codtabdinamic','I', 6,  @NewTable OUTPUT
exec AlterarCamposTmp 'PROTABDINAMIC', 'NOMETABELA', 'varchar', 'VARCHAR(25)', '25'
exec AlterarCamposTmp 'PROTABDINAMIC', 'NOMECAMPO', 'varchar', 'VARCHAR(25)', '25'
exec AlterarCamposTmp 'PROTABDINAMIC', 'TIPODADOS', 'varchar', 'VARCHAR(15)', '15'
exec AlterarCamposTmp 'PROTABDINAMIC', 'TIPOSQL', 'varchar', 'VARCHAR(15)', '15'
exec AlterarCamposTmp 'PROTABDINAMIC', 'LARGURA', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROTABDINAMIC', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

USE [W_GnBD]
if not exists(SELECT top 1 name FROM sysobjects where name = 'logPROpro')
begin
	EXEC ('CREATE TABLE logPROpro ( [Login] [varchar](100) NULL ) ')
end
exec ApagarTodosIndicesTmp 'logPROpro','PRO'
exec AlterarCamposTmp 'logPROpro', 'Login', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'logPROpro', 'Data', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'logPROpro', 'Accao', 'varchar', 'VARCHAR(200)', '200'
GO



declare @NewTable as bit;
exec CriarTabelaTmp 'PROaltran','codaltran','I', 6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROaltran','PRO','codaltran'

exec AlterarCamposTmp 'PROaltran', 'codaltran', 'int', 'INT NOT NULL', '16'
if (@NewTable = 1)
	ALTER TABLE [PROaltran] ADD CONSTRAINT [PROALTRAN_CODALTRAN] PRIMARY KEY(codaltran)
exec AlterarCamposTmp 'PROaltran', 'typlabel', 'varchar', 'VARCHAR(1)', '1'
exec AlterarCamposTmp 'PROaltran', 'referenc', 'varchar', 'VARCHAR(500)', '500'
exec AlterarCamposTmp 'PROaltran', 'language', 'varchar', 'VARCHAR(2)', '2'
exec AlterarCamposTmp 'PROaltran', 'curlabel', 'varchar', 'VARCHAR(500)', '500'
exec AlterarCamposTmp 'PROaltran', 'labellng', 'varchar', 'VARCHAR(500)', '500'
exec AlterarCamposTmp 'PROaltran', 'altran', 'varchar', 'VARCHAR(500)', '500'
exec AlterarCamposTmp 'PROaltran', 'opercria', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROaltran', 'datacria', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROaltran', 'opermuda', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROaltran', 'datamuda', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROaltran', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO



declare @NewTable as bit;
exec CriarTabelaTmp 'PROworkflowtask','codtask','I',  6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROworkflowtask','PRO','codtask'
exec AlterarCamposTmp 'PROworkflowtask', 'codtask', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROworkflowtask] ADD CONSTRAINT [PROWORKFLOWTASK_CODTASK_] PRIMARY KEY(codtask)
exec AlterarCamposTmp 'PROworkflowtask', 'codprcess', 'int', 'INT', '6'

exec AlterarCamposTmp 'PROworkflowtask', 'taskdefid', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROworkflowtask', 'taskid', 'varchar', 'VARCHAR(8000)', '8000'
exec AlterarCamposTmp 'PROworkflowtask', 'performedby', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROworkflowtask', 'runningsince', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROworkflowtask', 'nextrun', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROworkflowtask', 'modifieddate', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROworkflowtask', 'modifiedby', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROworkflowtask', 'skipped', 'varchar', 'VARCHAR(30)', '30'
exec AlterarCamposTmp 'PROworkflowtask', 'isactive', 'varchar', 'VARCHAR(30)', '30'
exec AlterarCamposTmp 'PROworkflowtask', 'errorExecute', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROworkflowtask', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO



declare @NewTable as bit;
exec CriarTabelaTmp 'PROworkflowprocess','codprcess','I',  6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROworkflowprocess','PRO','codprcess'
exec AlterarCamposTmp 'PROworkflowprocess', 'codprcess', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROworkflowprocess] ADD CONSTRAINT [PROWORKFLOWPROCESS_CODPRCESS] PRIMARY KEY(codprcess)
exec AlterarCamposTmp 'PROworkflowprocess', 'prcessid', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROworkflowprocess', 'prcessdefid', 'varchar', 'VARCHAR(8000)', '8000'
exec AlterarCamposTmp 'PROworkflowprocess', 'createdby', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'PROworkflowprocess', 'status', 'varchar', 'VARCHAR(30)', '30'
exec AlterarCamposTmp 'PROworkflowprocess', 'startdate', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROworkflowprocess', 'modifieddate', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'PROworkflowprocess', 'modifiedby', 'varchar', 'VARCHAR(100)', '100'

exec AlterarCamposTmp 'PROworkflowprocess', 'codinstance', 'int', 'INT', '6'

exec AlterarCamposTmp 'PROworkflowprocess', 'dotgraph', 'varchar', 'VARCHAR(8000)', '8000'
exec AlterarCamposTmp 'PROworkflowprocess', 'wferror', 'int', 'INT', '2'
exec AlterarCamposTmp 'PROworkflowprocess', 'ltask', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROworkflowprocess', 'idhk', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROworkflowprocess', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

--Se por alguma razão ainda não existe a tabela de logs, cria-a
IF OBJECT_ID('dbo.logPROall', 'U') IS NULL
BEGIN
  USE [W_GnBD]
	CREATE TABLE [dbo].[logPROall](
	  [COD] [varchar](38) NOT NULL,
	  [LOGTABLE] [varchar](50) NULL,
		[LOGFIELD] [varchar](50) NULL,
		[OP] [char](1) NOT NULL,
		[VAL] [varchar](MAX) NULL,
		[DATE] [datetime] NOT NULL,
		[WHO] [varchar](50) NULL
	)
END
GO

IF OBJECT_ID('dbo.PswBlacklist', 'U') IS NULL
BEGIN
	USE [W_GnBD]
	CREATE TABLE PswBlacklist (
		pass VARCHAR(64) PRIMARY KEY CLUSTERED WITH (IGNORE_DUP_KEY = on)
	)
END
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'PROalerts','codalerts','I',6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'PROalerts','PRO','codalerts'
exec AlterarCamposTmp 'PROalerts', 'Codalerts', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [PROalerts] ADD CONSTRAINT [PROALERTS_CODALERTS]  PRIMARY KEY(codalerts)
exec AlterarCamposTmp 'PROalerts', 'Codpsw', 'int', 'INT', '6'
exec AlterarCamposTmp 'PROalerts', 'Content', 'varchar', 'VARCHAR(8000)', '8000'
exec AlterarCamposTmp 'PROalerts', 'Datacria', 'datetime', 'DATETIME', '8' 
exec AlterarCamposTmp 'PROalerts', 'Datamuda', 'datetime', 'DATETIME', '8' 
exec AlterarCamposTmp 'PROalerts', 'Delay', 'int', 'INT', '5' 
exec AlterarCamposTmp 'PROalerts', 'Dismissable', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROalerts', 'Idalert', 'varchar','VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROalerts', 'Inactive', 'tinyint', 'TINYINT', '1'
exec AlterarCamposTmp 'PROalerts', 'Modulo', 'varchar', 'VARCHAR(10)', '10'
exec AlterarCamposTmp 'PROalerts', 'Nivel', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'PROalerts', 'Title', 'varchar', 'VARCHAR(500)', '500'
exec AlterarCamposTmp 'PROalerts', 'Type', 'int', 'INT', '1' --enum: success, info, warning, danger
exec AlterarCamposTmp 'PROalerts', 'URL', 'varchar', 'VARCHAR(8000)', '8000'
exec AlterarCamposTmp 'PROalerts', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'ReportList','codreport','I', 6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'ReportList','PRO','codreport'
exec AlterarCamposTmp 'ReportList', 'codreport', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [ReportList] ADD CONSTRAINT [REPORTLIST_CODREPORT] PRIMARY KEY(codreport)
exec AlterarCamposTmp 'ReportList', 'REPORT', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'ReportList', 'SLOTID', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'ReportList', 'TITULO', 'varchar', 'VARCHAR(120)', '120'
exec AlterarCamposTmp 'ReportList', 'OPERCRIA', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'ReportList', 'DATACRIA', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'ReportList', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO

declare @NewTable as bit;
exec CriarTabelaTmp 'Cavreport','codreport','I', 6, @NewTable OUTPUT
if (@NewTable = 0)
	exec ApagarTodosIndicesTmp 'Cavreport','PRO','codreport'
exec AlterarCamposTmp 'Cavreport', 'codreport', 'int', 'INT NOT NULL', '6'
if (@NewTable = 1)
	ALTER TABLE [Cavreport] ADD CONSTRAINT [CAVREPORT_CODREPORT] PRIMARY KEY(codreport)
exec AlterarCamposTmp 'Cavreport', 'TITLE', 'varchar', 'VARCHAR(200)', '200'
exec AlterarCamposTmp 'Cavreport', 'ACESSO', 'varchar', 'VARCHAR(50)', '50'
exec AlterarCamposTmp 'Cavreport', 'DATAXML', 'varchar', 'VARCHAR(MAX)', '8000'
exec AlterarCamposTmp 'Cavreport', 'OPERCRIA', 'varchar', 'VARCHAR(100)', '100'
exec AlterarCamposTmp 'Cavreport', 'DATACRIA', 'datetime', 'DATETIME', '8'
exec AlterarCamposTmp 'Cavreport', 'ZZSTATE', 'tinyint', 'TINYINT', '1'
GO


