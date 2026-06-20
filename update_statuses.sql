IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProjectStatuses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProjectStatuses] (
        [StatusId] INT NOT NULL PRIMARY KEY,
        [StatusNameAr] NVARCHAR(100) NOT NULL,
        [StatusNameEn] NVARCHAR(100) NOT NULL,
        [ColorClass] VARCHAR(50) NULL
    );

    INSERT INTO [dbo].[ProjectStatuses] ([StatusId], [StatusNameAr], [StatusNameEn], [ColorClass])
    VALUES 
    (1, N'قيد التطوير', N'Under Development', 'primary'),
    (2, N'منتهي', N'Completed', 'success'),
    (3, N'ملغي', N'Canceled', 'danger'),
    (4, N'معلق', N'On Hold', 'secondary');
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Projects]') AND name = 'StatusId')
BEGIN
    ALTER TABLE [dbo].[Projects] ADD [StatusId] INT NULL;
    ALTER TABLE [dbo].[Projects] ADD CONSTRAINT [FK_Projects_ProjectStatuses] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[ProjectStatuses]([StatusId]);
END
GO

UPDATE [dbo].[Projects] SET [StatusId] = 1 WHERE [StatusId] IS NULL;
GO
