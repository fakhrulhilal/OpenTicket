/****** Object:  Table [dbo].[Customers] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NOT NULL,
	[RegisteredDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmailAccounts] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailAccounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[DraftId] [int] NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](max) NULL,
	[AccessToken] [nvarchar](max) NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[LastUpdateAccessToken] [datetime] NULL,
	[ServerPort] [int] NOT NULL,
	[ServerAddress] [nvarchar](50) NOT NULL,
	[MailBox] [nvarchar](255) NULL,
	[UseSecureConnection] [bit] NOT NULL,
	[Protocol] [smallint] NOT NULL,
 CONSTRAINT [PK_EmailAccounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tickets] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tickets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailAccountId] [int] NULL,
	[CustomerId] [int] NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[Question] [nvarchar](max) NULL,
	[Answer] [nvarchar](max) NULL,
	[CreatedDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Tickets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
GO
ALTER TABLE [dbo].[Customers] ADD  CONSTRAINT [DF_Customers_RegisteredDateTime]  DEFAULT (getutcdate()) FOR [RegisteredDateTime]
GO
ALTER TABLE [dbo].[EmailAccounts] ADD  CONSTRAINT [DF_EmailAccounts_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[EmailAccounts] ADD  CONSTRAINT [DF_EmailAccounts_UseSecureConnection]  DEFAULT ((0)) FOR [UseSecureConnection]
GO
ALTER TABLE [dbo].[Tickets]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_ImportedEmail] FOREIGN KEY([EmailAccountId])
REFERENCES [dbo].[EmailAccounts] ([Id])
GO
ALTER TABLE [dbo].[Tickets] CHECK CONSTRAINT [FK_Ticket_ImportedEmail]
GO
ALTER TABLE [dbo].[Tickets]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Owner] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Tickets] CHECK CONSTRAINT [FK_Ticket_Owner]
GO
