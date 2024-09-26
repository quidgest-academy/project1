-- Framework change routines

-- Migrate data from field NIVEL to field ROLE
IF (0 < (SELECT COUNT(*) FROM UserAuthorization WHERE ROLE IS NULL))
BEGIN
	UPDATE UserAuthorization
	SET ROLE = NIVEL
	WHERE NIVEL <> 0
END
GO
