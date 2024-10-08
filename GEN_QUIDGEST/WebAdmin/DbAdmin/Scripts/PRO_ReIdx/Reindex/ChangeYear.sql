USE [W_GnBD]

GO

USE [W_GnSrcBD]

-- migração de dados da tabela PROMEM (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[PROMEM] ([CODMEM], [LOGIN], [ROTINA], [ALTURA], [OBS], [HOSTID], [CLIENTID], [ZZSTATE])
SELECT [MEM].[CODMEM], [MEM].[LOGIN], [MEM].[ROTINA], [MEM].[ALTURA], [MEM].[OBS], [MEM].[HOSTID], [MEM].[CLIENTID], [MEM].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[PROMEM] AS [MEM]
WHERE [MEM].ZZSTATE = 0
GO

-- migração de dados da tabela PROPAIS (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[PROPAIS] ([CODPAIS], [PAIS], [ZZSTATE])
SELECT [PAIS].[CODPAIS], [PAIS].[PAIS], [PAIS].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[PROPAIS] AS [PAIS]
WHERE [PAIS].ZZSTATE = 0
GO

-- migração de dados da tabela UserLogin (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[UserLogin] ([CODPSW], [NOME], [PASSWORD], [CERTSN], [EMAIL], [PSWTYPE], [SALT], [DATAPSW], [USERID], [PSW2FAVL], [PSW2FATP], [DATEXP], [ATTEMPTS], [PHONE], [STATUS], [ASSOCIA], [OPERCRIA], [DATACRIA], [OPERMUDA], [DATAMUDA], [ZZSTATE])
SELECT [PSW].[CODPSW], [PSW].[NOME], [PSW].[PASSWORD], [PSW].[CERTSN], [PSW].[EMAIL], [PSW].[PSWTYPE], [PSW].[SALT], [PSW].[DATAPSW], [PSW].[USERID], [PSW].[PSW2FAVL], [PSW].[PSW2FATP], [PSW].[DATEXP], [PSW].[ATTEMPTS], [PSW].[PHONE], [PSW].[STATUS], [PSW].[ASSOCIA], [PSW].[OPERCRIA], [PSW].[DATACRIA], [PSW].[OPERMUDA], [PSW].[DATAMUDA], [PSW].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[UserLogin] AS [PSW]
WHERE [PSW].ZZSTATE = 0
GO

-- migração de dados da tabela AsyncProcess (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[AsyncProcess] ([CODASCPR], [TYPE], [DATEREQU], [INITPRC], [ENDPRC], [DURATION], [STATUS], [RSLTMSG], [FINISHED], [LASTUPDT], [RESULT], [INFO], [PERCENTA], [MODOPROC], [EXTERNAL], [ID], [CODENTIT], [MOTIVO], [CODPSW], [OPERSHUT], [OPERCRIA], [DATACRIA], [OPERMUDA], [DATAMUDA], [ZZSTATE])
SELECT [S_APR].[CODASCPR], [S_APR].[TYPE], [S_APR].[DATEREQU], [S_APR].[INITPRC], [S_APR].[ENDPRC], [S_APR].[DURATION], [S_APR].[STATUS], [S_APR].[RSLTMSG], [S_APR].[FINISHED], [S_APR].[LASTUPDT], [S_APR].[RESULT], [S_APR].[INFO], [S_APR].[PERCENTA], [S_APR].[MODOPROC], [S_APR].[EXTERNAL], [S_APR].[ID], [S_APR].[CODENTIT], [S_APR].[MOTIVO], [S_APR].[CODPSW], [S_APR].[OPERSHUT], [S_APR].[OPERCRIA], [S_APR].[DATACRIA], [S_APR].[OPERMUDA], [S_APR].[DATAMUDA], [S_APR].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[AsyncProcess] AS [S_APR]
WHERE [S_APR].ZZSTATE = 0
GO

-- migração de dados da tabela NotificationEmailSignature (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[NotificationEmailSignature] ([CODSIGNA], [NAME], [IMAGE], [TEXTASS], [USERNAME], [PASSWORD], [OPERCRIA], [DATACRIA], [OPERMUDA], [DATAMUDA], [ZZSTATE])
SELECT [S_NES].[CODSIGNA], [S_NES].[NAME], [S_NES].[IMAGE], [S_NES].[TEXTASS], [S_NES].[USERNAME], [S_NES].[PASSWORD], [S_NES].[OPERCRIA], [S_NES].[DATACRIA], [S_NES].[OPERMUDA], [S_NES].[DATAMUDA], [S_NES].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[NotificationEmailSignature] AS [S_NES]
WHERE [S_NES].ZZSTATE = 0
GO

-- migração de dados da tabela NotificationMessage (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[NotificationMessage] ([CODMESGS], [CODSIGNA], [CODPMAIL], [FROM], [CODTPNOT], [CODDESTN], [TO], [DESTNMAN], [TOMANUAL], [CC], [BCC], [IDNOTIF], [NOTIFICA], [EMAIL], [ASSUNTO], [AGREGADO], [ANEXO], [HTML], [ATIVO], [DESIGNAC], [MENSAGEM], [GRAVABD], [OPERCRIA], [DATACRIA], [OPERMUDA], [DATAMUDA], [ZZSTATE])
SELECT [S_NM].[CODMESGS], [S_NM].[CODSIGNA], [S_NM].[CODPMAIL], [S_NM].[FROM], [S_NM].[CODTPNOT], [S_NM].[CODDESTN], [S_NM].[TO], [S_NM].[DESTNMAN], [S_NM].[TOMANUAL], [S_NM].[CC], [S_NM].[BCC], [S_NM].[IDNOTIF], [S_NM].[NOTIFICA], [S_NM].[EMAIL], [S_NM].[ASSUNTO], [S_NM].[AGREGADO], [S_NM].[ANEXO], [S_NM].[HTML], [S_NM].[ATIVO], [S_NM].[DESIGNAC], [S_NM].[MENSAGEM], [S_NM].[GRAVABD], [S_NM].[OPERCRIA], [S_NM].[DATACRIA], [S_NM].[OPERMUDA], [S_NM].[DATAMUDA], [S_NM].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[NotificationMessage] AS [S_NM]
WHERE [S_NM].ZZSTATE = 0
GO

-- migração de dados da tabela PROCIDAD (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[PROCIDAD] ([CODCIDAD], [CIDADE], [CODPAIS], [ZZSTATE])
SELECT [CIDAD].[CODCIDAD], [CIDAD].[CIDADE], [CIDAD].[CODPAIS], [CIDAD].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[PROCIDAD] AS [CIDAD]
WHERE [CIDAD].ZZSTATE = 0
GO

-- migração de dados da tabela AsyncProcessArgument (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[AsyncProcessArgument] ([CODARGPR], [CODS_APR], [ID], [VALOR], [DOCUMENT], [DOCUMENTFK], [TIPO], [DESIGNAC], [HIDDEN], [OPERCRIA], [DATACRIA], [OPERMUDA], [DATAMUDA], [ZZSTATE])
SELECT [S_ARG].[CODARGPR], [S_ARG].[CODS_APR], [S_ARG].[ID], [S_ARG].[VALOR], [S_ARG].[DOCUMENT], [S_ARG].[DOCUMENTFK], [S_ARG].[TIPO], [S_ARG].[DESIGNAC], [S_ARG].[HIDDEN], [S_ARG].[OPERCRIA], [S_ARG].[DATACRIA], [S_ARG].[OPERMUDA], [S_ARG].[DATAMUDA], [S_ARG].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[AsyncProcessArgument] AS [S_ARG]
WHERE [S_ARG].ZZSTATE = 0
GO

-- migração de dados da tabela AsyncProcessAttachments (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[AsyncProcessAttachments] ([CODPRANX], [CODS_APR], [DOCUMENT], [DOCUMENTFK], [OPERCRIA], [DATACRIA], [ZZSTATE])
SELECT [S_PAX].[CODPRANX], [S_PAX].[CODS_APR], [S_PAX].[DOCUMENT], [S_PAX].[DOCUMENTFK], [S_PAX].[OPERCRIA], [S_PAX].[DATACRIA], [S_PAX].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[AsyncProcessAttachments] AS [S_PAX]
WHERE [S_PAX].ZZSTATE = 0
GO

-- migração de dados da tabela UserAuthorization (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[UserAuthorization] ([CODUA], [CODPSW], [SISTEMA], [MODULO], [NAODUPLI], [ROLE], [NIVEL], [OPERCRIA], [DATACRIA], [OPERMUDA], [DATAMUDA], [ZZSTATE])
SELECT [S_UA].[CODUA], [S_UA].[CODPSW], [S_UA].[SISTEMA], [S_UA].[MODULO], [S_UA].[NAODUPLI], [S_UA].[ROLE], [S_UA].[NIVEL], [S_UA].[OPERCRIA], [S_UA].[DATACRIA], [S_UA].[OPERMUDA], [S_UA].[DATAMUDA], [S_UA].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[UserAuthorization] AS [S_UA]
WHERE [S_UA].ZZSTATE = 0
GO

-- migração de dados da tabela PROagente_imobiliario (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[PROagente_imobiliario] ([CODAGENT], [FOTO], [NOME], [DNASCIME], [EMAIL], [TELEFONE], [CODPMORA], [CODPNASC], [PERELUCR], [LUCRO], [ZZSTATE])
SELECT [AGENT].[CODAGENT], [AGENT].[FOTO], [AGENT].[NOME], [AGENT].[DNASCIME], [AGENT].[EMAIL], [AGENT].[TELEFONE], [AGENT].[CODPMORA], [AGENT].[CODPNASC], [AGENT].[PERELUCR], [AGENT].[LUCRO], [AGENT].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[PROagente_imobiliario] AS [AGENT]
WHERE [AGENT].ZZSTATE = 0
GO

-- migração de dados da tabela PROPROPR (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[PROPROPR] ([CODPROPR], [FOTO], [TITULO], [PRECO], [CODAGENT], [TAMANHO], [NR_WCS], [DTCONST], [DESCRICA], [CODCIDAD], [TIPOPROP], [TIPOLOGI], [IDPROPRE], [IDADEPRO], [ESPEXTER], [VENDIDA], [LUCRO], [LOCALIZA], [ZZSTATE])
SELECT [PROPR].[CODPROPR], [PROPR].[FOTO], [PROPR].[TITULO], [PROPR].[PRECO], [PROPR].[CODAGENT], [PROPR].[TAMANHO], [PROPR].[NR_WCS], [PROPR].[DTCONST], [PROPR].[DESCRICA], [PROPR].[CODCIDAD], [PROPR].[TIPOPROP], [PROPR].[TIPOLOGI], [PROPR].[IDPROPRE], [PROPR].[IDADEPRO], [PROPR].[ESPEXTER], [PROPR].[VENDIDA], [PROPR].[LUCRO], [PROPR].[LOCALIZA], [PROPR].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[PROPROPR] AS [PROPR]
WHERE [PROPR].ZZSTATE = 0
GO

-- migração de dados da tabela PROALBUM (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[PROALBUM] ([CODALBUM], [FOTO], [TITULO], [CODPROPR], [ZZSTATE])
SELECT [ALBUM].[CODALBUM], [ALBUM].[FOTO], [ALBUM].[TITULO], [ALBUM].[CODPROPR], [ALBUM].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[PROALBUM] AS [ALBUM]
WHERE [ALBUM].ZZSTATE = 0
GO

-- migração de dados da tabela PROCONTC (Modo: 0)
INSERT INTO [W_GnBD].[dbo].[PROCONTC] ([CODCONTC], [DTCONTAT], [CODPROPR], [CLTNAME], [CLTEMAIL], [TELEFONE], [DESCRIIC], [ZZSTATE])
SELECT [CONTC].[CODCONTC], [CONTC].[DTCONTAT], [CONTC].[CODPROPR], [CONTC].[CLTNAME], [CONTC].[CLTEMAIL], [CONTC].[TELEFONE], [CONTC].[DESCRIIC], [CONTC].[ZZSTATE]
 FROM [W_GnSrcBD].[dbo].[PROCONTC] AS [CONTC]
WHERE [CONTC].ZZSTATE = 0
GO

-- Tabelas Hardcoded
-- migração de dados da tabela Codigos Sequenciais
INSERT INTO [W_GnBD].[dbo].[Codigos_Sequenciais] (id_objecto, proximo)
SELECT id_objecto, proximo FROM [W_GnSrcBD].[dbo].[Codigos_Sequenciais]
GO

-- migração de dados da tabela UserAuthorization
INSERT INTO [W_GnBD].[dbo].[UserAuthorization] (CODUA, CODPSW, SISTEMA, MODULO, ROLE, NIVEL, ZZSTATE)
SELECT UA.CODUA, UA.CODPSW, UA.SISTEMA, UA.MODULO, UA.ROLE, UA.NIVEL, UA.ZZSTATE FROM [W_GnSrcBD].[dbo].[UserAuthorization] AS UA
WHERE UA.ZZSTATE = 0
GO

-- migração de dados da tabela CFG (ao criar schema, fica criada uma linha nesta tabela)
--INSERT INTO [W_GnBD].[dbo].[PROcfg] (codcfg, checkdat, versao, versindx, manutdat, upgrindx, ZZSTATE)
--SELECT codcfg, checkdat, versao, versindx, manutdat, upgrindx, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROcfg]
--WHERE ZZSTATE = 0
--GO

-- migração de dados da tabela LSTUSR
INSERT INTO [W_GnBD].[dbo].[PROlstusr] (CODLSTUSR, CODPSW, DESCRIC, IDLIST, MODULO, SISTEMA, ORDERCOL, ORDERTYPE, [DATA], ZZSTATE)
SELECT CODLSTUSR, CODPSW, DESCRIC, IDLIST, MODULO, SISTEMA, ORDERCOL, ORDERTYPE, [DATA], ZZSTATE FROM [W_GnSrcBD].[dbo].[PROlstusr]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela LSTCOL
INSERT INTO [W_GnBD].[dbo].[PROlstcol] (CODLSTCOL, CODLSTUSR, TABELA, ALIAS, CAMPO, VISIVEL, POSICAO, OPERACAO, TIPO, ZZSTATE)
SELECT CODLSTCOL, CODLSTUSR, TABELA, ALIAS, CAMPO, VISIVEL, POSICAO, OPERACAO, TIPO, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROlstcol]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela LSTREN
INSERT INTO [W_GnBD].[dbo].[PROlstren] (CODLSTREN, CODLSTUSR, RENDERIZACAO, VISIVEL, POSICAO, OPERACAO, TIPO, ZZSTATE)
SELECT CODLSTREN, CODLSTUSR, RENDERIZACAO, VISIVEL, POSICAO, OPERACAO, TIPO, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROlstren]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela USRWID
INSERT INTO [W_GnBD].[dbo].[PROusrwid] (CODUSRWID, CODLSTUSR, WIDGET, ROWKEY, VISIBLE, HPOSITION, VPOSITION, ZZSTATE)
SELECT CODUSRWID, CODLSTUSR, WIDGET, ROWKEY, VISIBLE, HPOSITION, VPOSITION, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROusrwid]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela USRCFG
INSERT INTO [W_GnBD].[dbo].[PROusrcfg] (codusrcfg, modulo, codpsw, tipo, id, ZZSTATE)
SELECT codusrcfg, modulo, codpsw, tipo, id, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROusrcfg]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela USRSET
INSERT INTO [W_GnBD].[dbo].[PROusrset] (codusrset, modulo, codpsw, chave, valor, ZZSTATE)
SELECT codusrset, modulo, codpsw, chave, valor, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROusrset]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela WKFACT
INSERT INTO [W_GnBD].[dbo].[PROwkfact] (codwkfact, wfid, actid, tpactid, descrica, duracao, perfil, perfilu, x, y, ZZSTATE)
SELECT codwkfact, wfid, actid, tpactid, descrica, duracao, perfil, perfilu, x, y, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROwkfact]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela WKFCON
INSERT INTO [W_GnBD].[dbo].[PROwkfcon] (codwkfcon, wfid, condid, tpcondid, descrica, tiporegr, sinal, x, y, valor, ZZSTATE)
SELECT codwkfcon, wfid, condid, tpcondid, descrica, tiporegr, sinal, x, y, valor, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROwkfcon]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela WKFLIG
INSERT INTO [W_GnBD].[dbo].[PROwkflig] (codwkflig, wfid, parentid, childid, ptoy, ptox, ptor, tipo, pfromx, pfromy, pfromr, ZZSTATE)
SELECT codwkflig, wfid, parentid, childid, ptoy, ptox, ptor, tipo, pfromx, pfromy, pfromr, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROwkflig]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela WKFLOW
INSERT INTO [W_GnBD].[dbo].[PROwkflow] (codwkflow, descrica, modulo, ZZSTATE)
SELECT codwkflow, descrica, modulo, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROwkflow]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela NOTIFI
INSERT INTO [W_GnBD].[dbo].[PROnotifi] (codnotifi, modulo, descrica, activid, menuid, usernivl, wfid, ZZSTATE)
SELECT codnotifi, modulo, descrica, activid, menuid, usernivl, wfid, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROnotifi]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela PRMFRM
INSERT INTO [W_GnBD].[dbo].[PROprmfrm] (codprmfrm, desform, perfil, autoriza, sevalida, prfvalid, prazodia, comprova, prazohor, secompro, mensag1, mensag2, mensag3, mensag4, ZZSTATE)
SELECT codprmfrm, desform, perfil, autoriza, sevalida, prfvalid, prazodia, comprova, prazohor, secompro, mensag1, mensag2, mensag3, mensag4, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROprmfrm]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela SCRCRD
INSERT INTO [W_GnBD].[dbo].[PROscrcrd] (codscrcrd, descrica, valor, cor, seta, usernivl, ZZSTATE)
SELECT codscrcrd, descrica, valor, cor, seta, usernivl, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROscrcrd]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela DOCUMS
INSERT INTO [W_GnBD].[dbo].[docums] (coddocums, documid, document, docpath, tabela, campo, chave, form, nome, versao, tamanho, extensao, opercria, datacria, opermuda, datamuda, ZZSTATE)
SELECT coddocums, documid, document, docpath, tabela, campo, chave, form, nome, versao, tamanho, extensao, opercria, datacria, opermuda, datamuda, ZZSTATE FROM [W_GnSrcBD].[dbo].[docums]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela POSTIT
INSERT INTO [W_GnBD].[dbo].[PROpostit] (codpostit, tabela, codtabel, postit, tpostit, datacria, opercria, codpsw, lido, apagado, validade, nivel, codpost1, ZZSTATE)
SELECT codpostit, tabela, codtabel, postit, tpostit, datacria, opercria, codpsw, lido, apagado, validade, nivel, codpost1, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROpostit]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela HASHCD
INSERT INTO [W_GnBD].[dbo].[hashcd] (codhashcd, tabela, campos, datacria, ZZSTATE)
SELECT codhashcd, tabela, campos, datacria, ZZSTATE FROM [W_GnSrcBD].[dbo].[hashcd]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela ALERTA
INSERT INTO [W_GnBD].[dbo].[PROalerta] (codalerta, codaltent, mensagem, tratado, activo, datacria, datareso, menu, cor, interno, backgrou, sms, email, emailenv, smsenvia, codigo, codigotp, ZZSTATE)
SELECT codalerta, codaltent, mensagem, tratado, activo, datacria, datareso, menu, cor, interno, backgrou, sms, email, emailenv, smsenvia, codigo, codigotp, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROalerta]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela ALTENT
INSERT INTO [W_GnBD].[dbo].[PROaltent] (codaltent, codtalert, codpsw, grupo, contador, tipo, mensagem, antecede, datainic, datafina, dtmodifi, todos, backgrou, sms, email, codtpgru, codtpess, menu, incluian, activo, individu, ZZSTATE)
SELECT codaltent, codtalert, codpsw, grupo, contador, tipo, mensagem, antecede, datainic, datafina, dtmodifi, todos, backgrou, sms, email, codtpgru, codtpess, menu, incluian, activo, individu, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROaltent]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela TALERT
INSERT INTO [W_GnBD].[dbo].[PROtalert] (codtalert, nome, metodo, priorida, cor, texto, menu, cmpnome, daltinic, daltfina, incluian, diferenc, anodifer, mesdifer, diadifer, ntabmae, ntabfilh, formid, tabela, campo, tipo, modulo, condicao, ZZSTATE)
SELECT codtalert, nome, metodo, priorida, cor, texto, menu, cmpnome, daltinic, daltfina, incluian, diferenc, anodifer, mesdifer, diadifer, ntabmae, ntabfilh, formid, tabela, campo, tipo, modulo, condicao, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROtalert]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela DELEGA
INSERT INTO [W_GnBD].[dbo].[PROdelega] (coddelega, codpswup, codpswdw, dateini, dateend, message, revoked, auditusr, opercria, datacria, opermuda, datamuda, ZZSTATE)
SELECT coddelega, codpswup, codpswdw, dateini, dateend, message, revoked, auditusr, opercria, datacria, opermuda, datamuda, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROdelega]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela TABDINAMIC
INSERT INTO [W_GnBD].[dbo].[PROTABDINAMIC] (codtabdinamic, NOMETABELA, NOMECAMPO, TIPODADOS, TIPOSQL, LARGURA, ZZSTATE)
SELECT codtabdinamic, NOMETABELA, NOMECAMPO, TIPODADOS, TIPOSQL, LARGURA, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROTABDINAMIC]
WHERE ZZSTATE = 0
GO


