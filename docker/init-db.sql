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

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[User](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Email] [nvarchar](255) NOT NULL,
    [DisplayName] [nvarchar](100) NOT NULL,
    [PasswordHash] [nvarchar](500) NOT NULL,
    [CreateDate] [datetime] NULL,
    [ChangeDate] [datetime] NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_User_Email] UNIQUE ([Email])
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Camp]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[Camp](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [StartDate] [datetime] NOT NULL,
    [EndDate] [datetime] NOT NULL,
    [CreatedByUserId] [int] NOT NULL,
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

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CampUser]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[CampUser](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [CampId] [int] NOT NULL,
    [UserId] [int] NOT NULL,
    [IsMainLeader] [bit] NOT NULL DEFAULT 0,
    [CreateDate] [datetime] NULL,
    CONSTRAINT [PK_CampUser] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_CampUser_CampId_UserId] UNIQUE ([CampId], [UserId])
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
    [SubCategoryId] [int] NULL,
    [Name] [nvarchar](255) NOT NULL,
    [FixedAmount_fc] [decimal](18, 2) NOT NULL,
    [Quantity_fc] [decimal](18, 2) NOT NULL,
    [UnitAmount_fc] [decimal](18, 2) NOT NULL,
    [FixedAmount_rl] [decimal](18, 2) NULL,
    [Quantity_rl] [decimal](18, 2) NULL,
    [UnitAmount_rl] [decimal](18, 2) NULL,
    [QuantityVar_fc] [nvarchar](50) NULL,
    [QuantityVar_rl] [nvarchar](50) NULL,
    [SortIndex] [int] NULL,
    [CreateDate] [datetime] NULL,
    [ChangeDate] [datetime] NULL,
    CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Position]') AND name = 'QuantityVar_fc')
    ALTER TABLE [dbo].[Position] ADD [QuantityVar_fc] [nvarchar](50) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Position]') AND name = 'QuantityVar_rl')
    ALTER TABLE [dbo].[Position] ADD [QuantityVar_rl] [nvarchar](50) NULL;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemplateBudget]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[TemplateBudget](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Description] [nvarchar](255) NULL,
    [CreatedByUserId] [int] NOT NULL,
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
    [SubCategoryId] [int] NULL,
    [Name] [nvarchar](255) NOT NULL,
    [FixedAmount] [decimal](18, 2) NULL,
    [Quantity] [decimal](18, 2) NULL,
    [QuantityVar] [nvarchar](50) NULL,
    [UnitAmount] [decimal](18, 2) NULL,
    [SortIndex] [int] NOT NULL,
    [CreateDate] [datetime] NULL,
    [ChangeDate] [datetime] NULL,
    CONSTRAINT [PK_TemplatePosition] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TemplatePosition]') AND name = 'QuantityVar')
    ALTER TABLE [dbo].[TemplatePosition] ADD [QuantityVar] [nvarchar](50) NULL
GO

-- Default constraints for CreateDate

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_User_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[User] ADD CONSTRAINT [DF_User_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_CampUser_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[CampUser] ADD CONSTRAINT [DF_CampUser_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
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

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Camp_User]'))
    ALTER TABLE [dbo].[Camp] WITH CHECK ADD CONSTRAINT [FK_Camp_User] FOREIGN KEY([CreatedByUserId]) REFERENCES [dbo].[User] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CampUser_Camp]'))
    ALTER TABLE [dbo].[CampUser] WITH CHECK ADD CONSTRAINT [FK_CampUser_Camp] FOREIGN KEY([CampId]) REFERENCES [dbo].[Camp] ([Id]) ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CampUser_User]'))
    ALTER TABLE [dbo].[CampUser] WITH CHECK ADD CONSTRAINT [FK_CampUser_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TemplateBudget_User]'))
    ALTER TABLE [dbo].[TemplateBudget] WITH CHECK ADD CONSTRAINT [FK_TemplateBudget_User] FOREIGN KEY([CreatedByUserId]) REFERENCES [dbo].[User] ([Id])
