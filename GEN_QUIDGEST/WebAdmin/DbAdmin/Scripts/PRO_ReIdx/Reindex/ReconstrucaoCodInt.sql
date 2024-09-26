USE [W_GnBD]
DECLARE @nextcod bigint

if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROagente_imobiliario')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROagente_imobiliario', coalesce(cast((select max(codagent) from PROagente_imobiliario) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codagent) from PROagente_imobiliario) as bigint) + 1, 1) where id_objecto = 'PROagente_imobiliario'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROALBUM')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROALBUM', coalesce(cast((select max(codalbum) from PROALBUM) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codalbum) from PROALBUM) as bigint) + 1, 1) where id_objecto = 'PROALBUM'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROCIDAD')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROCIDAD', coalesce(cast((select max(codcidad) from PROCIDAD) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codcidad) from PROCIDAD) as bigint) + 1, 1) where id_objecto = 'PROCIDAD'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROCONTC')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROCONTC', coalesce(cast((select max(codcontc) from PROCONTC) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codcontc) from PROCONTC) as bigint) + 1, 1) where id_objecto = 'PROCONTC'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROPAIS')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROPAIS', coalesce(cast((select max(codpais) from PROPAIS) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codpais) from PROPAIS) as bigint) + 1, 1) where id_objecto = 'PROPAIS'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROPROPR')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROPROPR', coalesce(cast((select max(codpropr) from PROPROPR) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codpropr) from PROPROPR) as bigint) + 1, 1) where id_objecto = 'PROPROPR'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'UserLogin')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('UserLogin', coalesce(cast((select max(codpsw) from UserLogin) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codpsw) from UserLogin) as bigint) + 1, 1) where id_objecto = 'UserLogin'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'NotificationEmailSignature')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('NotificationEmailSignature', coalesce(cast((select max(codsigna) from NotificationEmailSignature) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codsigna) from NotificationEmailSignature) as bigint) + 1, 1) where id_objecto = 'NotificationEmailSignature'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'NotificationMessage')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('NotificationMessage', coalesce(cast((select max(codmesgs) from NotificationMessage) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codmesgs) from NotificationMessage) as bigint) + 1, 1) where id_objecto = 'NotificationMessage'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'AsyncProcessAttachments')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('AsyncProcessAttachments', coalesce(cast((select max(codpranx) from AsyncProcessAttachments) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codpranx) from AsyncProcessAttachments) as bigint) + 1, 1) where id_objecto = 'AsyncProcessAttachments'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'UserAuthorization')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('UserAuthorization', coalesce(cast((select max(codua) from UserAuthorization) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codua) from UserAuthorization) as bigint) + 1, 1) where id_objecto = 'UserAuthorization'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROMEM')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROMEM', coalesce(cast((select max(CODMEM) from PROMEM) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODMEM) from PROMEM) as bigint) + 1, 1) where id_objecto = 'PROMEM'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROCFG')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROCFG', coalesce(cast((select max(CODCFG) from PROCFG) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODCFG) from PROCFG) as bigint) + 1, 1) where id_objecto = 'PROCFG'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROUSRSET')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROUSRSET', coalesce(cast((select max(CODUSRSET) from PROUSRSET) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODUSRSET) from PROUSRSET) as bigint) + 1, 1) where id_objecto = 'PROUSRSET'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROUSRCFG')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROUSRCFG', coalesce(cast((select max(CODUSRCFG) from PROUSRCFG) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODUSRCFG) from PROUSRCFG) as bigint) + 1, 1) where id_objecto = 'PROUSRCFG'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROWKFACT')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROWKFACT', coalesce(cast((select max(CODWKFACT) from PROWKFACT) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODWKFACT) from PROWKFACT) as bigint) + 1, 1) where id_objecto = 'PROWKFACT'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROWKFCON')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROWKFCON', coalesce(cast((select max(CODWKFCON) from PROWKFCON) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODWKFCON) from PROWKFCON) as bigint) + 1, 1) where id_objecto = 'PROWKFCON'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROWKFLIG')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROWKFLIG', coalesce(cast((select max(CODWKFLIG) from PROWKFLIG) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODWKFLIG) from PROWKFLIG) as bigint) + 1, 1) where id_objecto = 'PROWKFLIG'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROWKFLOW')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROWKFLOW', coalesce(cast((select max(CODWKFLOW) from PROWKFLOW) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODWKFLOW) from PROWKFLOW) as bigint) + 1, 1) where id_objecto = 'PROWKFLOW'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PRONOTIFI')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PRONOTIFI', coalesce(cast((select max(CODNOTIFI) from PRONOTIFI) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODNOTIFI) from PRONOTIFI) as bigint) + 1, 1) where id_objecto = 'PRONOTIFI'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROSCRCRD')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROSCRCRD', coalesce(cast((select max(CODSCRCRD) from PROSCRCRD) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODSCRCRD) from PROSCRCRD) as bigint) + 1, 1) where id_objecto = 'PROSCRCRD'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROPOSTIT')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROPOSTIT', coalesce(cast((select max(CODPOSTIT) from PROPOSTIT) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODPOSTIT) from PROPOSTIT) as bigint) + 1, 1) where id_objecto = 'PROPOSTIT'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROPRMFRM')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROPRMFRM', coalesce(cast((select max(CODPRMFRM) from PROPRMFRM) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODPRMFRM) from PROPRMFRM) as bigint) + 1, 1) where id_objecto = 'PROPRMFRM'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROALERTA')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROALERTA', coalesce(cast((select max(CODALERTA) from PROALERTA) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODALERTA) from PROALERTA) as bigint) + 1, 1) where id_objecto = 'PROALERTA'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROALTENT')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROALTENT', coalesce(cast((select max(CODALTENT) from PROALTENT) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODALTENT) from PROALTENT) as bigint) + 1, 1) where id_objecto = 'PROALTENT'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROTALERT')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROTALERT', coalesce(cast((select max(CODTALERT) from PROTALERT) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODTALERT) from PROTALERT) as bigint) + 1, 1) where id_objecto = 'PROTALERT'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PRODELEGA')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PRODELEGA', coalesce(cast((select max(CODDELEGA) from PRODELEGA) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODDELEGA) from PRODELEGA) as bigint) + 1, 1) where id_objecto = 'PRODELEGA'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROLSTUSR')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROLSTUSR', coalesce(cast((select max(CODLSTUSR) from PROLSTUSR) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODLSTUSR) from PROLSTUSR) as bigint) + 1, 1) where id_objecto = 'PROLSTUSR'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROLSTCOL')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROLSTCOL', coalesce(cast((select max(CODLSTCOL) from PROLSTCOL) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODLSTCOL) from PROLSTCOL) as bigint) + 1, 1) where id_objecto = 'PROLSTCOL'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROLSTREN')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROLSTREN', coalesce(cast((select max(CODLSTREN) from PROLSTREN) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODLSTREN) from PROLSTREN) as bigint) + 1, 1) where id_objecto = 'PROLSTREN'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROUSRWID')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROUSRWID', coalesce(cast((select max(CODUSRWID) from PROUSRWID) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODUSRWID) from PROUSRWID) as bigint) + 1, 1) where id_objecto = 'PROUSRWID'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROTBLCFG')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROTBLCFG', coalesce(cast((select max(CODTBLCFG) from PROTBLCFG) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODTBLCFG) from PROTBLCFG) as bigint) + 1, 1) where id_objecto = 'PROTBLCFG'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROTBLCFGSEL')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROTBLCFGSEL', coalesce(cast((select max(CODTBLCFGSEL) from PROTBLCFGSEL) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODTBLCFGSEL) from PROTBLCFGSEL) as bigint) + 1, 1) where id_objecto = 'PROTBLCFGSEL'

