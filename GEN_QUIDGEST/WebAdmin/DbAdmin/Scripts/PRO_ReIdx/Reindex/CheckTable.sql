USE [W_GnBD]
if ('W_GnZeroTrue'='1')
begin
    IF((SELECT COUNT(1) FROM [PROcfg]) > 0)
        UPDATE [PROcfg] SET versindx = 0
end
GO
IF ('W_GnZeroTrue'='1' AND EXISTS (SELECT * FROM [W_GnBD].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ASYNCPROCESS'))
BEGIN
if (not exists(SELECT CODASCPR FROM ASYNCPROCESS ))
begin	
	DROP TABLE ASYNCPROCESS	
end
END
GO
IF ('W_GnZeroTrue'='1' AND EXISTS (SELECT * FROM [W_GnBD].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ASYNCPROCESSARGUMENT'))
BEGIN
if (not exists(SELECT CODARGPR FROM ASYNCPROCESSARGUMENT ))
begin
	DROP TABLE ASYNCPROCESSARGUMENT	
end
END
GO
IF ('W_GnZeroTrue'='1' AND EXISTS (SELECT * FROM [W_GnBD].INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ASYNCPROCESSATTACHMENTS'))
BEGIN
if (not exists(SELECT CODPRANX FROM ASYNCPROCESSATTACHMENTS ))
begin
	DROP TABLE ASYNCPROCESSATTACHMENTS	
end
END
GO



if ('W_GnZeroTrue'='1' OR 2533 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'USERLOGIN','CODPSW','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'USERLOGIN','PRO','CODPSW'
	exec AlterarCamposTmp 'USERLOGIN', 'CODPSW', 'int', 'INT NOT NULL', '4'
   	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'USERLOGIN' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [USERLOGIN] ADD CONSTRAINT [USERLOGIN_CODPSW__] PRIMARY KEY(CODPSW)
	exec AlterarCamposTmp 'USERLOGIN', 'NOME', 'varchar', 'VARCHAR(100)', '100'
	exec AlterarCamposTmp 'USERLOGIN', 'PASSWORD', 'varchar', 'VARCHAR(150)', '150'
	exec AlterarCamposTmp 'USERLOGIN', 'CERTSN', 'varchar', 'VARCHAR(32)', '32'
	exec AlterarCamposTmp 'USERLOGIN', 'EMAIL', 'varchar', 'VARCHAR(254)', '254'
	exec AlterarCamposTmp 'USERLOGIN', 'PSWTYPE', 'varchar', 'VARCHAR(3)', '3'
	exec AlterarCamposTmp 'USERLOGIN', 'SALT', 'varchar', 'VARCHAR(32)', '32'
	exec AlterarCamposTmp 'USERLOGIN', 'DATAPSW', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'USERLOGIN', 'USERID', 'varchar', 'VARCHAR(250)', '250'
	exec AlterarCamposTmp 'USERLOGIN', 'PSW2FAVL', 'varchar', 'VARCHAR(1000)', '1000'
	exec AlterarCamposTmp 'USERLOGIN', 'PSW2FATP', 'varchar', 'VARCHAR(16)', '16'
	exec AlterarCamposTmp 'USERLOGIN', 'DATEXP', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'USERLOGIN', 'ATTEMPTS', 'smallint', 'SMALLINT', '2'
	exec AlterarCamposTmp 'USERLOGIN', 'PHONE', 'varchar', 'VARCHAR(16)', '16'
	exec AlterarCamposTmp 'USERLOGIN', 'STATUS', 'smallint', 'SMALLINT', '2'
	exec AlterarCamposTmp 'USERLOGIN', 'ASSOCIA', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'USERLOGIN', 'OPERCRIA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'USERLOGIN', 'DATACRIA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'USERLOGIN', 'OPERMUDA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'USERLOGIN', 'DATAMUDA', 'datetime', 'DATETIME', '8'
  exec AlterarCamposTmp 'USERLOGIN', 'PSWTYPE', 'varchar', 'VARCHAR(3)', '3'
  exec AlterarCamposTmp 'USERLOGIN', 'SALT', 'varchar', 'VARCHAR(32)', '32'
  exec AlterarCamposTmp 'USERLOGIN', 'DATAPSW', 'datetime', 'DATETIME', '8'
  exec AlterarCamposTmp 'USERLOGIN', 'USERID', 'varchar', 'VARCHAR(250)', '250'
  exec AlterarCamposTmp 'USERLOGIN', 'PSW2FAVL', 'varchar', 'VARCHAR(1000)', '1000'
  exec AlterarCamposTmp 'USERLOGIN', 'PSW2FATP', 'varchar', 'VARCHAR(16)', '16'
  exec AlterarCamposTmp 'USERLOGIN', 'EMAIL', 'varchar', 'VARCHAR(254)', '254'
  exec AlterarCamposTmp 'USERLOGIN', 'DATEXP', 'datetime', 'DATETIME', '8'
  exec AlterarCamposTmp 'USERLOGIN', 'ATTEMPTS', 'int', 'INT', '2'
  exec AlterarCamposTmp 'USERLOGIN', 'PHONE', 'varchar', 'VARCHAR(16)', '16'
  exec AlterarCamposTmp 'USERLOGIN', 'STATUS', 'int', 'INT', '2'
	exec AlterarCamposTmp 'USERLOGIN', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2534 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'USERAUTHORIZATION','CODUA','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'USERAUTHORIZATION','PRO','CODUA'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'CODUA', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'USERAUTHORIZATION' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [USERAUTHORIZATION] ADD CONSTRAINT [USERAUTHORIZATION_CODUA___] PRIMARY KEY(CODUA)
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'CODPSW', 'int', 'INT', '4'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'SISTEMA', 'varchar', 'VARCHAR(20)', '20'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'MODULO', 'varchar', 'VARCHAR(3)', '3'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'NAODUPLI', 'varchar', 'VARCHAR(39)', '39'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'ROLE', 'varchar', 'VARCHAR(16)', '16'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'NIVEL', 'bigint', 'BIGINT', '8'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'OPERCRIA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'DATACRIA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'OPERMUDA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'DATAMUDA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'USERAUTHORIZATION', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2535 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'NOTIFICATIONMESSAGE','CODMESGS','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'NOTIFICATIONMESSAGE','PRO','CODMESGS'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'CODMESGS', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'NOTIFICATIONMESSAGE' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [NOTIFICATIONMESSAGE] ADD CONSTRAINT [NOTIFICATIONMESSAGE_CODMESGS] PRIMARY KEY(CODMESGS)
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'CODSIGNA', 'varchar', 'VARCHAR(50)', '50'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'CODPMAIL', 'varchar', 'VARCHAR(50)', '50'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'FROM', 'varchar', 'VARCHAR(254)', '254'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'CODTPNOT', 'varchar', 'VARCHAR(50)', '50'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'CODDESTN', 'varchar', 'VARCHAR(50)', '50'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'TO', 'varchar', 'VARCHAR(254)', '254'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'DESTNMAN', 'tinyint', 'TINYINT', '1'
 	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'TOMANUAL', 'varchar', 'VARCHAR(MAX)', '8000'
 	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'CC', 'varchar', 'VARCHAR(MAX)', '8000'
 	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'BCC', 'varchar', 'VARCHAR(MAX)', '8000'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'IDNOTIF', 'varchar', 'VARCHAR(100)', '100'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'NOTIFICA', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'EMAIL', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'ASSUNTO', 'varchar', 'VARCHAR(100)', '100'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'AGREGADO', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'ANEXO', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'HTML', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'ATIVO', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'DESIGNAC', 'varchar', 'VARCHAR(100)', '100'
 	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'MENSAGEM', 'varchar', 'VARCHAR(MAX)', '8000'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'GRAVABD', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'OPERCRIA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'DATACRIA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'OPERMUDA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'DATAMUDA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'NOTIFICATIONMESSAGE', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2536 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'NOTIFICATIONEMAILSIGNATURE','CODSIGNA','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'NOTIFICATIONEMAILSIGNATURE','PRO','CODSIGNA'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'CODSIGNA', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'NOTIFICATIONEMAILSIGNATURE' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [NOTIFICATIONEMAILSIGNATURE] ADD CONSTRAINT [NOTIFICATIONEMAILSIGNATURE_CODSIGNA] PRIMARY KEY(CODSIGNA)
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'NAME', 'varchar', 'VARCHAR(100)', '100'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'IMAGE', 'varbinary', 'VARBINARY(MAX)', '3'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'TEXTASS', 'varchar', 'VARCHAR(300)', '300'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'USERNAME', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'PASSWORD', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'OPERCRIA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'DATACRIA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'OPERMUDA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'DATAMUDA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'NOTIFICATIONEMAILSIGNATURE', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2537 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'ASYNCPROCESS','CODASCPR','G', 16 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'ASYNCPROCESS','PRO','CODASCPR'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'CODASCPR', 'uniqueidentifier', 'uniqueidentifier NOT NULL', '16'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'ASYNCPROCESS' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [ASYNCPROCESS] ADD CONSTRAINT [ASYNCPROCESS_CODASCPR] PRIMARY KEY(CODASCPR)
	exec AlterarCamposTmp 'ASYNCPROCESS', 'TYPE', 'varchar', 'VARCHAR(12)', '12'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'DATEREQU', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'INITPRC', 'datetime', 'DATETIME', '16'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'ENDPRC', 'datetime', 'DATETIME', '16'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'DURATION', 'varchar', 'VARCHAR(5)', '5'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'STATUS', 'varchar', 'VARCHAR(2)', '2'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'RSLTMSG', 'varchar', 'VARCHAR(250)', '250'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'FINISHED', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'LASTUPDT', 'datetime', 'DATETIME', '19'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'RESULT', 'varchar', 'VARCHAR(2)', '2'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'INFO', 'varchar', 'VARCHAR(500)', '500'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'PERCENTA', 'smallint', 'SMALLINT', '2'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'MODOPROC', 'varchar', 'VARCHAR(9)', '9'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'EXTERNAL', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'ID', 'int', 'INT', '4'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'CODENTIT', 'int', 'INT', '4'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'MOTIVO', 'varchar', 'VARCHAR(200)', '200'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'CODPSW', 'int', 'INT', '4'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'OPERSHUT', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'OPERCRIA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'DATACRIA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'OPERMUDA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'DATAMUDA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'ASYNCPROCESS', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2538 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'ASYNCPROCESSARGUMENT','CODARGPR','G', 16 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'ASYNCPROCESSARGUMENT','PRO','CODARGPR'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'CODARGPR', 'uniqueidentifier', 'uniqueidentifier NOT NULL', '16'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'ASYNCPROCESSARGUMENT' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [ASYNCPROCESSARGUMENT] ADD CONSTRAINT [ASYNCPROCESSARGUMENT_CODARGPR] PRIMARY KEY(CODARGPR)
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'CODS_APR', 'uniqueidentifier', 'uniqueidentifier', '16'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'ID', 'varchar', 'VARCHAR(50)', '50'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'VALOR', 'varchar', 'VARCHAR(250)', '250'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'DOCUMENTFK', 'int', 'int', '16'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'DOCUMENT', 'varchar', 'VARCHAR(200)', '200'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'TIPO', 'varchar', 'VARCHAR(250)', '250'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'DESIGNAC', 'varchar', 'VARCHAR(200)', '200'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'HIDDEN', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'OPERCRIA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'DATACRIA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'OPERMUDA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'DATAMUDA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'ASYNCPROCESSARGUMENT', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2539 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'ASYNCPROCESSATTACHMENTS','CODPRANX','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'ASYNCPROCESSATTACHMENTS','PRO','CODPRANX'
	exec AlterarCamposTmp 'ASYNCPROCESSATTACHMENTS', 'CODPRANX', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'ASYNCPROCESSATTACHMENTS' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [ASYNCPROCESSATTACHMENTS] ADD CONSTRAINT [ASYNCPROCESSATTACHMENTS_CODPRANX] PRIMARY KEY(CODPRANX)
	exec AlterarCamposTmp 'ASYNCPROCESSATTACHMENTS', 'CODS_APR', 'uniqueidentifier', 'uniqueidentifier', '16'
	exec AlterarCamposTmp 'ASYNCPROCESSATTACHMENTS', 'DOCUMENTFK', 'int', 'int', '16'
	exec AlterarCamposTmp 'ASYNCPROCESSATTACHMENTS', 'DOCUMENT', 'varchar', 'VARCHAR(200)', '200'
	exec AlterarCamposTmp 'ASYNCPROCESSATTACHMENTS', 'OPERCRIA', 'varchar', 'VARCHAR(128)', '128'
	exec AlterarCamposTmp 'ASYNCPROCESSATTACHMENTS', 'DATACRIA', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'ASYNCPROCESSATTACHMENTS', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2541 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'PROAGENTE_IMOBILIARIO','CODAGENT','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'PROAGENTE_IMOBILIARIO','PRO','CODAGENT'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'CODAGENT', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'PROAGENTE_IMOBILIARIO' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [PROAGENTE_IMOBILIARIO] ADD CONSTRAINT [PROAGENTE_IMOBILIARIO_CODAGENT] PRIMARY KEY(CODAGENT)
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'FOTO', 'varbinary', 'VARBINARY(MAX)', '3'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'NOME', 'varchar', 'VARCHAR(80)', '80'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'DNASCIME', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'EMAIL', 'varchar', 'VARCHAR(80)', '80'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'TELEFONE', 'varchar', 'VARCHAR(14)', '14'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'CODPMORA', 'int', 'INT', '4'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'CODPNASC', 'int', 'INT', '4'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'PERELUCR', 'real', 'REAL', '4'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'LUCRO', 'float', 'FLOAT', '8'
	exec AlterarCamposTmp 'PROAGENTE_IMOBILIARIO', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2541 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'PROALBUM','CODALBUM','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'PROALBUM','PRO','CODALBUM'
	exec AlterarCamposTmp 'PROALBUM', 'CODALBUM', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'PROALBUM' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [PROALBUM] ADD CONSTRAINT [PROALBUM_CODALBUM] PRIMARY KEY(CODALBUM)
	exec AlterarCamposTmp 'PROALBUM', 'FOTO', 'varbinary', 'VARBINARY(MAX)', '3'
	exec AlterarCamposTmp 'PROALBUM', 'TITULO', 'varchar', 'VARCHAR(50)', '50'
	exec AlterarCamposTmp 'PROALBUM', 'CODPROPR', 'int', 'INT', '4'
	exec AlterarCamposTmp 'PROALBUM', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2541 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'PROCIDAD','CODCIDAD','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'PROCIDAD','PRO','CODCIDAD'
	exec AlterarCamposTmp 'PROCIDAD', 'CODCIDAD', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'PROCIDAD' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [PROCIDAD] ADD CONSTRAINT [PROCIDAD_CODCIDAD] PRIMARY KEY(CODCIDAD)
	exec AlterarCamposTmp 'PROCIDAD', 'CIDADE', 'varchar', 'VARCHAR(50)', '50'
	exec AlterarCamposTmp 'PROCIDAD', 'CODPAIS', 'int', 'INT', '4'
	exec AlterarCamposTmp 'PROCIDAD', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2541 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'PROCONTC','CODCONTC','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'PROCONTC','PRO','CODCONTC'
	exec AlterarCamposTmp 'PROCONTC', 'CODCONTC', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'PROCONTC' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [PROCONTC] ADD CONSTRAINT [PROCONTC_CODCONTC] PRIMARY KEY(CODCONTC)
	exec AlterarCamposTmp 'PROCONTC', 'DTCONTAT', 'datetime', 'DATETIME', '8'
	exec AlterarCamposTmp 'PROCONTC', 'CODPROPR', 'int', 'INT', '4'
	exec AlterarCamposTmp 'PROCONTC', 'CLTNAME', 'varchar', 'VARCHAR(50)', '50'
	exec AlterarCamposTmp 'PROCONTC', 'CLTEMAIL', 'varchar', 'VARCHAR(80)', '80'
	exec AlterarCamposTmp 'PROCONTC', 'TELEFONE', 'varchar', 'VARCHAR(14)', '14'
 	exec AlterarCamposTmp 'PROCONTC', 'DESCRIIC', 'varchar', 'VARCHAR(MAX)', '8000'
	exec AlterarCamposTmp 'PROCONTC', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2541 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'PROPAIS','CODPAIS','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'PROPAIS','PRO','CODPAIS'
	exec AlterarCamposTmp 'PROPAIS', 'CODPAIS', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'PROPAIS' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [PROPAIS] ADD CONSTRAINT [PROPAIS_CODPAIS_] PRIMARY KEY(CODPAIS)
	exec AlterarCamposTmp 'PROPAIS', 'PAIS', 'varchar', 'VARCHAR(50)', '50'
	exec AlterarCamposTmp 'PROPAIS', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

if ('W_GnZeroTrue'='1' OR 2541 > isnull((select versao from [PROcfg]),0))
begin
	declare @NewTable as bit;
	exec CriarTabelaTmp 'PROPROPR','CODPROPR','I', 8 , @NewTable OUTPUT
	if (@NewTable = 0)
		exec ApagarTodosIndicesTmp 'PROPROPR','PRO','CODPROPR'
	exec AlterarCamposTmp 'PROPROPR', 'CODPROPR', 'int', 'INT NOT NULL', '4'
  	if (not exists(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 AND TABLE_NAME = 'PROPROPR' AND TABLE_SCHEMA = 'dbo'))
		ALTER TABLE [PROPROPR] ADD CONSTRAINT [PROPROPR_CODPROPR] PRIMARY KEY(CODPROPR)
	exec AlterarCamposTmp 'PROPROPR', 'FOTO', 'varbinary', 'VARBINARY(MAX)', '3'
	exec AlterarCamposTmp 'PROPROPR', 'TITULO', 'varchar', 'VARCHAR(80)', '80'
	exec AlterarCamposTmp 'PROPROPR', 'PRECO', 'float', 'FLOAT', '8'
	exec AlterarCamposTmp 'PROPROPR', 'CODAGENT', 'int', 'INT', '4'
	exec AlterarCamposTmp 'PROPROPR', 'TAMANHO', 'real', 'REAL', '4'
	exec AlterarCamposTmp 'PROPROPR', 'NR_WCS', 'smallint', 'SMALLINT', '2'
	exec AlterarCamposTmp 'PROPROPR', 'DTCONST', 'datetime', 'DATETIME', '8'
 	exec AlterarCamposTmp 'PROPROPR', 'DESCRICA', 'varchar', 'VARCHAR(MAX)', '8000'
	exec AlterarCamposTmp 'PROPROPR', 'CODCIDAD', 'int', 'INT', '4'
	exec AlterarCamposTmp 'PROPROPR', 'TIPOPROP', 'varchar', 'VARCHAR(1)', '1'
	exec AlterarCamposTmp 'PROPROPR', 'TIPOLOGI', 'smallint', 'SMALLINT', '2'
	exec AlterarCamposTmp 'PROPROPR', 'IDPROPRE', 'bigint', 'BIGINT', '8'
	exec AlterarCamposTmp 'PROPROPR', 'IDADEPRO', 'smallint', 'SMALLINT', '2'
	exec AlterarCamposTmp 'PROPROPR', 'ESPEXTER', 'real', 'REAL', '4'
	exec AlterarCamposTmp 'PROPROPR', 'VENDIDA', 'tinyint', 'TINYINT', '1'
	exec AlterarCamposTmp 'PROPROPR', 'LUCRO', 'float', 'FLOAT', '8'
		exec AlterarCamposTmp 'PROPROPR', 'LOCALIZA', 'GEOGRAPHY', 'GEOGRAPHY', '20'
	exec AlterarCamposTmp 'PROPROPR', 'ZZSTATE', 'tinyint', 'TINYINT', '1'

end
GO

-- Create Placeholder Computed columns
--PROMEM
--PROPAIS
--USERLOGIN
--ASYNCPROCESS
if exists (select Col.name from systypes as tp, sysobjects as Tbl  inner join syscolumns as Col on Tbl.id = Col.id  where Col.xtype = tp.xtype and Tbl.name = 'ASYNCPROCESS' and Col.name = 'RTSTATUS')
BEGIN
  EXEC dbo.DropConstraintsTmp 'ASYNCPROCESS', 'RTSTATUS'
  EXEC('ALTER TABLE [ASYNCPROCESS] DROP COLUMN [RTSTATUS]')
END
EXEC('ALTER TABLE [ASYNCPROCESS] ADD [RTSTATUS] AS cast(null as varchar(2))')
--NOTIFICATIONEMAILSIGNATURE
--NOTIFICATIONMESSAGE
--PROCIDAD
--ASYNCPROCESSARGUMENT
--ASYNCPROCESSATTACHMENTS
--USERAUTHORIZATION
--PROAGENTE_IMOBILIARIO
--PROPROPR
--PROALBUM
--PROCONTC
GO

-- Update Computed Column functions
--PROMEM
--PROPAIS
--USERLOGIN
--ASYNCPROCESS
IF NOT EXISTS (SELECT id FROM sysobjects WHERE xtype='FN' AND NAME='FORMULA_ASYNCPROCESS_RTSTATUS') EXEC('CREATE FUNCTION dbo.FORMULA_ASYNCPROCESS_RTSTATUS(@CODASCPR VARCHAR(MAX)) RETURNS varchar(2) AS BEGIN RETURN NULL END;')
EXEC('ALTER FUNCTION dbo.FORMULA_ASYNCPROCESS_RTSTATUS(@CODASCPR VARCHAR(MAX))
RETURNS varchar(2) AS
BEGIN
    RETURN (SELECT (case when ([s_apr].[status]=''EE'' or [s_apr].[status]=''D'' or [s_apr].[status]=''AC'' or [s_apr].[status]=''AG'') then (case when ((dbo.Diferenca_entre_Datas([s_apr].[lastupdt],CONVERT(datetime,CURRENT_TIMESTAMP),''S'')>10 and [s_apr].[status]<>''AG'') or (dbo.Diferenca_entre_Datas([s_apr].[lastupdt],CONVERT(datetime,CURRENT_TIMESTAMP),''S'')>45 and [s_apr].[status]=''AG'')) then ''NR'' else [s_apr].[status] end) else [s_apr].[status] end) from [asyncprocess] as [s_apr] WHERE [S_APR].[CODASCPR] = @CODASCPR);
END')
EXEC dbo.DropConstraintsTmp 'ASYNCPROCESS', 'RTSTATUS'
EXEC('ALTER TABLE [ASYNCPROCESS] DROP COLUMN [RTSTATUS]')
EXEC('ALTER TABLE [ASYNCPROCESS] ADD [RTSTATUS] AS dbo.FORMULA_ASYNCPROCESS_RTSTATUS(CODASCPR)')
--NOTIFICATIONEMAILSIGNATURE
--NOTIFICATIONMESSAGE
--PROCIDAD
--ASYNCPROCESSARGUMENT
--ASYNCPROCESSATTACHMENTS
--USERAUTHORIZATION
--PROAGENTE_IMOBILIARIO
--PROPROPR
--PROALBUM
--PROCONTC
GO

	exec estrutura_dinamica
GO
