using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementApp.Models;
using ProjectManagementApp.Services;

namespace ProjectManagementApp.Controllers;

[Authorize]
public class MyProjectsController : Controller
{
    private readonly IMyProjectsService _myProjectsService;
    private readonly IAuthService _authService;
    private readonly ILocalizationService _localization;

    public MyProjectsController(IMyProjectsService myProjectsService, IAuthService authService, ILocalizationService localization)
    {
        _myProjectsService = myProjectsService;
        _authService = authService;
        _localization = localization;
    }

    private int GetCurrentUserId()
    {
        var employeeId = User.FindFirst("EmployeeId")?.Value;
        return int.TryParse(employeeId, out var id) ? id : 0;
    }

    private int? GetCurrentDepartmentCode()
    {
        var departmentCode = User.FindFirst("DepartmentCode")?.Value;
        return int.TryParse(departmentCode, out var id) ? id : null;
    }

    private bool IsAdministrator() => User.IsInRole("Administrator");

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(ApprovedProjects));
    }

    [HttpGet]
    public async Task<IActionResult> SearchEmployees(string q)
    {
        var results = await _myProjectsService.SearchEmployeesAsync(q);
        return Json(new { results = results });
    }

    [HttpGet]
    public async Task<IActionResult> ApprovedProjects()
    {
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser == null)
            return RedirectToAction("Login", "Account");

        ViewBag.CurrentUser = currentUser;
        ViewBag.ProjectStatuses = await _myProjectsService.GetProjectStatusesAsync();
        var projects = await _myProjectsService.GetApprovedProjectsAsync(GetCurrentUserId(), GetCurrentDepartmentCode(), IsAdministrator());
        return View(new MyProjectsApprovedPageViewModel
        {
            Projects = projects,
            IsAdministrator = IsAdministrator()
        });
    }

    [HttpGet]
    public async Task<IActionResult> Tasks()
    {
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser == null)
            return RedirectToAction("Login", "Account");

        ViewBag.CurrentUser = currentUser;
        var viewModel = await _myProjectsService.GetTasksDashboardAsync(GetCurrentUserId(), GetCurrentDepartmentCode(), IsAdministrator());
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Manage(Guid projectId)
    {
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser == null)
            return RedirectToAction("Login", "Account");

        var project = await _myProjectsService.GetApprovedProjectByIdAsync(projectId, GetCurrentUserId(), IsAdministrator());
        if (project == null)
            return NotFound();

        ViewBag.CurrentUser = currentUser;
        return View(await BuildManageProjectViewModelAsync(project));
    }

    [HttpGet]
    public async Task<IActionResult> GanttChart(Guid projectId)
    {
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser == null)
            return RedirectToAction("Login", "Account");

        var project = await _myProjectsService.GetApprovedProjectByIdAsync(projectId, GetCurrentUserId(), IsAdministrator());
        if (project == null)
            return NotFound();

        ViewBag.CurrentUser = currentUser;
        return View(await BuildManageProjectViewModelAsync(project));
    }

    [HttpGet]
    public async Task<IActionResult> GetTaskDetail(Guid projectId, int phaseTaskId)
    {
        var project = await _myProjectsService.GetApprovedProjectByIdAsync(projectId, GetCurrentUserId(), IsAdministrator());
        if (project == null)
            return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

        var tasks = await _myProjectsService.GetProjectPhaseTasksAsync(projectId);
        var task = tasks.FirstOrDefault(x => x.PhaseTaskId == phaseTaskId);
        if (task == null)
            return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

        var activities = await _myProjectsService.GetProjectPhaseActivitiesAsync(projectId);
        var phases = await _myProjectsService.GetProjectPhasesAsync(projectId);
        var activity = activities.FirstOrDefault(x => x.PhaseActivityId == task.PhaseActivityId);
        var phase = activity == null ? null : phases.FirstOrDefault(x => x.PhaseId == activity.PhaseId);

        return Json(new
        {
            success = true,
            canEdit = CanEditTask(project, task),
            canDelete = CanDeleteTask(project),
            task = new
            {
                task.PhaseTaskId,
                task.PhaseActivityId,
                task.ProjectId,
                task.TaskName,
                task.StartDate,
                task.EndDate,
                task.Priority,
                task.PercentComplete,
                task.Notes,
                task.DependentOnPhaseTaskId,
                task.DependencyType,
                task.DependentOnTaskName,
                ProjectName = project.ProjectName,
                PhaseName = phase?.PhaseName,
                ActivityName = activity?.ActivityName,
                Assignees = task.Assignees,
                ChecklistItems = task.ChecklistItems,
                FileLinks = task.FileLinks
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePhase([FromBody] ProjectPhaseSaveModel model)
    {
        var project = await _myProjectsService.GetApprovedProjectByIdAsync(model.ProjectId, GetCurrentUserId(), IsAdministrator());
        if (project == null)
            return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

        if (!CanManageProject(project))
            return Json(new { success = false, message = _localization.GetString("AccessDenied") });

        if (string.IsNullOrWhiteSpace(model.PhaseName))
            return Json(new { success = false, message = _localization.GetString("PhaseNameRequired") });

        try
        {
            var phaseId = await _myProjectsService.SaveProjectPhaseAsync(model, GetCurrentUserId());
            return Json(new { success = true, phaseId, message = _localization.GetString("RecordCreatedSuccessfully") });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePhaseActivity([FromBody] ProjectPhaseActivitySaveModel model)
    {
        var project = await _myProjectsService.GetApprovedProjectByIdAsync(model.ProjectId, GetCurrentUserId(), IsAdministrator());
        if (project == null)
            return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

        if (!CanManageProject(project))
            return Json(new { success = false, message = _localization.GetString("AccessDenied") });

        if (string.IsNullOrWhiteSpace(model.ActivityName))
            return Json(new { success = false, message = _localization.GetString("ActivityRequired") });

        try
        {
            var activityId = await _myProjectsService.SaveProjectPhaseActivityAsync(model, GetCurrentUserId());
            return Json(new { success = true, activityId, message = _localization.GetString("RecordCreatedSuccessfully") });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePhaseTask([FromBody] ProjectPhaseTaskSaveModel model)
    {
        var project = await _myProjectsService.GetApprovedProjectByIdAsync(model.ProjectId, GetCurrentUserId(), IsAdministrator());
        if (project == null)
            return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

        if (model.PhaseTaskId > 0)
        {
            var existingTask = (await _myProjectsService.GetProjectPhaseTasksAsync(model.ProjectId)).FirstOrDefault(x => x.PhaseTaskId == model.PhaseTaskId);
            if (existingTask == null)
                return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

            if (!CanEditTask(project, existingTask))
                return Json(new { success = false, message = _localization.GetString("AccessDenied") });
        }
        else if (!CanManageProject(project))
        {
            return Json(new { success = false, message = _localization.GetString("AccessDenied") });
        }

        if (string.IsNullOrWhiteSpace(model.TaskName))
            return Json(new { success = false, message = _localization.GetString("TaskNameRequired") });

        if (model.DependentOnPhaseTaskId.HasValue && string.IsNullOrWhiteSpace(model.DependencyType))
            return Json(new { success = false, message = _localization.GetString("DependencyTypeRequired") });

        try
        {
            var taskId = await _myProjectsService.SaveProjectPhaseTaskAsync(model, GetCurrentUserId());
            return Json(new { success = true, taskId, message = _localization.GetString("RecordCreatedSuccessfully") });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhase([FromBody] ProjectPhaseSaveModel model)
    {
        var project = await _myProjectsService.GetApprovedProjectByIdAsync(model.ProjectId, GetCurrentUserId(), IsAdministrator());
        if (project == null)
            return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

        if (!CanManageProject(project))
            return Json(new { success = false, message = _localization.GetString("AccessDenied") });

        await _myProjectsService.DeleteProjectPhaseAsync(model.ProjectId, model.PhaseId, GetCurrentUserId());
        return Json(new { success = true, message = _localization.GetString("RecordDeletedSuccessfully") });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhaseActivity([FromBody] ProjectPhaseActivitySaveModel model)
    {
        var project = await _myProjectsService.GetApprovedProjectByIdAsync(model.ProjectId, GetCurrentUserId(), IsAdministrator());
        if (project == null)
            return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

        if (!CanManageProject(project))
            return Json(new { success = false, message = _localization.GetString("AccessDenied") });

        await _myProjectsService.DeleteProjectPhaseActivityAsync(model.ProjectId, model.PhaseActivityId, GetCurrentUserId());
        return Json(new { success = true, message = _localization.GetString("RecordDeletedSuccessfully") });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhaseTask([FromBody] ProjectPhaseTaskSaveModel model)
    {
        var project = await _myProjectsService.GetApprovedProjectByIdAsync(model.ProjectId, GetCurrentUserId(), IsAdministrator());
        if (project == null)
            return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

        if (!CanDeleteTask(project))
            return Json(new { success = false, message = _localization.GetString("AccessDenied") });

        await _myProjectsService.DeleteProjectPhaseTaskAsync(model.ProjectId, model.PhaseTaskId, GetCurrentUserId());
        return Json(new { success = true, message = _localization.GetString("RecordDeletedSuccessfully") });
    }

    private async Task<ManageMyProjectViewModel> BuildManageProjectViewModelAsync(MyProjectsApprovedItem project)
    {
        var projectId = project.ProjectId;
        var phases = await _myProjectsService.GetProjectPhasesAsync(projectId);
        var phaseActivities = await _myProjectsService.GetProjectPhaseActivitiesAsync(projectId);
        var phaseTasks = await _myProjectsService.GetProjectPhaseTasksAsync(projectId);

        foreach (var phase in phases)
        {
            phase.Activities = phaseActivities.Where(x => x.PhaseId == phase.PhaseId).ToList();
            foreach (var activity in phase.Activities)
            {
                activity.Tasks = phaseTasks.Where(x => x.PhaseActivityId == activity.PhaseActivityId).ToList();
            }
        }

        return new ManageMyProjectViewModel
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            ProjectTypeText = project.ProjectTypeText,
            DepartmentName = project.DepartmentName,
            ResponsibleUserName = project.ResponsibleUserName,
            CanManagePhases = CanManageProject(project),
            Phases = phases,
            AvailableProjectActivities = await _myProjectsService.GetProjectActivityTemplatesAsync(projectId),
            AvailableTaskAssignees = await _myProjectsService.GetProjectTaskAssigneesAsync(projectId),
            AvailableTaskDependencyOptions = phaseTasks
                .Select(x => new ProjectTaskDependencyOption
                {
                    PhaseTaskId = x.PhaseTaskId,
                    TaskName = x.TaskName,
                    PhaseActivityId = x.PhaseActivityId,
                    ActivityName = phaseActivities.FirstOrDefault(a => a.PhaseActivityId == x.PhaseActivityId)?.ActivityName,
                    PhaseName = phases.FirstOrDefault(p => p.Activities.Any(a => a.PhaseActivityId == x.PhaseActivityId))?.PhaseName
                })
                .ToList()
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProject([FromBody] ProjectSaveModel model)
    {
        if (!IsAdministrator())
            return Json(new { success = false, message = _localization.GetString("AccessDenied") });

        if (string.IsNullOrWhiteSpace(model.ProjectName))
            return Json(new { success = false, message = _localization.GetString("ProjectNameRequired") });

        try
        {
            var projectId = await _myProjectsService.SaveProjectAsync(model, GetCurrentUserId());
            return Json(new { success = true, projectId, message = _localization.GetString("RecordCreatedSuccessfully") });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProject([FromBody] ProjectSaveModel model)
    {
        if (!IsAdministrator())
            return Json(new { success = false, message = _localization.GetString("AccessDenied") });

        if (!model.ProjectId.HasValue || model.ProjectId.Value == Guid.Empty)
            return Json(new { success = false, message = _localization.GetString("RecordNotFound") });

        try
        {
            await _myProjectsService.DeleteProjectAsync(model.ProjectId.Value, GetCurrentUserId());
            return Json(new { success = true, message = _localization.GetString("RecordDeletedSuccessfully") });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private bool CanManageProject(MyProjectsApprovedItem project)
        => IsAdministrator() || (project.ResponsibleUserId.HasValue && project.ResponsibleUserId.Value == GetCurrentUserId());

    private bool CanEditTask(MyProjectsApprovedItem project, ProjectPhaseTaskItem task)
        => IsAdministrator()
           || (project.ResponsibleUserId.HasValue && project.ResponsibleUserId.Value == GetCurrentUserId())
           || task.Assignees.Any(x => x.EmployeeId == GetCurrentUserId());

    private bool CanDeleteTask(MyProjectsApprovedItem project)
        => IsAdministrator() || (project.ResponsibleUserId.HasValue && project.ResponsibleUserId.Value == GetCurrentUserId());
}
