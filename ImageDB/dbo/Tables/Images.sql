CREATE TABLE [dbo].[Images] (
    [ImageID]      INT             IDENTITY (1, 1) NOT NULL,
    [ImageContent] VARBINARY (MAX) NULL,
    [ExpiredTime]  DATETIME        NULL,
    CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED ([ImageID] ASC)
);

