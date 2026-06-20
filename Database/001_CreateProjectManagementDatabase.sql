/*
    ProjectManagementApp - Independent Database
    Creates the standalone project-management database without strategic-plan tables.
*/

IF DB_ID(N'ProjectManagementApp') IS NULL
BEGIN
    CREATE DATABASE [ProjectManagementApp];
END
GO

USE [ProjectManagementApp];
GO

CREATE TABLE [dbo].[EmployeeAccount]
(
    [EmployeeId] INT NOT NULL PRIMARY KEY,
    [EmployeeCode] NVARCHAR(50) NOT NULL UNIQUE,
    [EmployeeNameAr] NVARCHAR(250) NOT NULL,
    [EmployeeNameEn] NVARCHAR(250) NULL,
    [Email] NVARCHAR(250) NULL,
    [DepartmentCode] INT NULL,
    [DepartmentName] NVARCHAR(250) NULL,
    [PositionName] NVARCHAR(250) NULL,
    [PasswordHash] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_EmployeeAccount_IsActive] DEFAULT (1),
    [LastLoginOn] DATETIME2 NULL,
    [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_EmployeeAccount_CreatedOn] DEFAULT (SYSDATETIME()),
    [ModifiedOn] DATETIME2 NOT NULL CONSTRAINT [DF_EmployeeAccount_ModifiedOn] DEFAULT (SYSDATETIME())
);
GO

