USE [master]
GO
/****** Object:  Database [BudgetApp]    Script Date: 21.11.2025 13:36:26 ******/
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BudgetApp')
BEGIN
CREATE DATABASE [BudgetApp]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BudgetApp', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQL2019\MSSQL\DATA\BudgetApp.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BudgetApp_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQL2019\MSSQL\DATA\BudgetApp_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
END
GO
ALTER DATABASE [BudgetApp] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BudgetApp].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BudgetApp] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BudgetApp] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BudgetApp] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BudgetApp] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BudgetApp] SET ARITHABORT OFF 
GO
ALTER DATABASE [BudgetApp] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BudgetApp] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BudgetApp] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BudgetApp] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BudgetApp] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BudgetApp] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BudgetApp] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BudgetApp] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BudgetApp] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BudgetApp] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BudgetApp] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BudgetApp] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BudgetApp] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BudgetApp] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BudgetApp] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BudgetApp] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BudgetApp] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BudgetApp] SET RECOVERY FULL 
GO
ALTER DATABASE [BudgetApp] SET  MULTI_USER 
GO
ALTER DATABASE [BudgetApp] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BudgetApp] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BudgetApp] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BudgetApp] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [BudgetApp] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [BudgetApp] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [BudgetApp] SET QUERY_STORE = OFF
GO
USE [BudgetApp]
GO
/****** Object:  Table [dbo].[Budget]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Budget]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Budget](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CampId] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ChangeDate] [datetime] NULL,
 CONSTRAINT [PK_Budget] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Camp]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Camp]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Camp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[MainLeader] [nvarchar](255) NULL,
	[ParticipantsCount_fc] [int] NOT NULL,
	[js_PersonsCount_fc] [int] NOT NULL,
	[LeadersTeamCount_fc] [int] NOT NULL,
	[ParticipantsCount_rl] [int] NULL,
	[js_PersonsCount_rl] [int] NULL,
	[LeadersTeamCount_rl] [int] NULL,
	[CreateDate] [datetime] NULL,
	[ChangeDate] [datetime] NULL,
 CONSTRAINT [PK_Camp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Category]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[SortIndex] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ChangeDate] [datetime] NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Position]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Position]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Position](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BudgetId] [int] NOT NULL,
	[PositionTypeId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[SubCategoryId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[FixedAmount_fc] [decimal](18, 2) NOT NULL,
	[Quantity_fc] [decimal](18, 2) NOT NULL,
	[UnitAmount_fc] [decimal](18, 2) NOT NULL,
	[FixedAmount_rl] [decimal](18, 2) NULL,
	[Quantity_rl] [decimal](18, 2) NULL,
	[UnitAmount_rl] [decimal](18, 2) NULL,
	[SortIndex] [int] NULL,
	[CreateDate] [datetime] NULL,
	[ChangeDate] [datetime] NULL,
 CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[PositionType]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PositionType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PositionType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[CreateDate] [datetime] NULL,
	[ChangeDate] [datetime] NULL,
 CONSTRAINT [PK_PositionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[SubCategory]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubCategory]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SubCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[SortIndex] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ChangeDate] [datetime] NULL,
 CONSTRAINT [PK_SubCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[TemplateBudget]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemplateBudget]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TemplateBudget](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CreateDate] [datetime] NULL,
	[ChangeDate] [datetime] NULL,
 CONSTRAINT [PK_TemplateBudget] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[TemplatePosition]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemplatePosition]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TemplatePosition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateBudgetId] [int] NOT NULL,
	[PositionTypeId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[SubCategoryId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[FixedAmount] [decimal](18, 2) NULL,
	[Quantity] [decimal](18, 2) NULL,
	[UnitAmount] [decimal](18, 2) NULL,
	[SortIndex] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ChangeDate] [datetime] NULL,
 CONSTRAINT [PK_TemplatePosition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Budget_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Budget] ADD  CONSTRAINT [DF_Budget_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Camp_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Camp] ADD  CONSTRAINT [DF_Camp_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Category_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Category] ADD  CONSTRAINT [DF_Category_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Position_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Position] ADD  CONSTRAINT [DF_Position_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_PositionType_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[PositionType] ADD  CONSTRAINT [DF_PositionType_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_SubCategory_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SubCategory] ADD  CONSTRAINT [DF_SubCategory_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_TemplateBudget_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[TemplateBudget] ADD  CONSTRAINT [DF_TemplateBudget_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_TemplatePosition_CreateDate]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[TemplatePosition] ADD  CONSTRAINT [DF_TemplatePosition_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Budget_Camp]') AND parent_object_id = OBJECT_ID(N'[dbo].[Budget]'))
ALTER TABLE [dbo].[Budget]  WITH CHECK ADD  CONSTRAINT [FK_Budget_Camp] FOREIGN KEY([CampId])
REFERENCES [dbo].[Camp] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Budget_Camp]') AND parent_object_id = OBJECT_ID(N'[dbo].[Budget]'))
ALTER TABLE [dbo].[Budget] CHECK CONSTRAINT [FK_Budget_Camp]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_Budget]') AND parent_object_id = OBJECT_ID(N'[dbo].[Position]'))
ALTER TABLE [dbo].[Position]  WITH CHECK ADD  CONSTRAINT [FK_Position_Budget] FOREIGN KEY([BudgetId])
REFERENCES [dbo].[Budget] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_Budget]') AND parent_object_id = OBJECT_ID(N'[dbo].[Position]'))
ALTER TABLE [dbo].[Position] CHECK CONSTRAINT [FK_Position_Budget]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[Position]'))
ALTER TABLE [dbo].[Position]  WITH CHECK ADD  CONSTRAINT [FK_Position_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[Position]'))
ALTER TABLE [dbo].[Position] CHECK CONSTRAINT [FK_Position_Category]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_PositionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Position]'))
ALTER TABLE [dbo].[Position]  WITH CHECK ADD  CONSTRAINT [FK_Position_PositionType] FOREIGN KEY([PositionTypeId])
REFERENCES [dbo].[PositionType] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_PositionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[Position]'))
ALTER TABLE [dbo].[Position] CHECK CONSTRAINT [FK_Position_PositionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_SubCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[Position]'))
ALTER TABLE [dbo].[Position]  WITH CHECK ADD  CONSTRAINT [FK_Position_SubCategory] FOREIGN KEY([SubCategoryId])
REFERENCES [dbo].[SubCategory] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_SubCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[Position]'))
ALTER TABLE [dbo].[Position] CHECK CONSTRAINT [FK_Position_SubCategory]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SubCategory_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[SubCategory]'))
ALTER TABLE [dbo].[SubCategory]  WITH CHECK ADD  CONSTRAINT [FK_SubCategory_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SubCategory_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[SubCategory]'))
ALTER TABLE [dbo].[SubCategory] CHECK CONSTRAINT [FK_SubCategory_Category]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[TemplatePosition]'))
ALTER TABLE [dbo].[TemplatePosition]  WITH CHECK ADD  CONSTRAINT [FK_TemplatePosition_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_Category]') AND parent_object_id = OBJECT_ID(N'[dbo].[TemplatePosition]'))
ALTER TABLE [dbo].[TemplatePosition] CHECK CONSTRAINT [FK_TemplatePosition_Category]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_PositionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[TemplatePosition]'))
ALTER TABLE [dbo].[TemplatePosition]  WITH CHECK ADD  CONSTRAINT [FK_TemplatePosition_PositionType] FOREIGN KEY([PositionTypeId])
REFERENCES [dbo].[PositionType] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_PositionType]') AND parent_object_id = OBJECT_ID(N'[dbo].[TemplatePosition]'))
ALTER TABLE [dbo].[TemplatePosition] CHECK CONSTRAINT [FK_TemplatePosition_PositionType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_SubCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[TemplatePosition]'))
ALTER TABLE [dbo].[TemplatePosition]  WITH CHECK ADD  CONSTRAINT [FK_TemplatePosition_SubCategory] FOREIGN KEY([SubCategoryId])
REFERENCES [dbo].[SubCategory] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_SubCategory]') AND parent_object_id = OBJECT_ID(N'[dbo].[TemplatePosition]'))
ALTER TABLE [dbo].[TemplatePosition] CHECK CONSTRAINT [FK_TemplatePosition_SubCategory]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_TemplateBudget]') AND parent_object_id = OBJECT_ID(N'[dbo].[TemplatePosition]'))
ALTER TABLE [dbo].[TemplatePosition]  WITH CHECK ADD  CONSTRAINT [FK_TemplatePosition_TemplateBudget] FOREIGN KEY([TemplateBudgetId])
REFERENCES [dbo].[TemplateBudget] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_TemplateBudget]') AND parent_object_id = OBJECT_ID(N'[dbo].[TemplatePosition]'))
ALTER TABLE [dbo].[TemplatePosition] CHECK CONSTRAINT [FK_TemplatePosition_TemplateBudget]
GO
/****** Object:  Trigger [dbo].[Budget_UpdateChangeDate]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Budget_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[Budget_UpdateChangeDate]
       ON [dbo].[Budget] AFTER INSERT, UPDATE AS
       BEGIN
       SET NOCOUNT ON;
       UPDATE [dbo].[Budget]
    SET ChangeDate = GETDATE()
    FROM Budget t
       INNER JOIN Inserted i
       ON t.Id = i.Id
       END' 
GO
ALTER TABLE [dbo].[Budget] ENABLE TRIGGER [Budget_UpdateChangeDate]
GO
/****** Object:  Trigger [dbo].[Camp_UpdateChangeDate]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Camp_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[Camp_UpdateChangeDate]
       ON [dbo].[Camp] AFTER INSERT, UPDATE AS
       BEGIN
       SET NOCOUNT ON;
       UPDATE [dbo].[Camp]
    SET ChangeDate = GETDATE()
    FROM Camp t
       INNER JOIN Inserted i
       ON t.Id = i.Id
       END' 
GO
ALTER TABLE [dbo].[Camp] ENABLE TRIGGER [Camp_UpdateChangeDate]
GO
/****** Object:  Trigger [dbo].[Category_UpdateChangeDate]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Category_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[Category_UpdateChangeDate]
       ON [dbo].[Category] AFTER INSERT, UPDATE AS
       BEGIN
       SET NOCOUNT ON;
       UPDATE [dbo].[Category]
    SET ChangeDate = GETDATE()
    FROM Category t
       INNER JOIN Inserted i
       ON t.Id = i.Id
       END' 
GO
ALTER TABLE [dbo].[Category] ENABLE TRIGGER [Category_UpdateChangeDate]
GO
/****** Object:  Trigger [dbo].[Position_UpdateChangeDate]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Position_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[Position_UpdateChangeDate]
       ON [dbo].[Position] AFTER INSERT, UPDATE AS
       BEGIN
       SET NOCOUNT ON;
       UPDATE [dbo].[Position]
    SET ChangeDate = GETDATE()
    FROM Position t
       INNER JOIN Inserted i
       ON t.Id = i.Id
       END' 
GO
ALTER TABLE [dbo].[Position] ENABLE TRIGGER [Position_UpdateChangeDate]
GO
/****** Object:  Trigger [dbo].[PositionType_UpdateChangeDate]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[PositionType_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[PositionType_UpdateChangeDate]
       ON [dbo].[PositionType] AFTER INSERT, UPDATE AS
       BEGIN
       SET NOCOUNT ON;
       UPDATE [dbo].[PositionType]
    SET ChangeDate = GETDATE()
    FROM PositionType t
       INNER JOIN Inserted i
       ON t.Id = i.Id
       END' 
GO
ALTER TABLE [dbo].[PositionType] ENABLE TRIGGER [PositionType_UpdateChangeDate]
GO
/****** Object:  Trigger [dbo].[SubCategory_UpdateChangeDate]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[SubCategory_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[SubCategory_UpdateChangeDate]
       ON [dbo].[SubCategory] AFTER INSERT, UPDATE AS
       BEGIN
       SET NOCOUNT ON;
       UPDATE [dbo].[SubCategory]
    SET ChangeDate = GETDATE()
    FROM SubCategory t
       INNER JOIN Inserted i
       ON t.Id = i.Id
       END' 
GO
ALTER TABLE [dbo].[SubCategory] ENABLE TRIGGER [SubCategory_UpdateChangeDate]
GO
/****** Object:  Trigger [dbo].[TemplateBudget_UpdateChangeDate]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TemplateBudget_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TemplateBudget_UpdateChangeDate]
       ON [dbo].[TemplateBudget] AFTER INSERT, UPDATE AS
       BEGIN
       SET NOCOUNT ON;
       UPDATE [dbo].[TemplateBudget]
    SET ChangeDate = GETDATE()
    FROM TemplateBudget t
       INNER JOIN Inserted i
       ON t.Id = i.Id
       END' 
GO
ALTER TABLE [dbo].[TemplateBudget] ENABLE TRIGGER [TemplateBudget_UpdateChangeDate]
GO
/****** Object:  Trigger [dbo].[TemplatePosition_UpdateChangeDate]    Script Date: 21.11.2025 13:36:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TemplatePosition_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TemplatePosition_UpdateChangeDate]
       ON [dbo].[TemplatePosition] AFTER INSERT, UPDATE AS
       BEGIN
       SET NOCOUNT ON;
       UPDATE [dbo].[TemplatePosition]
    SET ChangeDate = GETDATE()
    FROM TemplatePosition t
       INNER JOIN Inserted i
       ON t.Id = i.Id
       END' 
GO
ALTER TABLE [dbo].[TemplatePosition] ENABLE TRIGGER [TemplatePosition_UpdateChangeDate]
GO
USE [master]
GO
ALTER DATABASE [BudgetApp] SET  READ_WRITE 
GO