GO
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

-- Seed data

IF NOT EXISTS (SELECT 1 FROM [dbo].[PositionType] WHERE [Name] = N'Ausgabe')
    INSERT INTO [dbo].[PositionType] ([Name], [Description]) VALUES (N'Ausgabe', N'Ausgabe')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[PositionType] WHERE [Name] = N'Einnahme')
    INSERT INTO [dbo].[PositionType] ([Name], [Description]) VALUES (N'Einnahme', N'Einnahme')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Rekognoszierung')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Rekognoszierung', 1)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Unterkunft')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Unterkunft', 2)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Verpflegungskosten')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Verpflegungskosten', 3)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Reise- und Transportkosten')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Reise- und Transportkosten', 4)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Programmkosten')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Programmkosten', 5)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Materialkosten')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Materialkosten', 6)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Organisationskosten')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Organisationskosten', 7)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Lagerauswertung')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Lagerauswertung', 8)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Unvorhergesehenes')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Unvorhergesehenes', 9)
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Category] WHERE [Name] = N'Einnahmen')
    INSERT INTO [dbo].[Category] ([Name], [SortIndex]) VALUES (N'Einnahmen', 10)
GO

-- Triggers for ChangeDate

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[User_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[User_UpdateChangeDate] ON [dbo].[User] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[User] SET ChangeDate = GETDATE() FROM [User] t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

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

-- =============================================
-- Expense Tracking Feature
-- =============================================

-- New columns on [User] table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND name = 'FirstName')
    ALTER TABLE [dbo].[User] ADD [FirstName] [nvarchar](100) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND name = 'LastName')
    ALTER TABLE [dbo].[User] ADD [LastName] [nvarchar](100) NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND name = 'IBAN')
    ALTER TABLE [dbo].[User] ADD [IBAN] [nvarchar](34) NULL;
GO

-- New column on [Camp] table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Camp]') AND name = 'MainLeader')
    ALTER TABLE [dbo].[Camp] ADD [MainLeader] [nvarchar](255) NULL;
GO

-- [Transaction] table (Transaction is a reserved word, always use brackets)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Transaction]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[Transaction](
    [Id]                [int] IDENTITY(1,1) NOT NULL,
    [CampId]            [int] NOT NULL,
    [PerformedByUserId] [int] NOT NULL,
    [PositionId]        [int] NULL,
    [Name]              [nvarchar](255) NOT NULL,
    [Description]       [nvarchar](500) NULL,
    [Amount]            [decimal](18, 2) NOT NULL,
    [PaymentSource]     [int] NOT NULL,
    [PaymentMethod]     [int] NOT NULL,
    [CreateDate]        [datetime] NULL,
    [ChangeDate]        [datetime] NULL,
    CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

-- [TransactionDocument] table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransactionDocument]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[TransactionDocument](
    [Id]            [int] IDENTITY(1,1) NOT NULL,
    [TransactionId] [int] NOT NULL,
    [FileName]      [nvarchar](255) NOT NULL,
    [ContentType]   [nvarchar](100) NOT NULL,
    [FileSize]      [int] NOT NULL,
    [FileData]      [varbinary](max) NOT NULL,
    [CreateDate]    [datetime] NULL,
    [ChangeDate]    [datetime] NULL,
    CONSTRAINT [PK_TransactionDocument] PRIMARY KEY CLUSTERED ([Id] ASC)
)
END
GO

