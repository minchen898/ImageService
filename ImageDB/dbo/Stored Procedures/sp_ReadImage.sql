-- =============================================
-- Author:		Warren Chen
-- Create date: 2014/4/22
-- Description:	Load image by ID
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReadImage]
	@ImageId int
AS
BEGIN
	SELECT ImageContent FROM Images
	WHERE ImageID = @ImageId
	 AND ExpiredTime >= GETDATE()
END
