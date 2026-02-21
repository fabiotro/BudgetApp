USE [master]
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BudgetApp')
BEGIN
    CREATE DATABASE [BudgetApp]
END
GO

USE [BudgetApp]
GO

-- Tables

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Camp]') AND type = N'U')
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
    CONSTRAINT [PK_Camp] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Budget]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[Budget](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Description] [nvarchar](255) NULL,
    [CampId] [int] NOT NULL,
    [CreateDate] [datetime] NULL,
    [ChangeDate] [datetime] NULL,
    CONSTRAINT [PK_Budget] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[Category](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Description] [nvarchar](255) NULL,
    [SortIndex] [int] NOT NULL,
    [CreateDate] [datetime] NULL,
    [ChangeDate] [datetime] NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PositionType]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[PositionType](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Description] [nvarchar](100) NOT NULL,
    [CreateDate] [datetime] NULL,
    [ChangeDate] [datetime] NULL,
    CONSTRAINT [PK_PositionType] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubCategory]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[SubCategory](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [CategoryId] [int] NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Description] [nvarchar](255) NULL,
    [SortIndex] [int] NOT NULL,
    [CreateDate] [datetime] NULL,
    [ChangeDate] [datetime] NULL,
    CONSTRAINT [PK_SubCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Position]') AND type = N'U')
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
    CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemplateBudget]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[TemplateBudget](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Description] [nvarchar](255) NULL,
    [CreateDate] [datetime] NULL,
    [ChangeDate] [datetime] NULL,
    CONSTRAINT [PK_TemplateBudget] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemplatePosition]') AND type = N'U')
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
    CONSTRAINT [PK_TemplatePosition] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

-- Default constraints for CreateDate

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Camp_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[Camp] ADD CONSTRAINT [DF_Camp_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Budget_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[Budget] ADD CONSTRAINT [DF_Budget_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Category_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[Category] ADD CONSTRAINT [DF_Category_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_PositionType_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[PositionType] ADD CONSTRAINT [DF_PositionType_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_SubCategory_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[SubCategory] ADD CONSTRAINT [DF_SubCategory_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Position_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[Position] ADD CONSTRAINT [DF_Position_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_TemplateBudget_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[TemplateBudget] ADD CONSTRAINT [DF_TemplateBudget_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_TemplatePosition_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[TemplatePosition] ADD CONSTRAINT [DF_TemplatePosition_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO

-- Foreign keys

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Budget_Camp]'))
    ALTER TABLE [dbo].[Budget] WITH CHECK ADD CONSTRAINT [FK_Budget_Camp] FOREIGN KEY([CampId]) REFERENCES [dbo].[Camp] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SubCategory_Category]'))
    ALTER TABLE [dbo].[SubCategory] WITH CHECK ADD CONSTRAINT [FK_SubCategory_Category] FOREIGN KEY([CategoryId]) REFERENCES [dbo].[Category] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_Budget]'))
    ALTER TABLE [dbo].[Position] WITH CHECK ADD CONSTRAINT [FK_Position_Budget] FOREIGN KEY([BudgetId]) REFERENCES [dbo].[Budget] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_Category]'))
    ALTER TABLE [dbo].[Position] WITH CHECK ADD CONSTRAINT [FK_Position_Category] FOREIGN KEY([CategoryId]) REFERENCES [dbo].[Category] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_PositionType]'))
    ALTER TABLE [dbo].[Position] WITH CHECK ADD CONSTRAINT [FK_Position_PositionType] FOREIGN KEY([PositionTypeId]) REFERENCES [dbo].[PositionType] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Position_SubCategory]'))
    ALTER TABLE [dbo].[Position] WITH CHECK ADD CONSTRAINT [FK_Position_SubCategory] FOREIGN KEY([SubCategoryId]) REFERENCES [dbo].[SubCategory] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_TemplateBudget]'))
    ALTER TABLE [dbo].[TemplatePosition] WITH CHECK ADD CONSTRAINT [FK_TemplatePosition_TemplateBudget] FOREIGN KEY([TemplateBudgetId]) REFERENCES [dbo].[TemplateBudget] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_PositionType]'))
    ALTER TABLE [dbo].[TemplatePosition] WITH CHECK ADD CONSTRAINT [FK_TemplatePosition_PositionType] FOREIGN KEY([PositionTypeId]) REFERENCES [dbo].[PositionType] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_Category]'))
    ALTER TABLE [dbo].[TemplatePosition] WITH CHECK ADD CONSTRAINT [FK_TemplatePosition_Category] FOREIGN KEY([CategoryId]) REFERENCES [dbo].[Category] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplatePosition_SubCategory]'))
    ALTER TABLE [dbo].[TemplatePosition] WITH CHECK ADD CONSTRAINT [FK_TemplatePosition_SubCategory] FOREIGN KEY([SubCategoryId]) REFERENCES [dbo].[SubCategory] ([Id])
GO

-- Triggers for ChangeDate

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Camp_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[Camp_UpdateChangeDate] ON [dbo].[Camp] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Camp] SET ChangeDate = GETDATE() FROM Camp t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Budget_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[Budget_UpdateChangeDate] ON [dbo].[Budget] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Budget] SET ChangeDate = GETDATE() FROM Budget t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Category_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[Category_UpdateChangeDate] ON [dbo].[Category] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Category] SET ChangeDate = GETDATE() FROM Category t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[PositionType_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[PositionType_UpdateChangeDate] ON [dbo].[PositionType] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[PositionType] SET ChangeDate = GETDATE() FROM PositionType t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[SubCategory_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[SubCategory_UpdateChangeDate] ON [dbo].[SubCategory] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[SubCategory] SET ChangeDate = GETDATE() FROM SubCategory t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Position_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[Position_UpdateChangeDate] ON [dbo].[Position] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Position] SET ChangeDate = GETDATE() FROM Position t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TemplateBudget_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[TemplateBudget_UpdateChangeDate] ON [dbo].[TemplateBudget] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[TemplateBudget] SET ChangeDate = GETDATE() FROM TemplateBudget t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TemplatePosition_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[TemplatePosition_UpdateChangeDate] ON [dbo].[TemplatePosition] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[TemplatePosition] SET ChangeDate = GETDATE() FROM TemplatePosition t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO
