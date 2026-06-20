using Microsoft.Data.SqlClient;
using ProjectManagementApp.Models;

namespace ProjectManagementApp.Services;

public interface IMyProjectsService
{
    Task<List<MyProjectsApprovedItem>> GetApprovedProjectsAsync(int employeeId, int? departmentCode, bool isAdministrator);
    Task<MyProjectsApprovedItem?> GetApprovedProjectByIdAsync(Guid projectId, int employeeId, bool isAdministrator);
    Task<List<EmployeeSearchItem>> SearchEmployeesAsync(string query);
    Task<MyProjectsTasksDashboardViewModel> GetTasksDashboardAsync(int employeeId, int? departmentCode, bool isAdministrator);
    Task<List<ProjectPhaseItem>> GetProjectPhasesAsync(Guid projectId);
    Task<List<ProjectPhaseActivityItem>> GetProjectPhaseActivitiesAsync(Guid projectId);
    Task<List<ProjectPhaseTaskItem>> GetProjectPhaseTasksAsync(Guid projectId);
    Task<List<ProjectTaskAssigneeOption>> GetProjectTaskAssigneesAsync(Guid projectId);
    Task<List<ProjectActivityTemplateItem>> GetProjectActivityTemplatesAsync(Guid projectId);
    Task<int> SaveProjectPhaseAsync(ProjectPhaseSaveModel model, int currentUserId);
    Task<int> SaveProjectPhaseActivityAsync(ProjectPhaseActivitySaveModel model, int currentUserId);
    Task<int> SaveProjectPhaseTaskAsync(ProjectPhaseTaskSaveModel model, int currentUserId);
    Task DeleteProjectPhaseAsync(Guid projectId, int phaseId, int currentUserId);
    Task DeleteProjectPhaseActivityAsync(Guid projectId, int phaseActivityId, int currentUserId);
    Task DeleteProjectPhaseTaskAsync(Guid projectId, int phaseTaskId, int currentUserId);
    Task<Guid> SaveProjectAsync(ProjectSaveModel model, int currentUserId);
    Task DeleteProjectAsync(Guid projectId, int currentUserId);
    Task<List<ProjectStatusItem>> GetProjectStatusesAsync();
}

public class MyProjectsService : IMyProjectsService
{
    private readonly string _connectionString;

