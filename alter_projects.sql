ALTER TABLE dbo.Projects ADD
    ProjectDescription NVARCHAR(MAX) NULL,
    ProjectManagerId INT NULL,
    SponsorUserId INT NULL,
    Priority TINYINT NULL,
    PlannedBudget DECIMAL(18,2) NULL,
    ActualCost DECIMAL(18,2) NULL,
    HealthStatus TINYINT NULL,
    Archived BIT NOT NULL DEFAULT(0);
GO

-- Update with some dummy data
UPDATE dbo.Projects
SET 
    ProjectDescription = N'هذا المشروع يهدف إلى تطوير البنية التحتية وتحسين الكفاءة التشغيلية.',
    Priority = CASE WHEN ABS(CHECKSUM(ProjectId)) % 3 = 0 THEN 4 WHEN ABS(CHECKSUM(ProjectId)) % 3 = 1 THEN 3 ELSE 2 END,
    HealthStatus = CASE WHEN ABS(CHECKSUM(ProjectId)) % 3 = 0 THEN 3 WHEN ABS(CHECKSUM(ProjectId)) % 3 = 1 THEN 2 ELSE 1 END,
    PlannedBudget = 100000.00 + (ABS(CHECKSUM(ProjectId)) % 10) * 15000.00,
    ActualCost = 80000.00 + (ABS(CHECKSUM(ProjectId)) % 10) * 12000.00,
    ProjectManagerId = ResponsibleUserId,
    SponsorUserId = 1001 -- Dummy sponsor ID
WHERE Priority IS NULL;
GO
