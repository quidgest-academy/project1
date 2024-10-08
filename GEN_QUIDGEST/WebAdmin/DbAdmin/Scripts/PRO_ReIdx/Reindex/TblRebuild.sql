USE [W_GnBD]
if (2533 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'USERLOGIN') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [USERLOGIN] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2534 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'USERAUTHORIZATION') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [USERAUTHORIZATION] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2535 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'NOTIFICATIONMESSAGE') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [NOTIFICATIONMESSAGE] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2536 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'NOTIFICATIONEMAILSIGNATURE') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [NOTIFICATIONEMAILSIGNATURE] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2537 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'ASYNCPROCESS') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [ASYNCPROCESS] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2538 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'ASYNCPROCESSARGUMENT') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [ASYNCPROCESSARGUMENT] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2539 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'ASYNCPROCESSATTACHMENTS') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [ASYNCPROCESSATTACHMENTS] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2541 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROAGENTE_IMOBILIARIO') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [PROAGENTE_IMOBILIARIO] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2541 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROALBUM') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [PROALBUM] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2541 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROCIDAD') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [PROCIDAD] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2541 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROCONTC') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [PROCONTC] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2541 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROPAIS') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [PROPAIS] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if (2541 > isnull((select versao from [PROcfg]),0) or 'W_GnZeroTrue'='1')
begin
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROPROPR') = 0)
BEGIN
BEGIN TRAN
	ALTER TABLE [PROPROPR] REBUILD

	if (@@error <> 0)
		ROLLBACK TRAN
	else
		COMMIT TRAN
	end
END
GO
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROMEM') = 0)
	ALTER TABLE [PROMEM] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROCFG') = 0)
	ALTER TABLE [PROCFG] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROUSRSET') = 0)
	ALTER TABLE [PROUSRSET] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROUSRCFG') = 0)
	ALTER TABLE [PROUSRCFG] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROWKFACT') = 0)
	ALTER TABLE [PROWKFACT] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROWKFCON') = 0)
	ALTER TABLE [PROWKFCON] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROWKFLIG') = 0)
	ALTER TABLE [PROWKFLIG] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROWKFLOW') = 0)
	ALTER TABLE [PROWKFLOW] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PRONOTIFI') = 0)
	ALTER TABLE [PRONOTIFI] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROSCRCRD') = 0)
	ALTER TABLE [PROSCRCRD] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROPOSTIT') = 0)
	ALTER TABLE [PROPOSTIT] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROPRMFRM') = 0)
	ALTER TABLE [PROPRMFRM] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROALERTA') = 0)
	ALTER TABLE [PROALERTA] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROALTENT') = 0)
	ALTER TABLE [PROALTENT] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROTALERT') = 0)
	ALTER TABLE [PROTALERT] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PRODELEGA') = 0)
	ALTER TABLE [PRODELEGA] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROLSTUSR') = 0)
	ALTER TABLE [PROLSTUSR] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROLSTCOL') = 0)
	ALTER TABLE [PROLSTCOL] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROLSTREN') = 0)
	ALTER TABLE [PROLSTREN] REBUILD
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'PROUSRWID') = 0)
	ALTER TABLE [PROUSRWID] REBUILD
GO
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'DOCUMS') = 0)
	ALTER TABLE [DOCUMS] REBUILD
GO
if ((select count(1) tablename from sys.fulltext_index_columns fic where UPPER(object_name(fic.[object_id])) = 'hashcd') = 0)
	ALTER TABLE [hashcd] REBUILD
GO
