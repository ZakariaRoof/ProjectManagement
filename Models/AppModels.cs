using System.ComponentModel.DataAnnotations;

namespace ProjectManagementApp.Models;

public class LoginViewModel
{
    [Required]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
    public string? ErrorMessage { get; set; }
}

public class CurrentUser
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public int? DepartmentCode { get; set; }
    public string? DepartmentName { get; set; }
    public string? PositionName { get; set; }
}

public class MyProjectsApprovedPageViewModel
{
    public List<MyProjectsApprovedItem> Projects { get; set; } = new();
    public bool IsAdministrator { get; set; }
    public bool IsResponsibleUserScope { get; set; }
    public bool IsTeamMemberScope { get; set; }
}

public class MyProjectsApprovedItem
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectTypeText { get; set; }
    public int? DepartmentCode { get; set; }
    public string? DepartmentName { get; set; }
    public string? SectionNames { get; set; }
    public int? ResponsibleUserId { get; set; }
    public string? ResponsibleUserName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal CurrentProgressPercent { get; set; }
    
    // New Fields
    public string? ProjectDescription { get; set; }
    public int? SponsorUserId { get; set; }
    public byte? Priority { get; set; }
    public decimal? PlannedBudget { get; set; }
    public decimal? ActualCost { get; set; }
    public byte? HealthStatus { get; set; }
    public bool Archived { get; set; }
    
    public int? StatusId { get; set; }
    public string? StatusNameAr { get; set; }
    public string? StatusNameEn { get; set; }
    public string? StatusColorClass { get; set; }
    
    public List<TeamMemberItem> TeamMembers { get; set; } = new();
}

public class ProjectStatusItem
{
    public int StatusId { get; set; }
    public string StatusNameAr { get; set; } = string.Empty;
    public string StatusNameEn { get; set; } = string.Empty;
    public string? ColorClass { get; set; }
}

public class TeamMemberItem
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? Photo { get; set; }
    public string? DepartmentName { get; set; }
    public string? Email { get; set; }
}

public class EmployeeSearchItem
{
    public int id { get; set; }
    public string text { get; set; } = string.Empty;
    public string? photo { get; set; }
}

public class ManageMyProjectViewModel
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectTypeText { get; set; }
    public string? DepartmentName { get; set; }
    public string? ResponsibleUserName { get; set; }
    public bool CanManagePhases { get; set; }
    public List<ProjectPhaseItem> Phases { get; set; } = new();
    public List<ProjectActivityTemplateItem> AvailableProjectActivities { get; set; } = new();
    public List<ProjectTaskAssigneeOption> AvailableTaskAssignees { get; set; } = new();
    public List<ProjectTaskDependencyOption> AvailableTaskDependencyOptions { get; set; } = new();
}

public class MyProjectsTasksDashboardViewModel
{
    public int? DepartmentCode { get; set; }
    public string? DepartmentName { get; set; }
    public int CurrentUserId { get; set; }
    public bool IsAdministrator { get; set; }
    public List<MyProjectsApprovedItem> Projects { get; set; } = new();
    public List<MyProjectsTaskTeamMemberItem> TeamMembers { get; set; } = new();
    public List<MyProjectsTaskAssignmentItem> Assignments { get; set; } = new();
    public List<MyProjectsTaskDetailItem> DetailedTasks { get; set; } = new();
}

public class MyProjectsTaskTeamMemberItem
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? PositionName { get; set; }
    public string? DepartmentName { get; set; }
    public string? PictureBase64 { get; set; }
}

