-- =============================================
-- Author:		Warren Chen
-- Create date: 2014/4/22
-- Description:	Save image to DB and return id
-- =============================================
CREATE PROCEDURE [dbo].[sp_SaveImage]
	@ImageContent varbinary(MAX)
AS
BEGIN
	INSERT INTO Images (ImageContent, ExpiredTime)
	VALUES (@ImageContent, DATEADD(MINUTE, 20, GETDATE()))

	RETURN @@IDENTITY
END