if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'HASHCD')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('HASHCD', coalesce(cast((select max(codHASHCD) from HASHCD) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codHASHCD) from HASHCD) as bigint) + 1, 1) where id_objecto = 'HASHCD'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'DOCUMS')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('DOCUMS', coalesce(cast((select max(codDOCUMS) from DOCUMS) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codDOCUMS) from DOCUMS) as bigint) + 1, 1) where id_objecto = 'DOCUMS'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'UserAuthorization')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('UserAuthorization', coalesce(cast((select max(CODUA) from UserAuthorization) as int) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODUA) from UserAuthorization) as int) + 1, 1) where id_objecto = 'UserAuthorization'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROWORKFLOWPROCESS')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROWORKFLOWPROCESS', coalesce(cast((select max(CODPRCESS) from PROWORKFLOWPROCESS) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODPRCESS) from PROWORKFLOWPROCESS) as bigint) + 1, 1) where id_objecto = 'PROWORKFLOWPROCESS'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'PROWORKFLOWTASK')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('PROWORKFLOWTASK', coalesce(cast((select max(CODTASK) from PROWORKFLOWTASK) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODTASK) from PROWORKFLOWTASK) as bigint) + 1, 1) where id_objecto = 'PROWORKFLOWTASK'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'NotificationEmailSignature')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('NotificationEmailSignature', coalesce(cast((select max(codsigna) from NotificationEmailSignature) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codsigna) from NotificationEmailSignature) as bigint) + 1, 1) where id_objecto = 'NotificationEmailSignature'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'NotificationMessage')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('NotificationMessage', coalesce(cast((select max(codmesgs) from NotificationMessage) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(codmesgs) from NotificationMessage) as bigint) + 1, 1) where id_objecto = 'NotificationMessage'
if not exists (select id_objecto from Codigos_Sequenciais where id_objecto = 'ReportList')
	insert into Codigos_Sequenciais (id_objecto, proximo) values ('ReportList', coalesce(cast((select max(CODREPORT) from ReportList) as bigint) + 1, 1))
else
	update Codigos_Sequenciais set proximo = coalesce(cast((select max(CODREPORT) from ReportList) as bigint) + 1, 1) where id_objecto = 'ReportList'
GO
 