    public MyProjectsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ProjectManagementDatabase")
            ?? throw new InvalidOperationException("ProjectManagementDatabase connection string not found");
    }

    public async Task<List<EmployeeSearchItem>> SearchEmployeesAsync(string query)
    {
        var items = new List<EmployeeSearchItem>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(@"
            SELECT TOP 20
                EmployeeId,
                EmployeeNameAr,
                Photo
            FROM dbo.EmployeeAccount
            WHERE IsActive = 1
              AND (@Query IS NULL OR EmployeeNameAr LIKE '%' + @Query + '%' OR EmployeeNameEn LIKE '%' + @Query + '%' OR EmployeeCode LIKE '%' + @Query + '%')
            ORDER BY EmployeeNameAr", connection);

        command.Parameters.AddWithValue("@Query", string.IsNullOrWhiteSpace(query) ? DBNull.Value : query);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new EmployeeSearchItem
            {
                id = reader.GetInt32(0),
                text = reader.GetString(1),
                photo = reader.IsDBNull(2) ? null : reader.GetString(2)
            });
        }
        return items;
    }

    public async Task<List<ProjectStatusItem>> GetProjectStatusesAsync()
    {
        var items = new List<ProjectStatusItem>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(@"
            SELECT StatusId, StatusNameAr, StatusNameEn, ColorClass
            FROM dbo.ProjectStatuses
            ORDER BY StatusId", connection);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new ProjectStatusItem
            {
                StatusId = reader.GetInt32(0),
                StatusNameAr = reader.GetString(1),
                StatusNameEn = reader.GetString(2),
                ColorClass = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }
        return items;
    }

    public async Task<List<MyProjectsApprovedItem>> GetApprovedProjectsAsync(int employeeId, int? departmentCode, bool isAdministrator)
    {
        var items = new List<MyProjectsApprovedItem>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(@"
            SELECT DISTINCT
                p.ProjectId,
                p.ProjectName,
                p.ProjectType,
                p.DepartmentCode,
                p.DepartmentName,
                CAST(NULL AS NVARCHAR(250)) AS SectionNames,
                p.ResponsibleUserId,
                resp.EmployeeNameAr AS ResponsibleUserName,
                p.StartDate,
                p.EndDate,
                p.CurrentProgressPercent,
                p.ProjectDescription,
                p.ProjectManagerId,
                p.SponsorUserId,
                p.Priority,
                p.PlannedBudget,
                p.ActualCost,
                p.HealthStatus,
                p.Archived,
                p.StatusId,
                ps.StatusNameAr,
                ps.StatusNameEn,
                ps.ColorClass
            FROM dbo.Projects p
            LEFT JOIN dbo.EmployeeAccount resp ON resp.EmployeeId = p.ResponsibleUserId
            LEFT JOIN dbo.ProjectTeamMembers tm ON tm.ProjectId = p.ProjectId
            LEFT JOIN dbo.ProjectStatuses ps ON ps.StatusId = p.StatusId
            WHERE p.Approved = 1
              AND (
                    @IsAdministrator = 1
                    OR p.ResponsibleUserId = @EmployeeId
                    OR tm.EmployeeId = @EmployeeId
                    OR (@DepartmentCode IS NOT NULL AND p.DepartmentCode = @DepartmentCode)
                  )
            ORDER BY p.ProjectName", connection);

        command.Parameters.AddWithValue("@IsAdministrator", isAdministrator);
        command.Parameters.AddWithValue("@EmployeeId", employeeId);
        command.Parameters.AddWithValue("@DepartmentCode", (object?)departmentCode ?? DBNull.Value);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new MyProjectsApprovedItem
            {
                ProjectId = reader.GetGuid(0),
                ProjectName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                ProjectTypeText = reader.IsDBNull(2) ? null : reader.GetString(2),
                DepartmentCode = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                DepartmentName = reader.IsDBNull(4) ? null : reader.GetString(4),
                SectionNames = reader.IsDBNull(5) ? null : reader.GetString(5),
                ResponsibleUserId = reader.IsDBNull(6) ? null : reader.GetInt32(6),
                ResponsibleUserName = reader.IsDBNull(7) ? null : reader.GetString(7),
                StartDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                EndDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                CurrentProgressPercent = reader.IsDBNull(10) ? 0 : reader.GetDecimal(10),
                ProjectDescription = reader.IsDBNull(11) ? null : reader.GetString(11),
                SponsorUserId = reader.IsDBNull(13) ? null : reader.GetInt32(13),
                Priority = reader.IsDBNull(14) ? null : reader.GetByte(14),
                PlannedBudget = reader.IsDBNull(15) ? null : reader.GetDecimal(15),
                ActualCost = reader.IsDBNull(16) ? null : reader.GetDecimal(16),
                HealthStatus = reader.IsDBNull(17) ? null : reader.GetByte(17),
                Archived = reader.IsDBNull(18) ? false : reader.GetBoolean(18),
                StatusId = reader.IsDBNull(19) ? null : reader.GetInt32(19),
                StatusNameAr = reader.IsDBNull(20) ? null : reader.GetString(20),
                StatusNameEn = reader.IsDBNull(21) ? null : reader.GetString(21),
                StatusColorClass = reader.IsDBNull(22) ? null : reader.GetString(22)
            });
        }
        await reader.CloseAsync();

        if (items.Any())
        {
            var projectIds = items.Select(x => x.ProjectId).ToList();
            var idsString = string.Join(",", projectIds.Select(id => $"'{id}'"));
            
            using var teamCommand = new SqlCommand($@"
                SELECT tm.ProjectId, tm.EmployeeId, e.EmployeeNameAr, e.Photo, e.DepartmentName, e.EmployeeCode
                FROM dbo.ProjectTeamMembers tm
                JOIN dbo.EmployeeAccount e ON e.EmployeeId = tm.EmployeeId
                WHERE tm.ProjectId IN ({idsString})", connection);
                
            using var teamReader = await teamCommand.ExecuteReaderAsync();
            while (await teamReader.ReadAsync())
            {
                var pId = teamReader.GetGuid(0);
                var project = items.FirstOrDefault(p => p.ProjectId == pId);
                if (project != null)
                {
                    project.TeamMembers.Add(new TeamMemberItem
                    {
                        EmployeeId = teamReader.GetInt32(1),
                        EmployeeName = teamReader.GetString(2),
                        Photo = teamReader.IsDBNull(3) ? null : teamReader.GetString(3),
                        DepartmentName = teamReader.IsDBNull(4) ? null : teamReader.GetString(4),
                        Email = teamReader.IsDBNull(5) ? null : $"{teamReader.GetString(5)}@company.com"
                    });
                }
            }
        }

        return items;
    }

    public async Task<MyProjectsApprovedItem?> GetApprovedProjectByIdAsync(Guid projectId, int employeeId, bool isAdministrator)
    {
        return (await GetApprovedProjectsAsync(employeeId, null, isAdministrator))
            .FirstOrDefault(x => x.ProjectId == projectId);
    }

    public async Task<MyProjectsTasksDashboardViewModel> GetTasksDashboardAsync(int employeeId, int? departmentCode, bool isAdministrator)
    {
        var projects = await GetApprovedProjectsAsync(employeeId, departmentCode, isAdministrator);
        var result = new MyProjectsTasksDashboardViewModel
        {
            DepartmentCode = departmentCode,
            DepartmentName = projects.FirstOrDefault()?.DepartmentName,
            CurrentUserId = employeeId,
            IsAdministrator = isAdministrator,
            Projects = projects
        };

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using (var teamMemberCommand = new SqlCommand(@"
            SELECT DISTINCT
                e.EmployeeId,
                e.EmployeeNameAr,
                e.PositionName,
                e.DepartmentName
            FROM dbo.EmployeeAccount e
            WHERE e.EmployeeId IN (
                SELECT p.ResponsibleUserId
                FROM dbo.Projects p
                WHERE p.ProjectId IN (SELECT ProjectId FROM dbo.Projects WHERE Approved = 1)
                UNION
                SELECT tm.EmployeeId
                FROM dbo.ProjectTeamMembers tm
                WHERE tm.ProjectId IN (SELECT ProjectId FROM dbo.Projects WHERE Approved = 1)
                UNION
                SELECT a.EmployeeId
                FROM dbo.ProjectPhaseTaskAssignee a
                INNER JOIN dbo.ProjectPhaseTask t ON t.PhaseTaskId = a.PhaseTaskId
                WHERE t.ProjectId IN (SELECT ProjectId FROM dbo.Projects WHERE Approved = 1)
            )
            ORDER BY e.EmployeeNameAr", connection))
        {
            using var reader = await teamMemberCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.TeamMembers.Add(new MyProjectsTaskTeamMemberItem
                {
                    EmployeeId = reader.GetInt32(0),
                    EmployeeName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    PositionName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    DepartmentName = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
        }

        using (var assignmentCommand = new SqlCommand(@"
            SELECT
                t.PhaseTaskId,
                p.ProjectId,
                p.ProjectName,
                a.EmployeeId,
                e.EmployeeNameAr,
                t.TaskName,
                ph.PhaseName,
                act.ActivityName,
                t.StartDate,
                t.EndDate,
                t.Priority,
                t.PercentComplete
            FROM dbo.ProjectPhaseTaskAssignee a
            INNER JOIN dbo.ProjectPhaseTask t ON t.PhaseTaskId = a.PhaseTaskId
            INNER JOIN dbo.Projects p ON p.ProjectId = t.ProjectId AND p.Approved = 1
            LEFT JOIN dbo.ProjectPhaseActivity act ON act.PhaseActivityId = t.PhaseActivityId
            LEFT JOIN dbo.ProjectPhase ph ON ph.PhaseId = act.PhaseId
            LEFT JOIN dbo.EmployeeAccount e ON e.EmployeeId = a.EmployeeId
            WHERE @IsAdministrator = 1
               OR p.ResponsibleUserId = @EmployeeId
               OR a.EmployeeId = @EmployeeId
               OR EXISTS (SELECT 1 FROM dbo.ProjectTeamMembers tm WHERE tm.ProjectId = p.ProjectId AND tm.EmployeeId = @EmployeeId)
            ORDER BY p.ProjectName, e.EmployeeNameAr, t.TaskName", connection))
        {
            assignmentCommand.Parameters.AddWithValue("@IsAdministrator", isAdministrator);
            assignmentCommand.Parameters.AddWithValue("@EmployeeId", employeeId);
            using var reader = await assignmentCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Assignments.Add(new MyProjectsTaskAssignmentItem
                {
                    PhaseTaskId = reader.GetInt32(0),
                    ProjectId = reader.GetGuid(1),
                    ProjectName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    EmployeeId = reader.GetInt32(3),
                    EmployeeName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    TaskName = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    PhaseName = reader.IsDBNull(6) ? null : reader.GetString(6),
                    ActivityName = reader.IsDBNull(7) ? null : reader.GetString(7),
                    StartDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    EndDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                    Priority = reader.IsDBNull(10) ? "Medium" : reader.GetString(10),
                    PercentComplete = reader.IsDBNull(11) ? 0 : reader.GetInt32(11)
                });
            }
        }

        result.DetailedTasks = await BuildDashboardTaskDetailsAsync(projects);
        return result;
    }

    public async Task<List<ProjectPhaseItem>> GetProjectPhasesAsync(Guid projectId)
    {
        var items = new List<ProjectPhaseItem>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(@"
            SELECT PhaseId, ProjectId, PhaseName, PhaseCode, DisplayOrder, PhaseStatus,
                   PlannedStartDate, PlannedEndDate, ActualStartDate, ActualEndDate, Notes
            FROM dbo.ProjectPhase
            WHERE ProjectId = @ProjectId
            ORDER BY DisplayOrder, PhaseId", connection);
        command.Parameters.AddWithValue("@ProjectId", projectId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new ProjectPhaseItem
            {
                PhaseId = reader.GetInt32(0),
                ProjectId = reader.GetGuid(1),
                PhaseName = reader.GetString(2),
                PhaseCode = reader.IsDBNull(3) ? null : reader.GetString(3),
                DisplayOrder = reader.GetInt32(4),
                PhaseStatus = reader.IsDBNull(5) ? "NotStarted" : reader.GetString(5),
                PlannedStartDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                PlannedEndDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                ActualStartDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                ActualEndDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                Notes = reader.IsDBNull(10) ? null : reader.GetString(10)
            });
        }
        return items;
    }

    public async Task<List<ProjectPhaseActivityItem>> GetProjectPhaseActivitiesAsync(Guid projectId)
    {
        var items = new List<ProjectPhaseActivityItem>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(@"
            SELECT PhaseActivityId, PhaseId, ProjectId, ActivityName, IndicatorName, EstimatedCost,
                   TargetValue, VerificationSource, RisksAndAssumptions, StartDate, EndDate
            FROM dbo.ProjectPhaseActivity
            WHERE ProjectId = @ProjectId
            ORDER BY PhaseId, PhaseActivityId", connection);
        command.Parameters.AddWithValue("@ProjectId", projectId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new ProjectPhaseActivityItem
            {
                PhaseActivityId = reader.GetInt32(0),
                PhaseId = reader.GetInt32(1),
                ProjectId = reader.GetGuid(2),
                SourceActivityId = null,
                ReferenceNumber = null,
                ActivityName = reader.GetString(3),
                IndicatorName = reader.IsDBNull(4) ? null : reader.GetString(4),
                EstimatedCost = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                TargetValue = reader.IsDBNull(6) ? null : reader.GetString(6),
                VerificationSource = reader.IsDBNull(7) ? null : reader.GetString(7),
                RisksAndAssumptions = reader.IsDBNull(8) ? null : reader.GetString(8),
                StartDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                EndDate = reader.IsDBNull(10) ? null : reader.GetDateTime(10)
            });
        }
        return items;
    }

    public async Task<List<ProjectPhaseTaskItem>> GetProjectPhaseTasksAsync(Guid projectId)
    {
        var items = new List<ProjectPhaseTaskItem>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new SqlCommand(@"
            SELECT t.PhaseTaskId, t.PhaseActivityId, t.ProjectId, t.TaskName, t.StartDate, t.EndDate,
                   t.Priority, t.PercentComplete, t.Notes, t.DependentOnPhaseTaskId, t.DependencyType,
                   dep.TaskName AS DependentOnTaskName
            FROM dbo.ProjectPhaseTask t
            LEFT JOIN dbo.ProjectPhaseTask dep ON dep.PhaseTaskId = t.DependentOnPhaseTaskId
            WHERE t.ProjectId = @ProjectId
            ORDER BY t.PhaseActivityId, t.PhaseTaskId", connection))
        {
            command.Parameters.AddWithValue("@ProjectId", projectId);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new ProjectPhaseTaskItem
                {
                    PhaseTaskId = reader.GetInt32(0),
                    PhaseActivityId = reader.GetInt32(1),
                    ProjectId = reader.GetGuid(2),
                    TaskName = reader.GetString(3),
                    StartDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    EndDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    Priority = reader.IsDBNull(6) ? "Medium" : reader.GetString(6),
                    PercentComplete = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                    Notes = reader.IsDBNull(8) ? null : reader.GetString(8),
                    DependentOnPhaseTaskId = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                    DependencyType = reader.IsDBNull(10) ? null : reader.GetString(10),
                    DependentOnTaskName = reader.IsDBNull(11) ? null : reader.GetString(11)
                });
            }
        }

        using (var assigneeCommand = new SqlCommand(@"
            SELECT a.PhaseTaskId, a.EmployeeId, e.EmployeeNameAr
            FROM dbo.ProjectPhaseTaskAssignee a
            INNER JOIN dbo.ProjectPhaseTask t ON t.PhaseTaskId = a.PhaseTaskId
            LEFT JOIN dbo.EmployeeAccount e ON e.EmployeeId = a.EmployeeId
            WHERE t.ProjectId = @ProjectId
            ORDER BY a.PhaseTaskId, e.EmployeeNameAr", connection))
        {
            assigneeCommand.Parameters.AddWithValue("@ProjectId", projectId);
            using var reader = await assigneeCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var task = items.FirstOrDefault(x => x.PhaseTaskId == reader.GetInt32(0));
                if (task == null) continue;
                task.Assignees.Add(new ProjectTaskAssigneeOption
                {
                    EmployeeId = reader.GetInt32(1),
                    EmployeeName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                });
            }
        }

        using (var checklistCommand = new SqlCommand(@"
            SELECT PhaseTaskChecklistItemId, PhaseTaskId, Title, IsCompleted, SortOrder
            FROM dbo.ProjectPhaseTaskChecklistItem
            WHERE PhaseTaskId IN (SELECT PhaseTaskId FROM dbo.ProjectPhaseTask WHERE ProjectId = @ProjectId)
            ORDER BY PhaseTaskId, SortOrder, PhaseTaskChecklistItemId", connection))
        {
            checklistCommand.Parameters.AddWithValue("@ProjectId", projectId);
            using var reader = await checklistCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var task = items.FirstOrDefault(x => x.PhaseTaskId == reader.GetInt32(1));
                if (task == null) continue;
                task.ChecklistItems.Add(new ProjectPhaseTaskChecklistItem
                {
                    PhaseTaskChecklistItemId = reader.GetInt32(0),
                    PhaseTaskId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    IsCompleted = !reader.IsDBNull(3) && reader.GetBoolean(3),
                    SortOrder = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                });
            }
        }

        using (var linkCommand = new SqlCommand(@"
            SELECT PhaseTaskLinkItemId, PhaseTaskId, FileName, FileUrl, SortOrder
            FROM dbo.ProjectPhaseTaskLinkItem
            WHERE PhaseTaskId IN (SELECT PhaseTaskId FROM dbo.ProjectPhaseTask WHERE ProjectId = @ProjectId)
            ORDER BY PhaseTaskId, SortOrder, PhaseTaskLinkItemId", connection))
        {
            linkCommand.Parameters.AddWithValue("@ProjectId", projectId);
            using var reader = await linkCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var task = items.FirstOrDefault(x => x.PhaseTaskId == reader.GetInt32(1));
                if (task == null) continue;
                task.FileLinks.Add(new ProjectPhaseTaskLinkItem
                {
                    PhaseTaskLinkItemId = reader.GetInt32(0),
                    PhaseTaskId = reader.GetInt32(1),
                    FileName = reader.GetString(2),
                    FileUrl = reader.GetString(3),
                    SortOrder = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                });
            }
        }

        return items;
    }

    public async Task<List<ProjectTaskAssigneeOption>> GetProjectTaskAssigneesAsync(Guid projectId)
    {
        var items = new List<ProjectTaskAssigneeOption>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(@"
            SELECT DISTINCT e.EmployeeId, e.EmployeeNameAr
            FROM dbo.EmployeeAccount e
            WHERE e.EmployeeId IN (
                SELECT p.ResponsibleUserId FROM dbo.Projects p WHERE p.ProjectId = @ProjectId
                UNION
                SELECT tm.EmployeeId FROM dbo.ProjectTeamMembers tm WHERE tm.ProjectId = @ProjectId
            )
            ORDER BY e.EmployeeNameAr", connection);
        command.Parameters.AddWithValue("@ProjectId", projectId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            items.Add(new ProjectTaskAssigneeOption
            {
                EmployeeId = reader.GetInt32(0),
                EmployeeName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
            });
        }
        return items;
    }

    public Task<List<ProjectActivityTemplateItem>> GetProjectActivityTemplatesAsync(Guid projectId)
        => Task.FromResult(new List<ProjectActivityTemplateItem>());

    public async Task<int> SaveProjectPhaseAsync(ProjectPhaseSaveModel model, int currentUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        if (model.PhaseId > 0)
        {
            using var updateCommand = new SqlCommand(@"
                UPDATE dbo.ProjectPhase
                SET PhaseName = @PhaseName,
                    PhaseCode = @PhaseCode,
                    DisplayOrder = @DisplayOrder,
                    PhaseStatus = @PhaseStatus,
                    PlannedStartDate = @PlannedStartDate,
                    PlannedEndDate = @PlannedEndDate,
                    ActualStartDate = @ActualStartDate,
                    ActualEndDate = @ActualEndDate,
                    Notes = @Notes,
                    ModifiedOn = SYSDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE PhaseId = @PhaseId AND ProjectId = @ProjectId", connection);
            FillPhaseParameters(updateCommand, model, currentUserId);
            updateCommand.Parameters.AddWithValue("@PhaseId", model.PhaseId);
            await updateCommand.ExecuteNonQueryAsync();
            return model.PhaseId;
        }

        using var command = new SqlCommand(@"
            INSERT INTO dbo.ProjectPhase
                (ProjectId, PhaseName, PhaseCode, DisplayOrder, PhaseStatus, PlannedStartDate, PlannedEndDate, ActualStartDate, ActualEndDate, Notes, CreatedBy)
            VALUES
                (@ProjectId, @PhaseName, @PhaseCode, @DisplayOrder, @PhaseStatus, @PlannedStartDate, @PlannedEndDate, @ActualStartDate, @ActualEndDate, @Notes, @CreatedBy);
            SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);
        FillPhaseParameters(command, model, currentUserId, false);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<int> SaveProjectPhaseActivityAsync(ProjectPhaseActivitySaveModel model, int currentUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        if (model.PhaseActivityId > 0)
        {
            using var updateCommand = new SqlCommand(@"
                UPDATE dbo.ProjectPhaseActivity
                SET PhaseId = @PhaseId,
                    ProjectId = @ProjectId,
                    ActivityName = @ActivityName,
                    IndicatorName = @IndicatorName,
                    EstimatedCost = @EstimatedCost,
                    TargetValue = @TargetValue,
                    VerificationSource = @VerificationSource,
                    RisksAndAssumptions = @RisksAndAssumptions,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    ModifiedOn = SYSDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE PhaseActivityId = @PhaseActivityId", connection);
            FillActivityParameters(updateCommand, model, currentUserId);
            updateCommand.Parameters.AddWithValue("@PhaseActivityId", model.PhaseActivityId);
            await updateCommand.ExecuteNonQueryAsync();
            return model.PhaseActivityId;
        }

        using var command = new SqlCommand(@"
            INSERT INTO dbo.ProjectPhaseActivity
                (PhaseId, ProjectId, ActivityName, IndicatorName, EstimatedCost, TargetValue, VerificationSource, RisksAndAssumptions, StartDate, EndDate, CreatedBy)
            VALUES
                (@PhaseId, @ProjectId, @ActivityName, @IndicatorName, @EstimatedCost, @TargetValue, @VerificationSource, @RisksAndAssumptions, @StartDate, @EndDate, @CreatedBy);
            SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);
        FillActivityParameters(command, model, currentUserId, false);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<int> SaveProjectPhaseTaskAsync(ProjectPhaseTaskSaveModel model, int currentUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        int taskId;
        if (model.PhaseTaskId > 0)
        {
            using var updateCommand = new SqlCommand(@"
                UPDATE dbo.ProjectPhaseTask
                SET PhaseActivityId = @PhaseActivityId,
                    ProjectId = @ProjectId,
                    TaskName = @TaskName,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    Priority = @Priority,
                    PercentComplete = @PercentComplete,
                    Notes = @Notes,
                    DependentOnPhaseTaskId = @DependentOnPhaseTaskId,
                    DependencyType = @DependencyType,
                    ModifiedOn = SYSDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE PhaseTaskId = @PhaseTaskId", connection, transaction);
            FillTaskParameters(updateCommand, model, currentUserId);
            updateCommand.Parameters.AddWithValue("@PhaseTaskId", model.PhaseTaskId);
            await updateCommand.ExecuteNonQueryAsync();
            taskId = model.PhaseTaskId;
        }
        else
        {
            using var command = new SqlCommand(@"
                INSERT INTO dbo.ProjectPhaseTask
                    (PhaseActivityId, ProjectId, TaskName, StartDate, EndDate, Priority, PercentComplete, Notes, DependentOnPhaseTaskId, DependencyType, CreatedBy)
                VALUES
                    (@PhaseActivityId, @ProjectId, @TaskName, @StartDate, @EndDate, @Priority, @PercentComplete, @Notes, @DependentOnPhaseTaskId, @DependencyType, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", connection, transaction);
            FillTaskParameters(command, model, currentUserId, false);
            taskId = Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        using (var deleteAssignee = new SqlCommand("DELETE FROM dbo.ProjectPhaseTaskAssignee WHERE PhaseTaskId = @PhaseTaskId", connection, transaction))
        {
            deleteAssignee.Parameters.AddWithValue("@PhaseTaskId", taskId);
            await deleteAssignee.ExecuteNonQueryAsync();
        }

        foreach (var employeeId in model.AssignedEmployeeIds.Where(x => x > 0).Distinct())
        {
            using var insertAssignee = new SqlCommand(@"
                INSERT INTO dbo.ProjectPhaseTaskAssignee (PhaseTaskId, EmployeeId)
                VALUES (@PhaseTaskId, @EmployeeId)", connection, transaction);
            insertAssignee.Parameters.AddWithValue("@PhaseTaskId", taskId);
            insertAssignee.Parameters.AddWithValue("@EmployeeId", employeeId);
            await insertAssignee.ExecuteNonQueryAsync();
        }

        using (var deleteChecklist = new SqlCommand("DELETE FROM dbo.ProjectPhaseTaskChecklistItem WHERE PhaseTaskId = @PhaseTaskId", connection, transaction))
        {
            deleteChecklist.Parameters.AddWithValue("@PhaseTaskId", taskId);
            await deleteChecklist.ExecuteNonQueryAsync();
        }

        foreach (var checklistItem in model.ChecklistItems.Where(x => !string.IsNullOrWhiteSpace(x.Title)).Select((x, index) => new { Item = x, Index = index + 1 }))
        {
            using var insertChecklist = new SqlCommand(@"
                INSERT INTO dbo.ProjectPhaseTaskChecklistItem (PhaseTaskId, Title, IsCompleted, SortOrder)
                VALUES (@PhaseTaskId, @Title, @IsCompleted, @SortOrder)", connection, transaction);
            insertChecklist.Parameters.AddWithValue("@PhaseTaskId", taskId);
            insertChecklist.Parameters.AddWithValue("@Title", checklistItem.Item.Title.Trim());
            insertChecklist.Parameters.AddWithValue("@IsCompleted", checklistItem.Item.IsCompleted);
            insertChecklist.Parameters.AddWithValue("@SortOrder", checklistItem.Index);
            await insertChecklist.ExecuteNonQueryAsync();
        }

        using (var deleteLinks = new SqlCommand("DELETE FROM dbo.ProjectPhaseTaskLinkItem WHERE PhaseTaskId = @PhaseTaskId", connection, transaction))
        {
            deleteLinks.Parameters.AddWithValue("@PhaseTaskId", taskId);
            await deleteLinks.ExecuteNonQueryAsync();
        }

        foreach (var linkItem in model.FileLinks.Where(x => !string.IsNullOrWhiteSpace(x.FileName) && !string.IsNullOrWhiteSpace(x.FileUrl)).Select((x, index) => new { Item = x, Index = index + 1 }))
        {
            using var insertLink = new SqlCommand(@"
                INSERT INTO dbo.ProjectPhaseTaskLinkItem (PhaseTaskId, FileName, FileUrl, SortOrder)
                VALUES (@PhaseTaskId, @FileName, @FileUrl, @SortOrder)", connection, transaction);
            insertLink.Parameters.AddWithValue("@PhaseTaskId", taskId);
            insertLink.Parameters.AddWithValue("@FileName", linkItem.Item.FileName.Trim());
            insertLink.Parameters.AddWithValue("@FileUrl", linkItem.Item.FileUrl.Trim());
            insertLink.Parameters.AddWithValue("@SortOrder", linkItem.Index);
            await insertLink.ExecuteNonQueryAsync();
        }

        await transaction.CommitAsync();
        return taskId;
    }

    public async Task DeleteProjectPhaseAsync(Guid projectId, int phaseId, int currentUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        using (var clearDependencies = new SqlCommand(@"
            UPDATE t
            SET t.DependentOnPhaseTaskId = NULL,
                t.DependencyType = NULL,
                t.ModifiedOn = SYSDATETIME(),
                t.ModifiedBy = @ModifiedBy
            FROM dbo.ProjectPhaseTask t
            INNER JOIN dbo.ProjectPhaseTask dep ON dep.PhaseTaskId = t.DependentOnPhaseTaskId
            INNER JOIN dbo.ProjectPhaseActivity depAct ON depAct.PhaseActivityId = dep.PhaseActivityId
            WHERE depAct.PhaseId = @PhaseId AND depAct.ProjectId = @ProjectId", connection, transaction))
        {
            clearDependencies.Parameters.AddWithValue("@ModifiedBy", currentUserId == 0 ? DBNull.Value : currentUserId);
            clearDependencies.Parameters.AddWithValue("@PhaseId", phaseId);
            clearDependencies.Parameters.AddWithValue("@ProjectId", projectId);
            await clearDependencies.ExecuteNonQueryAsync();
        }

        using var deleteCommand = new SqlCommand("DELETE FROM dbo.ProjectPhase WHERE PhaseId = @PhaseId AND ProjectId = @ProjectId", connection, transaction);
        deleteCommand.Parameters.AddWithValue("@PhaseId", phaseId);
        deleteCommand.Parameters.AddWithValue("@ProjectId", projectId);
        await deleteCommand.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
    }

    public async Task DeleteProjectPhaseActivityAsync(Guid projectId, int phaseActivityId, int currentUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        using (var clearDependencies = new SqlCommand(@"
            UPDATE t
            SET t.DependentOnPhaseTaskId = NULL,
                t.DependencyType = NULL,
                t.ModifiedOn = SYSDATETIME(),
                t.ModifiedBy = @ModifiedBy
            FROM dbo.ProjectPhaseTask t
            INNER JOIN dbo.ProjectPhaseTask dep ON dep.PhaseTaskId = t.DependentOnPhaseTaskId
            WHERE dep.PhaseActivityId = @PhaseActivityId AND dep.ProjectId = @ProjectId", connection, transaction))
        {
            clearDependencies.Parameters.AddWithValue("@ModifiedBy", currentUserId == 0 ? DBNull.Value : currentUserId);
            clearDependencies.Parameters.AddWithValue("@PhaseActivityId", phaseActivityId);
            clearDependencies.Parameters.AddWithValue("@ProjectId", projectId);
            await clearDependencies.ExecuteNonQueryAsync();
        }

        using var deleteCommand = new SqlCommand("DELETE FROM dbo.ProjectPhaseActivity WHERE PhaseActivityId = @PhaseActivityId AND ProjectId = @ProjectId", connection, transaction);
        deleteCommand.Parameters.AddWithValue("@PhaseActivityId", phaseActivityId);
        deleteCommand.Parameters.AddWithValue("@ProjectId", projectId);
        await deleteCommand.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
    }

    public async Task DeleteProjectPhaseTaskAsync(Guid projectId, int phaseTaskId, int currentUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        using (var clearDependencies = new SqlCommand(@"
            UPDATE dbo.ProjectPhaseTask
            SET DependentOnPhaseTaskId = NULL,
                DependencyType = NULL,
                ModifiedOn = SYSDATETIME(),
                ModifiedBy = @ModifiedBy
            WHERE DependentOnPhaseTaskId = @PhaseTaskId", connection, transaction))
        {
            clearDependencies.Parameters.AddWithValue("@ModifiedBy", currentUserId == 0 ? DBNull.Value : currentUserId);
            clearDependencies.Parameters.AddWithValue("@PhaseTaskId", phaseTaskId);
            await clearDependencies.ExecuteNonQueryAsync();
        }

        using var deleteCommand = new SqlCommand("DELETE FROM dbo.ProjectPhaseTask WHERE PhaseTaskId = @PhaseTaskId AND ProjectId = @ProjectId", connection, transaction);
        deleteCommand.Parameters.AddWithValue("@PhaseTaskId", phaseTaskId);
        deleteCommand.Parameters.AddWithValue("@ProjectId", projectId);
        await deleteCommand.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
    }

    private async Task<List<MyProjectsTaskDetailItem>> BuildDashboardTaskDetailsAsync(List<MyProjectsApprovedItem> projects)
    {
        var items = new List<MyProjectsTaskDetailItem>();
        foreach (var project in projects)
        {
            var phases = await GetProjectPhasesAsync(project.ProjectId);
            var activities = await GetProjectPhaseActivitiesAsync(project.ProjectId);
            var tasks = await GetProjectPhaseTasksAsync(project.ProjectId);

            foreach (var task in tasks)
            {
                var activity = activities.FirstOrDefault(x => x.PhaseActivityId == task.PhaseActivityId);
                var phase = activity == null ? null : phases.FirstOrDefault(x => x.PhaseId == activity.PhaseId);
                items.Add(new MyProjectsTaskDetailItem
                {
                    PhaseTaskId = task.PhaseTaskId,
                    PhaseActivityId = task.PhaseActivityId,
                    ProjectId = task.ProjectId,
                    ProjectName = project.ProjectName,
                    TaskName = task.TaskName,
                    PhaseName = phase?.PhaseName,
                    ActivityName = activity?.ActivityName,
                    StartDate = task.StartDate,
                    EndDate = task.EndDate,
                    Priority = task.Priority,
                    PercentComplete = task.PercentComplete,
                    Notes = task.Notes,
                    DependentOnPhaseTaskId = task.DependentOnPhaseTaskId,
                    DependencyType = task.DependencyType,
                    DependentOnTaskName = task.DependentOnTaskName,
                    ResponsibleUserId = project.ResponsibleUserId,
                    Assignees = task.Assignees,
                    ChecklistItems = task.ChecklistItems,
                    FileLinks = task.FileLinks
                });
            }
        }
        return items;
    }

    private static void FillPhaseParameters(SqlCommand command, ProjectPhaseSaveModel model, int currentUserId, bool isUpdate = true)
    {
        command.Parameters.AddWithValue("@ProjectId", model.ProjectId);
        command.Parameters.AddWithValue("@PhaseName", model.PhaseName.Trim());
        command.Parameters.AddWithValue("@PhaseCode", (object?)model.PhaseCode ?? DBNull.Value);
        command.Parameters.AddWithValue("@DisplayOrder", model.DisplayOrder);
        command.Parameters.AddWithValue("@PhaseStatus", string.IsNullOrWhiteSpace(model.PhaseStatus) ? "NotStarted" : model.PhaseStatus);
        command.Parameters.AddWithValue("@PlannedStartDate", (object?)model.PlannedStartDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@PlannedEndDate", (object?)model.PlannedEndDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@ActualStartDate", (object?)model.ActualStartDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@ActualEndDate", (object?)model.ActualEndDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@Notes", (object?)model.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue(isUpdate ? "@ModifiedBy" : "@CreatedBy", currentUserId == 0 ? DBNull.Value : currentUserId);
    }

    private static void FillActivityParameters(SqlCommand command, ProjectPhaseActivitySaveModel model, int currentUserId, bool isUpdate = true)
    {
        command.Parameters.AddWithValue("@PhaseId", model.PhaseId);
        command.Parameters.AddWithValue("@ProjectId", model.ProjectId);
        command.Parameters.AddWithValue("@ActivityName", model.ActivityName.Trim());
        command.Parameters.AddWithValue("@IndicatorName", (object?)model.IndicatorName ?? DBNull.Value);
        command.Parameters.AddWithValue("@EstimatedCost", (object?)model.EstimatedCost ?? DBNull.Value);
        command.Parameters.AddWithValue("@TargetValue", (object?)model.TargetValue ?? DBNull.Value);
        command.Parameters.AddWithValue("@VerificationSource", (object?)model.VerificationSource ?? DBNull.Value);
        command.Parameters.AddWithValue("@RisksAndAssumptions", (object?)model.RisksAndAssumptions ?? DBNull.Value);
        command.Parameters.AddWithValue("@StartDate", (object?)model.StartDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@EndDate", (object?)model.EndDate ?? DBNull.Value);
        command.Parameters.AddWithValue(isUpdate ? "@ModifiedBy" : "@CreatedBy", currentUserId == 0 ? DBNull.Value : currentUserId);
    }

    private static void FillTaskParameters(SqlCommand command, ProjectPhaseTaskSaveModel model, int currentUserId, bool isUpdate = true)
    {
        command.Parameters.AddWithValue("@PhaseActivityId", model.PhaseActivityId);
        command.Parameters.AddWithValue("@ProjectId", model.ProjectId);
        command.Parameters.AddWithValue("@TaskName", model.TaskName.Trim());
        command.Parameters.AddWithValue("@StartDate", (object?)model.StartDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@EndDate", (object?)model.EndDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@Priority", string.IsNullOrWhiteSpace(model.Priority) ? "Medium" : model.Priority);
        command.Parameters.AddWithValue("@PercentComplete", model.PercentComplete);
        command.Parameters.AddWithValue("@Notes", (object?)model.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue("@DependentOnPhaseTaskId", (object?)model.DependentOnPhaseTaskId ?? DBNull.Value);
        command.Parameters.AddWithValue("@DependencyType", (object?)model.DependencyType ?? DBNull.Value);
        command.Parameters.AddWithValue(isUpdate ? "@ModifiedBy" : "@CreatedBy", currentUserId == 0 ? DBNull.Value : currentUserId);
    }

    public async Task<Guid> SaveProjectAsync(ProjectSaveModel model, int currentUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        Guid projectIdToReturn;

        if (model.ProjectId.HasValue && model.ProjectId.Value != Guid.Empty)
        {
            using var updateCommand = new SqlCommand(@"
                UPDATE dbo.Projects
                SET ProjectName = @ProjectName,
                    ProjectCode = @ProjectCode,
                    ProjectType = @ProjectType,
                    DepartmentCode = @DepartmentCode,
                    DepartmentName = @DepartmentName,
                    ResponsibleUserId = @ResponsibleUserId,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    ProjectDescription = @ProjectDescription,
                    SponsorUserId = @SponsorUserId,
                    Priority = @Priority,
                    PlannedBudget = @PlannedBudget,
                    ActualCost = @ActualCost,
                    HealthStatus = @HealthStatus,
                    Archived = @Archived,
                    StatusId = @StatusId,
                    ModifiedOn = SYSDATETIME(),
                    ModifiedBy = @ModifiedBy
                WHERE ProjectId = @ProjectId", connection);
            
            updateCommand.Parameters.AddWithValue("@ProjectId", model.ProjectId.Value);
            updateCommand.Parameters.AddWithValue("@ProjectName", model.ProjectName.Trim());
            updateCommand.Parameters.AddWithValue("@ProjectCode", (object?)model.ProjectCode ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@ProjectType", (object?)model.ProjectType ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@DepartmentCode", (object?)model.DepartmentCode ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@DepartmentName", (object?)model.DepartmentName ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@ResponsibleUserId", (object?)model.ResponsibleUserId ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@StartDate", (object?)model.StartDate ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@EndDate", (object?)model.EndDate ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@ProjectDescription", (object?)model.ProjectDescription ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@SponsorUserId", (object?)model.SponsorUserId ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@Priority", (object?)model.Priority ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@PlannedBudget", (object?)model.PlannedBudget ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@ActualCost", (object?)model.ActualCost ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@HealthStatus", (object?)model.HealthStatus ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@Archived", model.Archived);
            updateCommand.Parameters.AddWithValue("@StatusId", (object?)model.StatusId ?? DBNull.Value);
            updateCommand.Parameters.AddWithValue("@ModifiedBy", currentUserId == 0 ? DBNull.Value : currentUserId);
            
            await updateCommand.ExecuteNonQueryAsync();
            projectIdToReturn = model.ProjectId.Value;
        }
        else
        {
            Guid newProjectId = Guid.NewGuid();
            using var command = new SqlCommand(@"
                INSERT INTO dbo.Projects
                    (ProjectId, ProjectName, ProjectCode, ProjectType, DepartmentCode, DepartmentName, ResponsibleUserId, StartDate, EndDate, CreatedBy,
                     ProjectDescription, SponsorUserId, Priority, PlannedBudget, ActualCost, HealthStatus, Archived, StatusId)
                VALUES
                    (@ProjectId, @ProjectName, @ProjectCode, @ProjectType, @DepartmentCode, @DepartmentName, @ResponsibleUserId, @StartDate, @EndDate, @CreatedBy,
                     @ProjectDescription, @SponsorUserId, @Priority, @PlannedBudget, @ActualCost, @HealthStatus, @Archived, @StatusId);", connection);
            
            command.Parameters.AddWithValue("@ProjectId", newProjectId);
            command.Parameters.AddWithValue("@ProjectName", model.ProjectName.Trim());
            command.Parameters.AddWithValue("@ProjectCode", (object?)model.ProjectCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProjectType", (object?)model.ProjectType ?? DBNull.Value);
            command.Parameters.AddWithValue("@DepartmentCode", (object?)model.DepartmentCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@DepartmentName", (object?)model.DepartmentName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ResponsibleUserId", (object?)model.ResponsibleUserId ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartDate", (object?)model.StartDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@EndDate", (object?)model.EndDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProjectDescription", (object?)model.ProjectDescription ?? DBNull.Value);
            command.Parameters.AddWithValue("@SponsorUserId", (object?)model.SponsorUserId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Priority", (object?)model.Priority ?? DBNull.Value);
            command.Parameters.AddWithValue("@PlannedBudget", (object?)model.PlannedBudget ?? DBNull.Value);
            command.Parameters.AddWithValue("@ActualCost", (object?)model.ActualCost ?? DBNull.Value);
            command.Parameters.AddWithValue("@HealthStatus", (object?)model.HealthStatus ?? DBNull.Value);
            command.Parameters.AddWithValue("@Archived", model.Archived);
            command.Parameters.AddWithValue("@StatusId", (object?)model.StatusId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedBy", currentUserId == 0 ? DBNull.Value : currentUserId);

            await command.ExecuteNonQueryAsync();
            projectIdToReturn = newProjectId;
        }

        // Save Team Members
        using var deleteTeamCommand = new SqlCommand("DELETE FROM dbo.ProjectTeamMembers WHERE ProjectId = @ProjectId", connection);
        deleteTeamCommand.Parameters.AddWithValue("@ProjectId", projectIdToReturn);
        await deleteTeamCommand.ExecuteNonQueryAsync();

        if (model.TeamMemberIds != null && model.TeamMemberIds.Any())
        {
            var distinctTeamMembers = model.TeamMemberIds.Distinct().ToList();
            foreach (var memberId in distinctTeamMembers)
            {
                using var insertTeamCommand = new SqlCommand(@"
                    INSERT INTO dbo.ProjectTeamMembers (ProjectId, EmployeeId) 
                    VALUES (@ProjectId, @EmployeeId)", connection);
                insertTeamCommand.Parameters.AddWithValue("@ProjectId", projectIdToReturn);
                insertTeamCommand.Parameters.AddWithValue("@EmployeeId", memberId);
                await insertTeamCommand.ExecuteNonQueryAsync();
            }
        }

        return projectIdToReturn;
    }

    public async Task DeleteProjectAsync(Guid projectId, int currentUserId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        using var transaction = connection.BeginTransaction();
        try
        {
            // Delete Tasks
            using var deleteTasks = new SqlCommand(@"
                DELETE t FROM dbo.ProjectPhaseTasks t
                INNER JOIN dbo.ProjectPhaseActivities a ON t.PhaseActivityId = a.PhaseActivityId
                INNER JOIN dbo.ProjectPhases p ON a.PhaseId = p.PhaseId
                WHERE p.ProjectId = @ProjectId", connection, transaction);
            deleteTasks.Parameters.AddWithValue("@ProjectId", projectId);
            await deleteTasks.ExecuteNonQueryAsync();

            // Delete Activities
            using var deleteActivities = new SqlCommand(@"
                DELETE a FROM dbo.ProjectPhaseActivities a
                INNER JOIN dbo.ProjectPhases p ON a.PhaseId = p.PhaseId
                WHERE p.ProjectId = @ProjectId", connection, transaction);
            deleteActivities.Parameters.AddWithValue("@ProjectId", projectId);
            await deleteActivities.ExecuteNonQueryAsync();

            // Delete Phases
            using var deletePhases = new SqlCommand("DELETE FROM dbo.ProjectPhases WHERE ProjectId = @ProjectId", connection, transaction);
            deletePhases.Parameters.AddWithValue("@ProjectId", projectId);
            await deletePhases.ExecuteNonQueryAsync();

            // Delete Project
            using var deleteProject = new SqlCommand("DELETE FROM dbo.Projects WHERE ProjectId = @ProjectId", connection, transaction);
            deleteProject.Parameters.AddWithValue("@ProjectId", projectId);
            await deleteProject.ExecuteNonQueryAsync();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