-- CreateDate default constraints
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_Transaction_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[Transaction] ADD CONSTRAINT [DF_Transaction_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_TransactionDocument_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[TransactionDocument] ADD CONSTRAINT [DF_TransactionDocument_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO

-- Foreign keys
-- CampId CASCADE: deleting a camp removes all its transactions
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transaction_Camp]'))
    ALTER TABLE [dbo].[Transaction] WITH CHECK ADD CONSTRAINT [FK_Transaction_Camp]
        FOREIGN KEY([CampId]) REFERENCES [dbo].[Camp] ([Id]) ON DELETE CASCADE
GO
-- PerformedByUserId NO ACTION: prevent deleting a user who has transactions
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transaction_User]'))
    ALTER TABLE [dbo].[Transaction] WITH CHECK ADD CONSTRAINT [FK_Transaction_User]
        FOREIGN KEY([PerformedByUserId]) REFERENCES [dbo].[User] ([Id])
GO
-- PositionId SET NULL: position deletion nulls out FK, preserves transaction history
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Transaction_Position]'))
    ALTER TABLE [dbo].[Transaction] WITH CHECK ADD CONSTRAINT [FK_Transaction_Position]
        FOREIGN KEY([PositionId]) REFERENCES [dbo].[Position] ([Id]) ON DELETE SET NULL
GO
-- TransactionId CASCADE: deleting a transaction removes its documents
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransactionDocument_Transaction]'))
    ALTER TABLE [dbo].[TransactionDocument] WITH CHECK ADD CONSTRAINT [FK_TransactionDocument_Transaction]
        FOREIGN KEY([TransactionId]) REFERENCES [dbo].[Transaction] ([Id]) ON DELETE CASCADE
GO

-- ChangeDate triggers
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Transaction_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[Transaction_UpdateChangeDate] ON [dbo].[Transaction] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Transaction] SET ChangeDate = GETDATE() FROM [Transaction] t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TransactionDocument_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[TransactionDocument_UpdateChangeDate] ON [dbo].[TransactionDocument] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[TransactionDocument] SET ChangeDate = GETDATE() FROM [TransactionDocument] t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO

-- =============================================
-- Camp Invite Feature
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CampInvite]') AND type = N'U')
BEGIN
CREATE TABLE [dbo].[CampInvite](
    [Id]               [int] IDENTITY(1,1) NOT NULL,
    [CampId]           [int] NOT NULL,
    [InvitedByUserId]  [int] NOT NULL,
    [InvitedUserId]    [int] NOT NULL,
    [Status]           [int] NOT NULL DEFAULT 0,
    [CreateDate]       [datetime] NULL,
    [ChangeDate]       [datetime] NULL,
    CONSTRAINT [PK_CampInvite] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_CampInvite_CampId_InvitedUserId] UNIQUE ([CampId], [InvitedUserId])
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DF_CampInvite_CreateDate]') AND type = 'D')
    ALTER TABLE [dbo].[CampInvite] ADD CONSTRAINT [DF_CampInvite_CreateDate] DEFAULT (GETDATE()) FOR [CreateDate]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CampInvite_Camp]'))
    ALTER TABLE [dbo].[CampInvite] WITH CHECK ADD CONSTRAINT [FK_CampInvite_Camp]
        FOREIGN KEY([CampId]) REFERENCES [dbo].[Camp] ([Id]) ON DELETE CASCADE
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CampInvite_InvitedByUser]'))
    ALTER TABLE [dbo].[CampInvite] WITH CHECK ADD CONSTRAINT [FK_CampInvite_InvitedByUser]
        FOREIGN KEY([InvitedByUserId]) REFERENCES [dbo].[User] ([Id])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CampInvite_InvitedUser]'))
    ALTER TABLE [dbo].[CampInvite] WITH CHECK ADD CONSTRAINT [FK_CampInvite_InvitedUser]
        FOREIGN KEY([InvitedUserId]) REFERENCES [dbo].[User] ([Id])
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[CampInvite_UpdateChangeDate]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[CampInvite_UpdateChangeDate] ON [dbo].[CampInvite] AFTER INSERT, UPDATE AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[CampInvite] SET ChangeDate = GETDATE() FROM [CampInvite] t INNER JOIN Inserted i ON t.Id = i.Id
END'
GO