public class MyProjectsTaskAssignmentItem
{
    public int PhaseTaskId { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string? PhaseName { get; set; }
    public string? ActivityName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public int PercentComplete { get; set; }
}

public class MyProjectsTaskDetailItem
{
    public int PhaseTaskId { get; set; }
    public int PhaseActivityId { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string? PhaseName { get; set; }
    public string? ActivityName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public int PercentComplete { get; set; }
    public string? Notes { get; set; }
    public int? DependentOnPhaseTaskId { get; set; }
    public string? DependencyType { get; set; }
    public string? DependentOnTaskName { get; set; }
    public int? ResponsibleUserId { get; set; }
    public List<ProjectTaskAssigneeOption> Assignees { get; set; } = new();
    public List<ProjectPhaseTaskChecklistItem> ChecklistItems { get; set; } = new();
    public List<ProjectPhaseTaskLinkItem> FileLinks { get; set; } = new();
}

public class ProjectPhaseItem
{
    public int PhaseId { get; set; }
    public Guid ProjectId { get; set; }
    public string PhaseName { get; set; } = string.Empty;
    public string? PhaseCode { get; set; }
    public int DisplayOrder { get; set; }
    public string PhaseStatus { get; set; } = "NotStarted";
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string? Notes { get; set; }
    public List<ProjectPhaseActivityItem> Activities { get; set; } = new();
}

public class ProjectPhaseActivityItem
{
    public int PhaseActivityId { get; set; }
    public int PhaseId { get; set; }
    public Guid ProjectId { get; set; }
    public int? SourceActivityId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public string? IndicatorName { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string? TargetValue { get; set; }
    public string? VerificationSource { get; set; }
    public string? RisksAndAssumptions { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<ProjectPhaseTaskItem> Tasks { get; set; } = new();
}

public class ProjectPhaseTaskItem
{
    public int PhaseTaskId { get; set; }
    public int PhaseActivityId { get; set; }
    public Guid ProjectId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public int PercentComplete { get; set; }
    public string? Notes { get; set; }
    public int? DependentOnPhaseTaskId { get; set; }
    public string? DependencyType { get; set; }
    public string? DependentOnTaskName { get; set; }
    public List<ProjectTaskAssigneeOption> Assignees { get; set; } = new();
    public List<ProjectPhaseTaskChecklistItem> ChecklistItems { get; set; } = new();
    public List<ProjectPhaseTaskLinkItem> FileLinks { get; set; } = new();
}

public class ProjectTaskAssigneeOption
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
}

public class ProjectTaskDependencyOption
{
    public int PhaseTaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public int PhaseActivityId { get; set; }
    public string? ActivityName { get; set; }
    public string? PhaseName { get; set; }
}

public class ProjectPhaseTaskChecklistItem
{
    public int PhaseTaskChecklistItemId { get; set; }
    public int PhaseTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int SortOrder { get; set; }
}

public class ProjectPhaseTaskLinkItem
{
    public int PhaseTaskLinkItemId { get; set; }
    public int PhaseTaskId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class ProjectPhaseSaveModel
{
    public int PhaseId { get; set; }
    public Guid ProjectId { get; set; }
    public string PhaseName { get; set; } = string.Empty;
    public string? PhaseCode { get; set; }
    public int DisplayOrder { get; set; } = 1;
    public string PhaseStatus { get; set; } = "NotStarted";
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string? Notes { get; set; }
}

public class ProjectActivityTemplateItem
{
    public int ActivityId { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public string? IndicatorName { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string? TargetValue { get; set; }
    public string? VerificationSource { get; set; }
    public string? RisksAndAssumptions { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ProjectPhaseActivitySaveModel
{
    public int PhaseActivityId { get; set; }
    public int PhaseId { get; set; }
    public Guid ProjectId { get; set; }
    public int? SourceActivityId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string ActivityName { get; set; } = string.Empty;
    public string? IndicatorName { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string? TargetValue { get; set; }
    public string? VerificationSource { get; set; }
    public string? RisksAndAssumptions { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ProjectPhaseTaskSaveModel
{
    public int PhaseTaskId { get; set; }
    public int PhaseActivityId { get; set; }
    public Guid ProjectId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public int PercentComplete { get; set; }
    public string? Notes { get; set; }
    public int? DependentOnPhaseTaskId { get; set; }
    public string? DependencyType { get; set; }
    public List<int> AssignedEmployeeIds { get; set; } = new();
    public List<ProjectPhaseTaskChecklistItem> ChecklistItems { get; set; } = new();
    public List<ProjectPhaseTaskLinkItem> FileLinks { get; set; } = new();
}

public class ProjectSaveModel
{
    public Guid? ProjectId { get; set; }
    [Required]
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectCode { get; set; }
    public string? ProjectType { get; set; }
    public int? DepartmentCode { get; set; }
    public string? DepartmentName { get; set; }
    public int? ResponsibleUserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // New Fields
    public string? ProjectDescription { get; set; }
    public int? SponsorUserId { get; set; }
    public byte? Priority { get; set; }
    public decimal? PlannedBudget { get; set; }
    public decimal? ActualCost { get; set; }
    public byte? HealthStatus { get; set; }
    public bool Archived { get; set; }
    public int? StatusId { get; set; }
    
    public List<int> TeamMemberIds { get; set; } = new();
}
