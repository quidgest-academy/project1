IF((SELECT COUNT(1) FROM PROcfg) = 0)
	INSERT INTO PROcfg (codcfg, checkdat, versao, versindx, manutdat, upgrindx, zzstate) VALUES ('     1', GETDATE(), 2540, 0, NULL, 0, 0)
ELSE
  UPDATE PROcfg set versao = 2540, upgrindx = 0;
GO
