USE [ProjectManagementApp]
GO
/****** Object:  Table [dbo].[AppRole]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppRole](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleCode] [nvarchar](100) NOT NULL,
	[RoleNameAr] [nvarchar](200) NOT NULL,
	[RoleNameEn] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeAccount]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeAccount](
	[EmployeeId] [int] NOT NULL,
	[EmployeeCode] [nvarchar](50) NOT NULL,
	[EmployeeNameAr] [nvarchar](250) NOT NULL,
	[EmployeeNameEn] [nvarchar](250) NULL,
	[Email] [nvarchar](250) NULL,
	[DepartmentCode] [int] NULL,
	[DepartmentName] [nvarchar](250) NULL,
	[PositionName] [nvarchar](250) NULL,
	[PasswordHash] [nvarchar](500) NULL,
	[Photo] [nvarchar](250) NULL,
	[IsActive] [bit] NOT NULL,
	[LastLoginOn] [datetime2](7) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__Employee__7AD04F118C70B349] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeAccountRole]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeAccountRole](
	[EmployeeId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_EmployeeAccountRole] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectPhase]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPhase](
	[PhaseId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[PhaseName] [nvarchar](250) NOT NULL,
	[PhaseCode] [nvarchar](50) NULL,
	[DisplayOrder] [int] NOT NULL,
	[PhaseStatus] [nvarchar](50) NOT NULL,
	[PlannedStartDate] [date] NULL,
	[PlannedEndDate] [date] NULL,
	[ActualStartDate] [date] NULL,
	[ActualEndDate] [date] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PhaseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectPhaseActivity]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPhaseActivity](
	[PhaseActivityId] [int] IDENTITY(1,1) NOT NULL,
	[PhaseId] [int] NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[ActivityName] [nvarchar](300) NOT NULL,
	[IndicatorName] [nvarchar](300) NULL,
	[EstimatedCost] [decimal](18, 2) NULL,
	[TargetValue] [nvarchar](200) NULL,
	[VerificationSource] [nvarchar](300) NULL,
	[RisksAndAssumptions] [nvarchar](max) NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PhaseActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectPhaseTask]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPhaseTask](
	[PhaseTaskId] [int] IDENTITY(1,1) NOT NULL,
	[PhaseActivityId] [int] NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[TaskName] [nvarchar](300) NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Priority] [nvarchar](50) NOT NULL,
	[PercentComplete] [int] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[DependentOnPhaseTaskId] [int] NULL,
	[DependencyType] [nvarchar](50) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PhaseTaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectPhaseTaskAssignee]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPhaseTaskAssignee](
	[PhaseTaskAssigneeId] [int] IDENTITY(1,1) NOT NULL,
	[PhaseTaskId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PhaseTaskAssigneeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectPhaseTaskChecklistItem]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPhaseTaskChecklistItem](
	[PhaseTaskChecklistItemId] [int] IDENTITY(1,1) NOT NULL,
	[PhaseTaskId] [int] NOT NULL,
	[Title] [nvarchar](300) NOT NULL,
	[IsCompleted] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PhaseTaskChecklistItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectPhaseTaskLinkItem]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPhaseTaskLinkItem](
	[PhaseTaskLinkItemId] [int] IDENTITY(1,1) NOT NULL,
	[PhaseTaskId] [int] NOT NULL,
	[FileName] [nvarchar](300) NOT NULL,
	[FileUrl] [nvarchar](1000) NOT NULL,
	[SortOrder] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PhaseTaskLinkItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Projects]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Projects](
	[ProjectId] [uniqueidentifier] NOT NULL,
	[ProjectName] [nvarchar](300) NOT NULL,
	[ProjectCode] [nvarchar](100) NULL,
	[ProjectType] [nvarchar](100) NULL,
	[DepartmentCode] [int] NULL,
	[DepartmentName] [nvarchar](250) NULL,
	[ResponsibleUserId] [int] NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[ProjectStatus] [nvarchar](50) NOT NULL,
	[Approved] [bit] NOT NULL,
	[CurrentProgressPercent] [decimal](9, 2) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[CreatedBy] [int] NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [int] NULL,
	[ProjectDescription] [nvarchar](max) NULL,
	[ProjectManagerId] [int] NULL,
	[SponsorUserId] [int] NULL,
	[Priority] [tinyint] NULL,
	[PlannedBudget] [decimal](18, 2) NULL,
	[ActualCost] [decimal](18, 2) NULL,
	[HealthStatus] [tinyint] NULL,
	[Archived] [bit] NOT NULL,
	[StatusId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectStatuses]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectStatuses](
	[StatusId] [int] NOT NULL,
	[StatusNameAr] [nvarchar](100) NOT NULL,
	[StatusNameEn] [nvarchar](100) NOT NULL,
	[ColorClass] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectTeamMembers]    Script Date: 6/21/2026 1:14:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectTeamMembers](
	[ProjectTeamMemberId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[MemberRole] [nvarchar](100) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProjectTeamMemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AppRole] ON 
GO
INSERT [dbo].[AppRole] ([RoleId], [RoleCode], [RoleNameAr], [RoleNameEn]) VALUES (1, N'Administrator', N'مدير النظام', N'Administrator')
GO
INSERT [dbo].[AppRole] ([RoleId], [RoleCode], [RoleNameAr], [RoleNameEn]) VALUES (2, N'ProjectManager', N'مدير المشروع', N'Project Manager')
GO
INSERT [dbo].[AppRole] ([RoleId], [RoleCode], [RoleNameAr], [RoleNameEn]) VALUES (3, N'TeamMember', N'عضو فريق', N'Team Member')
GO
SET IDENTITY_INSERT [dbo].[AppRole] OFF
GO
INSERT [dbo].[EmployeeAccount] ([EmployeeId], [EmployeeCode], [EmployeeNameAr], [EmployeeNameEn], [Email], [DepartmentCode], [DepartmentName], [PositionName], [PasswordHash], [Photo], [IsActive], [LastLoginOn], [CreatedOn], [ModifiedOn]) VALUES (1001, N'EMP-1001', N'أحمد محمد', N'Ahmed Mohamed', N'ahmed@pmp.local', 10, N'تقنية المعلومات', N'مدير مشروع', NULL, NULL, 1, NULL, CAST(N'2026-06-20T18:04:06.3426352' AS DateTime2), CAST(N'2026-06-20T18:04:06.3426352' AS DateTime2))
GO
INSERT [dbo].[EmployeeAccount] ([EmployeeId], [EmployeeCode], [EmployeeNameAr], [EmployeeNameEn], [Email], [DepartmentCode], [DepartmentName], [PositionName], [PasswordHash], [Photo], [IsActive], [LastLoginOn], [CreatedOn], [ModifiedOn]) VALUES (1002, N'EMP-1002', N'سارة خالد', N'Sarah Khaled', N'sarah@pmp.local', 10, N'تقنية المعلومات', N'مطور برمجيات', NULL, NULL, 1, NULL, CAST(N'2026-06-20T18:04:06.3426352' AS DateTime2), CAST(N'2026-06-20T18:04:06.3426352' AS DateTime2))
GO
INSERT [dbo].[EmployeeAccount] ([EmployeeId], [EmployeeCode], [EmployeeNameAr], [EmployeeNameEn], [Email], [DepartmentCode], [DepartmentName], [PositionName], [PasswordHash], [Photo], [IsActive], [LastLoginOn], [CreatedOn], [ModifiedOn]) VALUES (1003, N'EMP-1003', N'عمر عبدالله', N'Omar Abdullah', N'omar@pmp.local', 20, N'التسويق', N'أخصائي تسويق', NULL, NULL, 1, NULL, CAST(N'2026-06-20T18:04:06.3426352' AS DateTime2), CAST(N'2026-06-20T18:04:06.3426352' AS DateTime2))
GO
INSERT [dbo].[EmployeeAccount] ([EmployeeId], [EmployeeCode], [EmployeeNameAr], [EmployeeNameEn], [Email], [DepartmentCode], [DepartmentName], [PositionName], [PasswordHash], [Photo], [IsActive], [LastLoginOn], [CreatedOn], [ModifiedOn]) VALUES (1004, N'EMP-1004', N'فاطمة علي', N'Fatima Ali', N'fatima@pmp.local', 30, N'الموارد البشرية', N'مدير الموارد', NULL, NULL, 1, NULL, CAST(N'2026-06-20T18:04:06.3426352' AS DateTime2), CAST(N'2026-06-20T18:04:06.3426352' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[ProjectPhase] ON 
GO
INSERT [dbo].[ProjectPhase] ([PhaseId], [ProjectId], [PhaseName], [PhaseCode], [DisplayOrder], [PhaseStatus], [PlannedStartDate], [PlannedEndDate], [ActualStartDate], [ActualEndDate], [Notes], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (3, N'11111111-1111-1111-1111-111111111111', N'مرحلة التخطيط', N'PH-01', 1, N'Completed', CAST(N'2025-01-01' AS Date), CAST(N'2025-02-28' AS Date), NULL, NULL, NULL, CAST(N'2026-06-20T18:04:06.3518120' AS DateTime2), NULL, CAST(N'2026-06-20T18:04:06.3518120' AS DateTime2), NULL)
GO
INSERT [dbo].[ProjectPhase] ([PhaseId], [ProjectId], [PhaseName], [PhaseCode], [DisplayOrder], [PhaseStatus], [PlannedStartDate], [PlannedEndDate], [ActualStartDate], [ActualEndDate], [Notes], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (4, N'11111111-1111-1111-1111-111111111111', N'مرحلة التنفيذ', N'PH-02', 2, N'InProgress', CAST(N'2025-03-01' AS Date), CAST(N'2025-09-30' AS Date), NULL, NULL, NULL, CAST(N'2026-06-20T18:04:06.3518120' AS DateTime2), NULL, CAST(N'2026-06-20T18:04:06.3518120' AS DateTime2), NULL)
GO
INSERT [dbo].[ProjectPhase] ([PhaseId], [ProjectId], [PhaseName], [PhaseCode], [DisplayOrder], [PhaseStatus], [PlannedStartDate], [PlannedEndDate], [ActualStartDate], [ActualEndDate], [Notes], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (5, N'33333333-3333-3333-3333-333333333333', N'rwerwe', N'ee', 1, N'InProgress', CAST(N'2026-06-20' AS Date), CAST(N'2026-06-24' AS Date), CAST(N'2026-06-18' AS Date), CAST(N'2026-06-25' AS Date), N'werewrew', CAST(N'2026-06-20T20:19:07.7469095' AS DateTime2), NULL, CAST(N'2026-06-20T20:19:07.7469095' AS DateTime2), NULL)
GO
SET IDENTITY_INSERT [dbo].[ProjectPhase] OFF
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseActivity] ON 
GO
INSERT [dbo].[ProjectPhaseActivity] ([PhaseActivityId], [PhaseId], [ProjectId], [ActivityName], [IndicatorName], [EstimatedCost], [TargetValue], [VerificationSource], [RisksAndAssumptions], [StartDate], [EndDate], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (3, 3, N'11111111-1111-1111-1111-111111111111', N'تحليل المتطلبات', NULL, NULL, NULL, NULL, NULL, CAST(N'2025-01-01' AS Date), CAST(N'2025-01-31' AS Date), CAST(N'2026-06-20T18:04:06.3663729' AS DateTime2), NULL, CAST(N'2026-06-20T18:04:06.3663729' AS DateTime2), NULL)
GO
INSERT [dbo].[ProjectPhaseActivity] ([PhaseActivityId], [PhaseId], [ProjectId], [ActivityName], [IndicatorName], [EstimatedCost], [TargetValue], [VerificationSource], [RisksAndAssumptions], [StartDate], [EndDate], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (4, 4, N'11111111-1111-1111-1111-111111111111', N'برمجة الواجهات', NULL, NULL, NULL, NULL, NULL, CAST(N'2025-03-01' AS Date), CAST(N'2025-04-30' AS Date), CAST(N'2026-06-20T18:04:06.3663729' AS DateTime2), NULL, CAST(N'2026-06-20T18:04:06.3663729' AS DateTime2), NULL)
GO
INSERT [dbo].[ProjectPhaseActivity] ([PhaseActivityId], [PhaseId], [ProjectId], [ActivityName], [IndicatorName], [EstimatedCost], [TargetValue], [VerificationSource], [RisksAndAssumptions], [StartDate], [EndDate], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (5, 5, N'33333333-3333-3333-3333-333333333333', N'1', N'1', CAST(1.00 AS Decimal(18, 2)), NULL, N'1', N'ewrwer', CAST(N'2026-06-20' AS Date), CAST(N'2026-06-30' AS Date), CAST(N'2026-06-20T20:20:12.4699059' AS DateTime2), NULL, CAST(N'2026-06-20T20:20:12.4699059' AS DateTime2), NULL)
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseActivity] OFF
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseTask] ON 
GO
INSERT [dbo].[ProjectPhaseTask] ([PhaseTaskId], [PhaseActivityId], [ProjectId], [TaskName], [StartDate], [EndDate], [Priority], [PercentComplete], [Notes], [DependentOnPhaseTaskId], [DependencyType], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (4, 3, N'11111111-1111-1111-1111-111111111111', N'كتابة وثيقة الشروط والمواصفات', CAST(N'2025-01-05' AS Date), CAST(N'2025-01-20' AS Date), N'High', 100, NULL, NULL, NULL, CAST(N'2026-06-20T18:04:06.3694980' AS DateTime2), NULL, CAST(N'2026-06-20T18:04:06.3694980' AS DateTime2), NULL)
GO
INSERT [dbo].[ProjectPhaseTask] ([PhaseTaskId], [PhaseActivityId], [ProjectId], [TaskName], [StartDate], [EndDate], [Priority], [PercentComplete], [Notes], [DependentOnPhaseTaskId], [DependencyType], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (5, 4, N'11111111-1111-1111-1111-111111111111', N'تصميم واجهة المستخدم الرئيسية', CAST(N'2025-03-05' AS Date), CAST(N'2025-03-20' AS Date), N'High', 50, NULL, NULL, NULL, CAST(N'2026-06-20T18:04:06.3694980' AS DateTime2), NULL, CAST(N'2026-06-20T18:04:06.3694980' AS DateTime2), NULL)
GO
INSERT [dbo].[ProjectPhaseTask] ([PhaseTaskId], [PhaseActivityId], [ProjectId], [TaskName], [StartDate], [EndDate], [Priority], [PercentComplete], [Notes], [DependentOnPhaseTaskId], [DependencyType], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (6, 4, N'11111111-1111-1111-1111-111111111111', N'برمجة لوحة التحكم', CAST(N'2025-03-21' AS Date), CAST(N'2025-04-15' AS Date), N'Medium', 20, NULL, NULL, NULL, CAST(N'2026-06-20T18:04:06.3694980' AS DateTime2), NULL, CAST(N'2026-06-20T18:04:06.3694980' AS DateTime2), NULL)
GO
INSERT [dbo].[ProjectPhaseTask] ([PhaseTaskId], [PhaseActivityId], [ProjectId], [TaskName], [StartDate], [EndDate], [Priority], [PercentComplete], [Notes], [DependentOnPhaseTaskId], [DependencyType], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy]) VALUES (7, 5, N'33333333-3333-3333-3333-333333333333', N'werwrw', CAST(N'2026-06-20' AS Date), NULL, N'Low', 0, N'ewrwe', NULL, NULL, CAST(N'2026-06-20T20:21:25.9359048' AS DateTime2), NULL, CAST(N'2026-06-20T20:21:25.9359048' AS DateTime2), NULL)
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseTask] OFF
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseTaskAssignee] ON 
GO
INSERT [dbo].[ProjectPhaseTaskAssignee] ([PhaseTaskAssigneeId], [PhaseTaskId], [EmployeeId], [CreatedOn]) VALUES (4, 4, 1001, CAST(N'2026-06-20T18:04:06.3724834' AS DateTime2))
GO
INSERT [dbo].[ProjectPhaseTaskAssignee] ([PhaseTaskAssigneeId], [PhaseTaskId], [EmployeeId], [CreatedOn]) VALUES (5, 5, 1002, CAST(N'2026-06-20T18:04:06.3724834' AS DateTime2))
GO
INSERT [dbo].[ProjectPhaseTaskAssignee] ([PhaseTaskAssigneeId], [PhaseTaskId], [EmployeeId], [CreatedOn]) VALUES (6, 6, 1002, CAST(N'2026-06-20T18:04:06.3724834' AS DateTime2))
GO
INSERT [dbo].[ProjectPhaseTaskAssignee] ([PhaseTaskAssigneeId], [PhaseTaskId], [EmployeeId], [CreatedOn]) VALUES (7, 7, 1001, CAST(N'2026-06-20T20:21:27.4480800' AS DateTime2))
GO
INSERT [dbo].[ProjectPhaseTaskAssignee] ([PhaseTaskAssigneeId], [PhaseTaskId], [EmployeeId], [CreatedOn]) VALUES (8, 7, 1002, CAST(N'2026-06-20T20:21:27.4530796' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseTaskAssignee] OFF
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseTaskChecklistItem] ON 
GO
INSERT [dbo].[ProjectPhaseTaskChecklistItem] ([PhaseTaskChecklistItemId], [PhaseTaskId], [Title], [IsCompleted], [SortOrder]) VALUES (1, 7, N'fdsf', 0, 1)
GO
INSERT [dbo].[ProjectPhaseTaskChecklistItem] ([PhaseTaskChecklistItemId], [PhaseTaskId], [Title], [IsCompleted], [SortOrder]) VALUES (2, 7, N'ewrwer', 1, 2)
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseTaskChecklistItem] OFF
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseTaskLinkItem] ON 
GO
INSERT [dbo].[ProjectPhaseTaskLinkItem] ([PhaseTaskLinkItemId], [PhaseTaskId], [FileName], [FileUrl], [SortOrder]) VALUES (1, 7, N'rwerwe', N'erw', 1)
GO
SET IDENTITY_INSERT [dbo].[ProjectPhaseTaskLinkItem] OFF
GO
INSERT [dbo].[Projects] ([ProjectId], [ProjectName], [ProjectCode], [ProjectType], [DepartmentCode], [DepartmentName], [ResponsibleUserId], [StartDate], [EndDate], [ProjectStatus], [Approved], [CurrentProgressPercent], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [ProjectDescription], [ProjectManagerId], [SponsorUserId], [Priority], [PlannedBudget], [ActualCost], [HealthStatus], [Archived], [StatusId]) VALUES (N'11111111-1111-1111-1111-111111111111', N'مشروع التحول الرقمي', NULL, N'استراتيجي', 10, N'تقنية المعلومات', 1001, CAST(N'2025-01-01' AS Date), CAST(N'2025-12-31' AS Date), N'Active', 1, CAST(45.00 AS Decimal(9, 2)), CAST(N'2026-06-20T18:04:06.3458576' AS DateTime2), NULL, CAST(N'2026-06-21T00:12:48.8110779' AS DateTime2), NULL, N'Ù‡Ø°Ø§ Ø§Ù„Ù…Ø´Ø±ÙˆØ¹ ÙŠÙ‡Ø¯Ù Ø¥Ù„Ù‰ ØªØ·ÙˆÙŠØ± Ø§Ù„Ø¨Ù†ÙŠØ© Ø§Ù„ØªØ­ØªÙŠØ© ÙˆØªØ­Ø³ÙŠÙ† Ø§Ù„ÙƒÙØ§Ø¡Ø© Ø§Ù„ØªØ´ØºÙŠÙ„ÙŠØ©.', 1001, 1001, 1, CAST(100000.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), 1, 0, NULL)
GO
INSERT [dbo].[Projects] ([ProjectId], [ProjectName], [ProjectCode], [ProjectType], [DepartmentCode], [DepartmentName], [ResponsibleUserId], [StartDate], [EndDate], [ProjectStatus], [Approved], [CurrentProgressPercent], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [ProjectDescription], [ProjectManagerId], [SponsorUserId], [Priority], [PlannedBudget], [ActualCost], [HealthStatus], [Archived], [StatusId]) VALUES (N'22222222-2222-2222-2222-222222222222', N'حملة التسويق السنوية', NULL, N'تشغيلي', 20, N'التسويق', 1003, CAST(N'2025-03-01' AS Date), CAST(N'2025-06-30' AS Date), N'Active', 1, CAST(15.00 AS Decimal(9, 2)), CAST(N'2026-06-20T18:04:06.3458576' AS DateTime2), NULL, CAST(N'2026-06-21T00:12:33.9451265' AS DateTime2), NULL, N'هنا يكون وصف المشروع ', 1003, 1001, 3, CAST(100000.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), 3, 0, NULL)
GO
INSERT [dbo].[Projects] ([ProjectId], [ProjectName], [ProjectCode], [ProjectType], [DepartmentCode], [DepartmentName], [ResponsibleUserId], [StartDate], [EndDate], [ProjectStatus], [Approved], [CurrentProgressPercent], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [ProjectDescription], [ProjectManagerId], [SponsorUserId], [Priority], [PlannedBudget], [ActualCost], [HealthStatus], [Archived], [StatusId]) VALUES (N'33333333-3333-3333-3333-333333333333', N'تطوير البنية التحتية', NULL, N'تحولي', 10, N'تقنية المعلومات', 1001, CAST(N'2025-05-01' AS Date), CAST(N'2026-05-01' AS Date), N'Active', 1, CAST(0.00 AS Decimal(9, 2)), CAST(N'2026-06-20T18:04:06.3458576' AS DateTime2), NULL, CAST(N'2026-06-21T01:13:55.6252556' AS DateTime2), NULL, N'هنا يكون وصف المشروع', 1001, 1001, 2, CAST(100000.00 AS Decimal(18, 2)), CAST(80000.00 AS Decimal(18, 2)), 3, 0, NULL)
GO
INSERT [dbo].[ProjectStatuses] ([StatusId], [StatusNameAr], [StatusNameEn], [ColorClass]) VALUES (1, N'قيد التطوير', N'Under Development', N'primary')
GO
INSERT [dbo].[ProjectStatuses] ([StatusId], [StatusNameAr], [StatusNameEn], [ColorClass]) VALUES (2, N'منتهي', N'Completed', N'success')
GO
INSERT [dbo].[ProjectStatuses] ([StatusId], [StatusNameAr], [StatusNameEn], [ColorClass]) VALUES (3, N'ملغي', N'Canceled', N'danger')
GO
INSERT [dbo].[ProjectStatuses] ([StatusId], [StatusNameAr], [StatusNameEn], [ColorClass]) VALUES (4, N'معلق', N'On Hold', N'secondary')
GO
SET IDENTITY_INSERT [dbo].[ProjectTeamMembers] ON 
GO
INSERT [dbo].[ProjectTeamMembers] ([ProjectTeamMemberId], [ProjectId], [EmployeeId], [MemberRole], [CreatedOn]) VALUES (14, N'22222222-2222-2222-2222-222222222222', 1003, NULL, CAST(N'2026-06-21T00:12:33.9481066' AS DateTime2))
GO
INSERT [dbo].[ProjectTeamMembers] ([ProjectTeamMemberId], [ProjectId], [EmployeeId], [MemberRole], [CreatedOn]) VALUES (15, N'11111111-1111-1111-1111-111111111111', 1001, NULL, CAST(N'2026-06-21T00:12:48.8120780' AS DateTime2))
GO
INSERT [dbo].[ProjectTeamMembers] ([ProjectTeamMemberId], [ProjectId], [EmployeeId], [MemberRole], [CreatedOn]) VALUES (16, N'11111111-1111-1111-1111-111111111111', 1002, NULL, CAST(N'2026-06-21T00:12:48.8120780' AS DateTime2))
GO
INSERT [dbo].[ProjectTeamMembers] ([ProjectTeamMemberId], [ProjectId], [EmployeeId], [MemberRole], [CreatedOn]) VALUES (21, N'33333333-3333-3333-3333-333333333333', 1001, NULL, CAST(N'2026-06-21T01:13:55.6407771' AS DateTime2))
GO
INSERT [dbo].[ProjectTeamMembers] ([ProjectTeamMemberId], [ProjectId], [EmployeeId], [MemberRole], [CreatedOn]) VALUES (22, N'33333333-3333-3333-3333-333333333333', 1002, NULL, CAST(N'2026-06-21T01:13:55.6428335' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[ProjectTeamMembers] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__AppRole__D62CB59C96ACF8DD]    Script Date: 6/21/2026 1:14:55 AM ******/
ALTER TABLE [dbo].[AppRole] ADD UNIQUE NONCLUSTERED 
(
	[RoleCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Employee__1F6425484DEEA6FA]    Script Date: 6/21/2026 1:14:55 AM ******/
ALTER TABLE [dbo].[EmployeeAccount] ADD  CONSTRAINT [UQ__Employee__1F6425484DEEA6FA] UNIQUE NONCLUSTERED 
(
	[EmployeeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UX_ProjectPhaseTaskAssignee_Task_Employee]    Script Date: 6/21/2026 1:14:55 AM ******/
ALTER TABLE [dbo].[ProjectPhaseTaskAssignee] ADD  CONSTRAINT [UX_ProjectPhaseTaskAssignee_Task_Employee] UNIQUE NONCLUSTERED 
(
	[PhaseTaskId] ASC,
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UX_ProjectTeamMembers_Project_Employee]    Script Date: 6/21/2026 1:14:55 AM ******/
ALTER TABLE [dbo].[ProjectTeamMembers] ADD  CONSTRAINT [UX_ProjectTeamMembers_Project_Employee] UNIQUE NONCLUSTERED 
(
	[ProjectId] ASC,
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EmployeeAccount] ADD  CONSTRAINT [DF_EmployeeAccount_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[EmployeeAccount] ADD  CONSTRAINT [DF_EmployeeAccount_CreatedOn]  DEFAULT (sysdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[EmployeeAccount] ADD  CONSTRAINT [DF_EmployeeAccount_ModifiedOn]  DEFAULT (sysdatetime()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[ProjectPhase] ADD  CONSTRAINT [DF_ProjectPhase_DisplayOrder]  DEFAULT ((1)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[ProjectPhase] ADD  CONSTRAINT [DF_ProjectPhase_PhaseStatus]  DEFAULT (N'NotStarted') FOR [PhaseStatus]
GO
ALTER TABLE [dbo].[ProjectPhase] ADD  CONSTRAINT [DF_ProjectPhase_CreatedOn]  DEFAULT (sysdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[ProjectPhase] ADD  CONSTRAINT [DF_ProjectPhase_ModifiedOn]  DEFAULT (sysdatetime()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[ProjectPhaseActivity] ADD  CONSTRAINT [DF_ProjectPhaseActivity_CreatedOn]  DEFAULT (sysdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[ProjectPhaseActivity] ADD  CONSTRAINT [DF_ProjectPhaseActivity_ModifiedOn]  DEFAULT (sysdatetime()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[ProjectPhaseTask] ADD  CONSTRAINT [DF_ProjectPhaseTask_Priority]  DEFAULT (N'Medium') FOR [Priority]
GO
ALTER TABLE [dbo].[ProjectPhaseTask] ADD  CONSTRAINT [DF_ProjectPhaseTask_PercentComplete]  DEFAULT ((0)) FOR [PercentComplete]
GO
ALTER TABLE [dbo].[ProjectPhaseTask] ADD  CONSTRAINT [DF_ProjectPhaseTask_CreatedOn]  DEFAULT (sysdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[ProjectPhaseTask] ADD  CONSTRAINT [DF_ProjectPhaseTask_ModifiedOn]  DEFAULT (sysdatetime()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[ProjectPhaseTaskAssignee] ADD  CONSTRAINT [DF_ProjectPhaseTaskAssignee_CreatedOn]  DEFAULT (sysdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[ProjectPhaseTaskChecklistItem] ADD  CONSTRAINT [DF_ProjectPhaseTaskChecklistItem_IsCompleted]  DEFAULT ((0)) FOR [IsCompleted]
GO
ALTER TABLE [dbo].[ProjectPhaseTaskChecklistItem] ADD  CONSTRAINT [DF_ProjectPhaseTaskChecklistItem_SortOrder]  DEFAULT ((1)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[ProjectPhaseTaskLinkItem] ADD  CONSTRAINT [DF_ProjectPhaseTaskLinkItem_SortOrder]  DEFAULT ((1)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_ProjectId]  DEFAULT (newid()) FOR [ProjectId]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_ProjectStatus]  DEFAULT (N'Active') FOR [ProjectStatus]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_Approved]  DEFAULT ((1)) FOR [Approved]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_CurrentProgressPercent]  DEFAULT ((0)) FOR [CurrentProgressPercent]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_CreatedOn]  DEFAULT (sysdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_ModifiedOn]  DEFAULT (sysdatetime()) FOR [ModifiedOn]
GO
ALTER TABLE [dbo].[Projects] ADD  DEFAULT ((0)) FOR [Archived]
GO
ALTER TABLE [dbo].[ProjectTeamMembers] ADD  CONSTRAINT [DF_ProjectTeamMembers_CreatedOn]  DEFAULT (sysdatetime()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[EmployeeAccountRole]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAccountRole_AppRole] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AppRole] ([RoleId])
GO
ALTER TABLE [dbo].[EmployeeAccountRole] CHECK CONSTRAINT [FK_EmployeeAccountRole_AppRole]
GO
ALTER TABLE [dbo].[EmployeeAccountRole]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAccountRole_EmployeeAccount] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[EmployeeAccount] ([EmployeeId])
GO
ALTER TABLE [dbo].[EmployeeAccountRole] CHECK CONSTRAINT [FK_EmployeeAccountRole_EmployeeAccount]
GO
ALTER TABLE [dbo].[ProjectPhase]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhase_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([ProjectId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectPhase] CHECK CONSTRAINT [FK_ProjectPhase_Projects]
GO
ALTER TABLE [dbo].[ProjectPhaseActivity]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhaseActivity_ProjectPhase] FOREIGN KEY([PhaseId])
REFERENCES [dbo].[ProjectPhase] ([PhaseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectPhaseActivity] CHECK CONSTRAINT [FK_ProjectPhaseActivity_ProjectPhase]
GO
ALTER TABLE [dbo].[ProjectPhaseActivity]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhaseActivity_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([ProjectId])
GO
ALTER TABLE [dbo].[ProjectPhaseActivity] CHECK CONSTRAINT [FK_ProjectPhaseActivity_Projects]
GO
ALTER TABLE [dbo].[ProjectPhaseTask]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhaseTask_Dependency] FOREIGN KEY([DependentOnPhaseTaskId])
REFERENCES [dbo].[ProjectPhaseTask] ([PhaseTaskId])
GO
ALTER TABLE [dbo].[ProjectPhaseTask] CHECK CONSTRAINT [FK_ProjectPhaseTask_Dependency]
GO
ALTER TABLE [dbo].[ProjectPhaseTask]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhaseTask_ProjectPhaseActivity] FOREIGN KEY([PhaseActivityId])
REFERENCES [dbo].[ProjectPhaseActivity] ([PhaseActivityId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectPhaseTask] CHECK CONSTRAINT [FK_ProjectPhaseTask_ProjectPhaseActivity]
GO
ALTER TABLE [dbo].[ProjectPhaseTask]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhaseTask_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([ProjectId])
GO
ALTER TABLE [dbo].[ProjectPhaseTask] CHECK CONSTRAINT [FK_ProjectPhaseTask_Projects]
GO
ALTER TABLE [dbo].[ProjectPhaseTaskAssignee]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhaseTaskAssignee_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[EmployeeAccount] ([EmployeeId])
GO
ALTER TABLE [dbo].[ProjectPhaseTaskAssignee] CHECK CONSTRAINT [FK_ProjectPhaseTaskAssignee_Employee]
GO
ALTER TABLE [dbo].[ProjectPhaseTaskAssignee]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhaseTaskAssignee_Task] FOREIGN KEY([PhaseTaskId])
REFERENCES [dbo].[ProjectPhaseTask] ([PhaseTaskId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectPhaseTaskAssignee] CHECK CONSTRAINT [FK_ProjectPhaseTaskAssignee_Task]
GO
ALTER TABLE [dbo].[ProjectPhaseTaskChecklistItem]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhaseTaskChecklistItem_Task] FOREIGN KEY([PhaseTaskId])
REFERENCES [dbo].[ProjectPhaseTask] ([PhaseTaskId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectPhaseTaskChecklistItem] CHECK CONSTRAINT [FK_ProjectPhaseTaskChecklistItem_Task]
GO
ALTER TABLE [dbo].[ProjectPhaseTaskLinkItem]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPhaseTaskLinkItem_Task] FOREIGN KEY([PhaseTaskId])
REFERENCES [dbo].[ProjectPhaseTask] ([PhaseTaskId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectPhaseTaskLinkItem] CHECK CONSTRAINT [FK_ProjectPhaseTaskLinkItem_Task]
GO
ALTER TABLE [dbo].[Projects]  WITH CHECK ADD  CONSTRAINT [FK_Projects_ProjectStatuses] FOREIGN KEY([StatusId])
REFERENCES [dbo].[ProjectStatuses] ([StatusId])
GO
ALTER TABLE [dbo].[Projects] CHECK CONSTRAINT [FK_Projects_ProjectStatuses]
GO
ALTER TABLE [dbo].[Projects]  WITH CHECK ADD  CONSTRAINT [FK_Projects_ResponsibleUser] FOREIGN KEY([ResponsibleUserId])
REFERENCES [dbo].[EmployeeAccount] ([EmployeeId])
GO
ALTER TABLE [dbo].[Projects] CHECK CONSTRAINT [FK_Projects_ResponsibleUser]
GO
ALTER TABLE [dbo].[ProjectTeamMembers]  WITH CHECK ADD  CONSTRAINT [FK_ProjectTeamMembers_EmployeeAccount] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[EmployeeAccount] ([EmployeeId])
GO
ALTER TABLE [dbo].[ProjectTeamMembers] CHECK CONSTRAINT [FK_ProjectTeamMembers_EmployeeAccount]
GO
ALTER TABLE [dbo].[ProjectTeamMembers]  WITH CHECK ADD  CONSTRAINT [FK_ProjectTeamMembers_Projects] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Projects] ([ProjectId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectTeamMembers] CHECK CONSTRAINT [FK_ProjectTeamMembers_Projects]
GO
ALTER TABLE [dbo].[ProjectPhaseTask]  WITH CHECK ADD  CONSTRAINT [CK_ProjectPhaseTask_PercentComplete] CHECK  (([PercentComplete]>=(0) AND [PercentComplete]<=(100)))
GO
ALTER TABLE [dbo].[ProjectPhaseTask] CHECK CONSTRAINT [CK_ProjectPhaseTask_PercentComplete]
GO
