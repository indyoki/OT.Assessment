USE [Master]

CREATE DATABASE [OT_Assessment_DB]
CONTAINMENT = NONE
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [OT_Assessment_DB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [OT_Assessment_DB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [OT_Assessment_DB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [OT_Assessment_DB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [OT_Assessment_DB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [OT_Assessment_DB] SET ARITHABORT OFF 
GO
ALTER DATABASE [OT_Assessment_DB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [OT_Assessment_DB] SET QUERY_STORE = ON
GO
ALTER DATABASE [OT_Assessment_DB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200)
GO
USE [OT_Assessment_DB]
GO
/****** Object:  Table [dbo].[Casino]    Script Date: 2024/11/21 21:30:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Casino](
	[Theme] [varchar](50) NULL,
	[Provider] [varchar](50) NOT NULL,
	[GameName] [varchar](50) NOT NULL,
	[BrandId] [uniqueidentifier] NULL,
	[CountryCode] [varchar](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[Provider] ASC,
	[GameName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CasinoWager]    Script Date: 2024/11/21 21:30:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CasinoWager](
	[WagerId] [uniqueidentifier] NOT NULL,
	[AccountId] [uniqueidentifier] NULL,
	[Provider] [varchar](50) NULL,
	[TransactionId] [uniqueidentifier] NULL,
	[TransactionTypeId] [uniqueidentifier] NULL,
	[Amount] [decimal](18, 2) NULL,
	[GameName] [varchar](50) NULL,
	[NumberOfBets] [int] NULL,
	[CreatedDateTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[WagerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Player]    Script Date: 2024/11/21 21:30:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Player](
	[AccountId] [uniqueidentifier] NOT NULL,
	[Username] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CasinoWager]  WITH CHECK ADD FOREIGN KEY([AccountId])
REFERENCES [dbo].[Player] ([AccountId])
GO
ALTER TABLE [dbo].[CasinoWager]  WITH CHECK ADD FOREIGN KEY([Provider], [GameName])
REFERENCES [dbo].[Casino] ([Provider], [GameName])
GO
/****** Object:  StoredProcedure [dbo].[usp_GetCasinoWagersByAccountId]    Script Date: 2024/11/21 21:30:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_GetCasinoWagersByAccountId]
	@AccountId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT [WagerId], [GameName], [Provider], [Amount], [CreatedDateTime] 
	FROM [OT_Assessment_DB].[dbo].[CasinoWager]
	WHERE [AccountId] = @AccountId
	ORDER BY [CreatedDateTime] DESC
END
GO
/****** Object:  StoredProcedure [dbo].[usp_GetTopSpendingPlayers]    Script Date: 2024/11/21 21:30:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_GetTopSpendingPlayers] 
	@Count INT
AS
BEGIN
	SELECT TOP (@Count) cw.AccountId, p.Username, SUM(Amount) as totalAmountSpend 
	FROM [CasinoWager] as cw
	INNER JOIN [Player] as p on p.AccountId = cw.AccountId
	GROUP BY cw.AccountId, p.Username
END
GO
/****** Object:  StoredProcedure [dbo].[usp_InsertCasinoWager]    Script Date: 2024/11/21 21:30:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[usp_InsertCasinoWager] 
	  @AccountId UNIQUEIDENTIFIER
	, @Username VARCHAR (50)
	, @Theme varchar (50) 
	, @Provider varchar (50)
	, @GameName varchar (50) 
	, @CountryCode varchar (3)
	, @Amount decimal
	, @NumberOfBets int
	, @BrandId UNIQUEIDENTIFIER 
	, @WagerId UNIQUEIDENTIFIER 
	, @TransactionId UNIQUEIDENTIFIER 
	, @TransactionTypeId UNIQUEIDENTIFIER 
	, @CreatedDateTime DateTime
AS
BEGIN
	SET NOCOUNT ON
	---Insert player details
	IF NOT EXISTS (SELECT 1 FROM [OT_Assessment_DB].[dbo].[Player] WHERE [AccountId] = @AccountId)
	BEGIN
		INSERT INTO [OT_Assessment_DB].[dbo].[Player]
		VALUES(@AccountId, @Username);
	END
	
	---Insert casino and game details
	IF NOT EXISTS (SELECT 1 FROM [OT_Assessment_DB].[dbo].[Casino] WHERE [Provider] = @Provider AND [GameName] = @GameName)
	BEGIN
		INSERT INTO [OT_Assessment_DB].[dbo].[Casino]
		VALUES(@Theme, @Provider, @GameName, @BrandId, @CountryCode);
	END

	---Insert CasinoWager
	IF NOT EXISTS (SELECT 1 FROM [OT_Assessment_DB].[dbo].[CasinoWager] WHERE [WagerId] = @WagerId)
	BEGIN
		INSERT INTO [OT_Assessment_DB].[dbo].[CasinoWager]
		VALUES(@WagerId, @AccountId, @Provider, @TransactionId, @TransactionTypeId, @Amount, @GameName, @NumberOfBets, @CreatedDateTime);
	END
END
GO