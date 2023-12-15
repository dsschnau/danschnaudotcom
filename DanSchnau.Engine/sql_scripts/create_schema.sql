CREATE TABLE [dbo].[Blog] (
	[BlogId] [nvarchar](450) NOT NULL DEFAULT(newid()),
	[Title] [nvarchar](255) NOT NULL,
	[Slug] [nvarchar](255) NOT NULL,
    [Content] [nvarchar](max) NULL,
    [Published] [DateTime] NULL,
    [LastUpdated] [DateTime] NULL
CONSTRAINT [PK_Blog] PRIMARY KEY CLUSTERED
(
	[BlogId] ASC
)WITH (
	PAD_INDEX = OFF, 
	STATISTICS_NORECOMPUTE = OFF,
	IGNORE_DUP_KEY = OFF,
	ALLOW_ROW_LOCKS = ON,
	ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)