-- migração de dados da tabela ALTRAN
INSERT INTO [W_GnBD].[dbo].[PROaltran] (codaltran, typlabel, referenc, [language], curlabel, labellng, altran, opercria, datacria, opermuda, datamuda, ZZSTATE)
SELECT codaltran, typlabel, referenc, [language], curlabel, labellng, altran, opercria, datacria, opermuda, datamuda, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROaltran]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela WORKFLOWTASK
INSERT INTO [W_GnBD].[dbo].[PROworkflowtask] (codtask, codprcess, taskdefid, taskid, performedby, runningsince, nextrun, modifieddate, modifiedby, skipped, isactive, errorExecute, ZZSTATE)
SELECT codtask, codprcess, taskdefid, taskid, performedby, runningsince, nextrun, modifieddate, modifiedby, skipped, isactive, errorExecute, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROworkflowtask]
WHERE ZZSTATE = 0
GO

-- migração de dados da tabela WORKFLOWPROCESS
INSERT INTO [W_GnBD].[dbo].[PROworkflowprocess] (codprcess, prcessid, prcessdefid, createdby, status, startdate, modifieddate, modifiedby, codinstance, dotgraph, wferror, ltask, idhk, ZZSTATE)
SELECT codprcess, prcessid, prcessdefid, createdby, status, startdate, modifieddate, modifiedby, codinstance, dotgraph, wferror, ltask, idhk, ZZSTATE FROM [W_GnSrcBD].[dbo].[PROworkflowprocess]
WHERE ZZSTATE = 0
GO