CREATE TABLE [dbo].[AppRole]
(
    [RoleId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [RoleCode] NVARCHAR(100) NOT NULL UNIQUE,
    [RoleNameAr] NVARCHAR(200) NOT NULL,
    [RoleNameEn] NVARCHAR(200) NULL
);
GO

CREATE TABLE [dbo].[EmployeeAccountRole]
(
    [EmployeeId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    CONSTRAINT [PK_EmployeeAccountRole] PRIMARY KEY ([EmployeeId], [RoleId]),
    CONSTRAINT [FK_EmployeeAccountRole_EmployeeAccount] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[EmployeeAccount]([EmployeeId]),
    CONSTRAINT [FK_EmployeeAccountRole_AppRole] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AppRole]([RoleId])
);
GO

CREATE TABLE [dbo].[Projects]
(
    [ProjectId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Projects_ProjectId] DEFAULT (NEWID()) PRIMARY KEY,
    [ProjectName] NVARCHAR(300) NOT NULL,
    [ProjectCode] NVARCHAR(100) NULL,
    [ProjectType] NVARCHAR(100) NULL,
    [DepartmentCode] INT NULL,
    [DepartmentName] NVARCHAR(250) NULL,
    [ResponsibleUserId] INT NULL,
    [StartDate] DATE NULL,
    [EndDate] DATE NULL,
    [ProjectStatus] NVARCHAR(50) NOT NULL CONSTRAINT [DF_Projects_ProjectStatus] DEFAULT (N'Active'),
    [Approved] BIT NOT NULL CONSTRAINT [DF_Projects_Approved] DEFAULT (1),
    [CurrentProgressPercent] DECIMAL(9,2) NOT NULL CONSTRAINT [DF_Projects_CurrentProgressPercent] DEFAULT (0),
    [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_Projects_CreatedOn] DEFAULT (SYSDATETIME()),
    [CreatedBy] INT NULL,
    [ModifiedOn] DATETIME2 NOT NULL CONSTRAINT [DF_Projects_ModifiedOn] DEFAULT (SYSDATETIME()),
    [ModifiedBy] INT NULL,
    CONSTRAINT [FK_Projects_ResponsibleUser] FOREIGN KEY ([ResponsibleUserId]) REFERENCES [dbo].[EmployeeAccount]([EmployeeId])
);
GO

CREATE TABLE [dbo].[ProjectTeamMembers]
(
    [ProjectTeamMemberId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProjectId] UNIQUEIDENTIFIER NOT NULL,
    [EmployeeId] INT NOT NULL,
    [MemberRole] NVARCHAR(100) NULL,
    [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_ProjectTeamMembers_CreatedOn] DEFAULT (SYSDATETIME()),
    CONSTRAINT [FK_ProjectTeamMembers_Projects] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects]([ProjectId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProjectTeamMembers_EmployeeAccount] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[EmployeeAccount]([EmployeeId]),
    CONSTRAINT [UX_ProjectTeamMembers_Project_Employee] UNIQUE ([ProjectId], [EmployeeId])
);
GO

CREATE TABLE [dbo].[ProjectPhase]
(
    [PhaseId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ProjectId] UNIQUEIDENTIFIER NOT NULL,
    [PhaseName] NVARCHAR(250) NOT NULL,
    [PhaseCode] NVARCHAR(50) NULL,
    [DisplayOrder] INT NOT NULL CONSTRAINT [DF_ProjectPhase_DisplayOrder] DEFAULT (1),
    [PhaseStatus] NVARCHAR(50) NOT NULL CONSTRAINT [DF_ProjectPhase_PhaseStatus] DEFAULT (N'NotStarted'),
    [PlannedStartDate] DATE NULL,
    [PlannedEndDate] DATE NULL,
    [ActualStartDate] DATE NULL,
    [ActualEndDate] DATE NULL,
    [Notes] NVARCHAR(MAX) NULL,
    [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_ProjectPhase_CreatedOn] DEFAULT (SYSDATETIME()),
    [CreatedBy] INT NULL,
    [ModifiedOn] DATETIME2 NOT NULL CONSTRAINT [DF_ProjectPhase_ModifiedOn] DEFAULT (SYSDATETIME()),
    [ModifiedBy] INT NULL,
    CONSTRAINT [FK_ProjectPhase_Projects] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects]([ProjectId]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[ProjectPhaseActivity]
(
    [PhaseActivityId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PhaseId] INT NOT NULL,
    [ProjectId] UNIQUEIDENTIFIER NOT NULL,
    [ActivityName] NVARCHAR(300) NOT NULL,
    [IndicatorName] NVARCHAR(300) NULL,
    [EstimatedCost] DECIMAL(18,2) NULL,
    [TargetValue] NVARCHAR(200) NULL,
    [VerificationSource] NVARCHAR(300) NULL,
    [RisksAndAssumptions] NVARCHAR(MAX) NULL,
    [StartDate] DATE NULL,
    [EndDate] DATE NULL,
    [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_ProjectPhaseActivity_CreatedOn] DEFAULT (SYSDATETIME()),
    [CreatedBy] INT NULL,
    [ModifiedOn] DATETIME2 NOT NULL CONSTRAINT [DF_ProjectPhaseActivity_ModifiedOn] DEFAULT (SYSDATETIME()),
    [ModifiedBy] INT NULL,
    CONSTRAINT [FK_ProjectPhaseActivity_ProjectPhase] FOREIGN KEY ([PhaseId]) REFERENCES [dbo].[ProjectPhase]([PhaseId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProjectPhaseActivity_Projects] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects]([ProjectId])
);
GO

CREATE TABLE [dbo].[ProjectPhaseTask]
(
    [PhaseTaskId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PhaseActivityId] INT NOT NULL,
    [ProjectId] UNIQUEIDENTIFIER NOT NULL,
    [TaskName] NVARCHAR(300) NOT NULL,
    [StartDate] DATE NULL,
    [EndDate] DATE NULL,
    [Priority] NVARCHAR(50) NOT NULL CONSTRAINT [DF_ProjectPhaseTask_Priority] DEFAULT (N'Medium'),
    [PercentComplete] INT NOT NULL CONSTRAINT [DF_ProjectPhaseTask_PercentComplete] DEFAULT (0),
    [Notes] NVARCHAR(MAX) NULL,
    [DependentOnPhaseTaskId] INT NULL,
    [DependencyType] NVARCHAR(50) NULL,
    [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_ProjectPhaseTask_CreatedOn] DEFAULT (SYSDATETIME()),
    [CreatedBy] INT NULL,
    [ModifiedOn] DATETIME2 NOT NULL CONSTRAINT [DF_ProjectPhaseTask_ModifiedOn] DEFAULT (SYSDATETIME()),
    [ModifiedBy] INT NULL,
    CONSTRAINT [FK_ProjectPhaseTask_ProjectPhaseActivity] FOREIGN KEY ([PhaseActivityId]) REFERENCES [dbo].[ProjectPhaseActivity]([PhaseActivityId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProjectPhaseTask_Projects] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects]([ProjectId]),
    CONSTRAINT [FK_ProjectPhaseTask_Dependency] FOREIGN KEY ([DependentOnPhaseTaskId]) REFERENCES [dbo].[ProjectPhaseTask]([PhaseTaskId]),
    CONSTRAINT [CK_ProjectPhaseTask_PercentComplete] CHECK ([PercentComplete] BETWEEN 0 AND 100)
);
GO

CREATE TABLE [dbo].[ProjectPhaseTaskAssignee]
(
    [PhaseTaskAssigneeId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PhaseTaskId] INT NOT NULL,
    [EmployeeId] INT NOT NULL,
    [CreatedOn] DATETIME2 NOT NULL CONSTRAINT [DF_ProjectPhaseTaskAssignee_CreatedOn] DEFAULT (SYSDATETIME()),
    CONSTRAINT [FK_ProjectPhaseTaskAssignee_Task] FOREIGN KEY ([PhaseTaskId]) REFERENCES [dbo].[ProjectPhaseTask]([PhaseTaskId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProjectPhaseTaskAssignee_Employee] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[EmployeeAccount]([EmployeeId]),
    CONSTRAINT [UX_ProjectPhaseTaskAssignee_Task_Employee] UNIQUE ([PhaseTaskId], [EmployeeId])
);
GO

CREATE TABLE [dbo].[ProjectPhaseTaskChecklistItem]
(
    [PhaseTaskChecklistItemId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PhaseTaskId] INT NOT NULL,
    [Title] NVARCHAR(300) NOT NULL,
    [IsCompleted] BIT NOT NULL CONSTRAINT [DF_ProjectPhaseTaskChecklistItem_IsCompleted] DEFAULT (0),
    [SortOrder] INT NOT NULL CONSTRAINT [DF_ProjectPhaseTaskChecklistItem_SortOrder] DEFAULT (1),
    CONSTRAINT [FK_ProjectPhaseTaskChecklistItem_Task] FOREIGN KEY ([PhaseTaskId]) REFERENCES [dbo].[ProjectPhaseTask]([PhaseTaskId]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[ProjectPhaseTaskLinkItem]
(
    [PhaseTaskLinkItemId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PhaseTaskId] INT NOT NULL,
    [FileName] NVARCHAR(300) NOT NULL,
    [FileUrl] NVARCHAR(1000) NOT NULL,
    [SortOrder] INT NOT NULL CONSTRAINT [DF_ProjectPhaseTaskLinkItem_SortOrder] DEFAULT (1),
    CONSTRAINT [FK_ProjectPhaseTaskLinkItem_Task] FOREIGN KEY ([PhaseTaskId]) REFERENCES [dbo].[ProjectPhaseTask]([PhaseTaskId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Projects_DepartmentCode] ON [dbo].[Projects]([DepartmentCode]);
CREATE INDEX [IX_ProjectPhase_ProjectId] ON [dbo].[ProjectPhase]([ProjectId]);
CREATE INDEX [IX_ProjectPhaseActivity_ProjectId] ON [dbo].[ProjectPhaseActivity]([ProjectId]);
CREATE INDEX [IX_ProjectPhaseTask_ProjectId] ON [dbo].[ProjectPhaseTask]([ProjectId]);
CREATE INDEX [IX_ProjectPhaseTaskAssignee_EmployeeId] ON [dbo].[ProjectPhaseTaskAssignee]([EmployeeId]);
GO

MERGE [dbo].[AppRole] AS target
USING (VALUES
    (N'Administrator', N'مدير النظام', N'Administrator'),
    (N'ProjectManager', N'مدير المشروع', N'Project Manager'),
    (N'TeamMember', N'عضو فريق', N'Team Member')

) AS source ([RoleCode], [RoleNameAr], [RoleNameEn])
ON target.[RoleCode] = source.[RoleCode]
WHEN NOT MATCHED THEN
    INSERT ([RoleCode], [RoleNameAr], [RoleNameEn])
    VALUES (source.[RoleCode], source.[RoleNameAr], source.[RoleNameEn]);
GO
