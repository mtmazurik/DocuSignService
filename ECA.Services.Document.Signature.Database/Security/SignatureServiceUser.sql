CREATE ROLE [SignatureServiceUser]
	AUTHORIZATION [dbo];
GO

grant select on schema :: dbo to SignatureServiceUser;
GO
grant update on schema :: dbo to SignatureServiceUser;
GO
grant insert on schema :: dbo to SignatureServiceUser;
GO
grant delete on schema :: dbo to SignatureServiceUser;
GO
grant execute on schema :: dbo to SignatureServiceUser;
GO