USE [W_GnBD]
GO
-- Removing invalid FK values
UPDATE [CIDAD] SET
[CIDAD].[CODPAIS] = [q0_PAIS].[CODPAIS]
 FROM [W_GnBD].[dbo].[PROCIDAD] AS [CIDAD]
LEFT JOIN [PROPAIS] AS [q0_PAIS] ON [CIDAD].[CODPAIS] = [q0_PAIS].[CODPAIS]
GO

UPDATE [S_ARG] SET
[S_ARG].[CODS_APR] = [q0_S_APR].[CODASCPR]
 FROM [W_GnBD].[dbo].[AsyncProcessArgument] AS [S_ARG]
LEFT JOIN [AsyncProcess] AS [q0_S_APR] ON [S_ARG].[CODS_APR] = [q0_S_APR].[CODASCPR]
GO

UPDATE [S_PAX] SET
[S_PAX].[CODS_APR] = [q0_S_APR].[CODASCPR]
 FROM [W_GnBD].[dbo].[AsyncProcessAttachments] AS [S_PAX]
LEFT JOIN [AsyncProcess] AS [q0_S_APR] ON [S_PAX].[CODS_APR] = [q0_S_APR].[CODASCPR]
GO

UPDATE [S_UA] SET
[S_UA].[CODPSW] = [q0_PSW].[CODPSW]
 FROM [W_GnBD].[dbo].[UserAuthorization] AS [S_UA]
