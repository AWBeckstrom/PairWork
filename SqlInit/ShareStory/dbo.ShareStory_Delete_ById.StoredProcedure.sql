/****** Object:  StoredProcedure [dbo].[ShareStory_Delete_ById]    Script Date: 8/31/2023 4:00:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[ShareStory_Delete_ById]
			@Id int			
	AS

	/*
	DECLARE @Id int = 1
	
	EXECUTE dbo.ShareStory_Delete_ById
			@Id

	*/

	BEGIN

		DELETE FROM [dbo].[ShareStory]
		 WHERE Id = @Id

    END
GO