LEFT JOIN [UserLogin] AS [q0_PSW] ON [S_UA].[CODPSW] = [q0_PSW].[CODPSW]
GO

UPDATE [AGENT] SET
[AGENT].[CODPMORA] = [q0_PAIS].[CODPAIS]
, [AGENT].[CODPNASC] = [q1_PAIS].[CODPAIS]
 FROM [W_GnBD].[dbo].[PROagente_imobiliario] AS [AGENT]
LEFT JOIN [PROPAIS] AS [q0_PAIS] ON [AGENT].[CODPMORA] = [q0_PAIS].[CODPAIS]
LEFT JOIN [PROPAIS] AS [q1_PAIS] ON [AGENT].[CODPNASC] = [q1_PAIS].[CODPAIS]
GO

UPDATE [PROPR] SET
[PROPR].[CODAGENT] = [q0_AGENT].[CODAGENT]
, [PROPR].[CODCIDAD] = [q1_CIDAD].[CODCIDAD]
 FROM [W_GnBD].[dbo].[PROPROPR] AS [PROPR]
LEFT JOIN [PROagente_imobiliario] AS [q0_AGENT] ON [PROPR].[CODAGENT] = [q0_AGENT].[CODAGENT]
LEFT JOIN [PROCIDAD] AS [q1_CIDAD] ON [PROPR].[CODCIDAD] = [q1_CIDAD].[CODCIDAD]
GO

UPDATE [ALBUM] SET
[ALBUM].[CODPROPR] = [q0_PROPR].[CODPROPR]
 FROM [W_GnBD].[dbo].[PROALBUM] AS [ALBUM]
LEFT JOIN [PROPROPR] AS [q0_PROPR] ON [ALBUM].[CODPROPR] = [q0_PROPR].[CODPROPR]
GO

UPDATE [CONTC] SET
[CONTC].[CODPROPR] = [q0_PROPR].[CODPROPR]
 FROM [W_GnBD].[dbo].[PROCONTC] AS [CONTC]
LEFT JOIN [PROPROPR] AS [q0_PROPR] ON [CONTC].[CODPROPR] = [q0_PROPR].[CODPROPR]
GO


USE [W_GnBD]

GO
