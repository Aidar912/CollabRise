using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dsana.Data;
using Dsana.Extensions;
using Dsana.Models;
using Dsana.Models.Enums;
using Dsana.Models.ViewModels;
using Dsana.Services.Interfaces;

namespace Dsana.Controllers
{
    [Authorize]
    public class DTasksController : Controller
    {
        private readonly UserManager<DSUser> _userManager;
        private readonly IDSProjectService _projectService;
        private readonly IDSLookupService _lookupService;
        private readonly IDSDTaskService _dtaskService;
        private readonly IDSFileService _fileService;
        private readonly IDSDTaskHistoryService _historyService;
        private readonly IDSRolesService _rolesService;

        public DTasksController(UserManager<DSUser> userManager, IDSProjectService projectService, IDSLookupService lookupService, IDSDTaskService dtaskService, IDSFileService fileService, IDSDTaskHistoryService historyService, IDSRolesService rolesService)
        {
            _userManager = userManager;
            _projectService = projectService;
            _lookupService = lookupService;
            _dtaskService = dtaskService;
            _fileService = fileService;
            _historyService = historyService;
            _rolesService = rolesService;
        }

        public async Task<IActionResult> MyDTasks()
        {
            DSUser appUser = await _userManager.GetUserAsync(User);

            List<DTask> dtasks = await _dtaskService.GetDTasksByUserIdAsync(appUser.Id, appUser.CompanyID);

            return View(dtasks);
        }

        public async Task<IActionResult> AllDTasks()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<DTask> dtasks = await _dtaskService.GetAllDTasksByCompanyAsync(companyId);

            if (User.IsInRole(nameof(Roles.Developer)) || User.IsInRole(nameof(Roles.Submitter)))
            {
                return View(dtasks.Where(t => t.Archived == false));
            }
            else
            {
                return View(dtasks);
            }
        }

        public async Task<IActionResult> ArchivedDTasks()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            string appUserId = _userManager.GetUserId(User);

            List<DTask> dtasks = await _dtaskService.GetArchivedDTasksAsync(companyId);

            if(User.IsInRole(nameof(Roles.Admin)))
            {
                return View(dtasks);
            }
            else
            {
                List<DTask> pmDTasks = new();

                foreach(DTask dtask in dtasks)
                {
                    if (await _projectService.IsAssignedProjectManagerAsync(appUserId, dtask.ProjectId))
                    {
                        pmDTasks.Add(dtask);
                    }
                }

                return View(pmDTasks);
            }
        }

        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> UnassignedDTasks()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<DTask> dtasks = await _dtaskService.GetUnassignedDTasksAsync(companyId);

            return View(dtasks);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignDeveloper(int id)
        {
            AssignDeveloperViewModel model = new();

            int companyId = User.Identity.GetCompanyId().Value;

            model.DTask = await _dtaskService.GetDTaskByIdAsync(id);
            model.Developers = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.Developer.ToString(),companyId), "Id", "FullName");
            //model.Developers = new SelectList(await _projectService.GetProjectMembersByRoleAsync(model.DTask.ProjectId, nameof(Roles.Developer)), "Id", "FullName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            if(model.DeveloperId != null)
            {

                DSUser appUser = await _userManager.GetUserAsync(User);

                DTask oldDTask = await _dtaskService.GetDTasksAsNoTrackingAsync(model.DTask.Id);

                try
                {

                    await _dtaskService.AssignDTaskAsync(model.DTask.Id, model.DeveloperId);
                }
                catch (Exception ex)
                {
                    throw;
                }

                DTask newDTask = await _dtaskService.GetDTasksAsNoTrackingAsync(model.DTask.Id);

                await _historyService.AddHistoryAsync(oldDTask, newDTask, appUser.Id);

                return RedirectToAction(nameof(Details), new { id = model.DTask.Id });
            }

            return RedirectToAction(nameof(AssignDeveloper), new { id = model.DTask.Id });
        }

        // GET: DTasks/Details/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DTask dtask = await _dtaskService.GetDTaskByIdAsync(id.Value);



            if (dtask == null)
            {
                return NotFound();
            }

            return View(dtask);
        }

        // GET: DTasks/Create
        public async Task<IActionResult> Create()
        {
            DSUser appUser = await _userManager.GetUserAsync(User);

            int companyId = User.Identity.GetCompanyId().Value;

            if(User.IsInRole(nameof(Roles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId),"Id","Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(appUser.Id), "Id", "Name");
            }

            ViewData["DTaskPriorityId"] = new SelectList(await _lookupService.GetDTaskPrioritiesAsync(), "Id", "Name");
            ViewData["DTaskTypeId"] = new SelectList(await _lookupService.GetDTaskTypesAsync(), "Id", "Name");

            return View();
        }

        // POST: DTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,DTaskTypeId,DTaskPriorityId")] DTask dtask)
        {

            DSUser appUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                try
                {

                    dtask.Created = DateTimeOffset.Now;
                    dtask.OwnerUserId = appUser.Id;

                    dtask.DTaskStatusId = (await _dtaskService.LookupDTaskStatusIdAsync(nameof(DSDTaskStatus.New))).Value;

                    await _dtaskService.AddNewDTaskAsync(dtask);

                    DTask newDTask = await _dtaskService.GetDTasksAsNoTrackingAsync(dtask.Id);
                    await _historyService.AddHistoryAsync(null, newDTask, appUser.Id); //appUser.Id

                    

                }
                catch (Exception ex)
                {
                    throw;
                }
                return RedirectToAction(nameof(Details), new { id = dtask.Id });
            }

            //if (User.IsInRole(nameof(Roles.Admin)))
            //{
            //    ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(appUser.CompanyID), "Id", "Name");
            //}
            //else
            //{
            //    ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(appUser.Id), "Id", "Name");
            //}

            //ViewData["DTaskPriorityId"] = new SelectList(await _lookupService.GetDTaskPrioritiesAsync(), "Id", "Name");
            //ViewData["DTaskTypeId"] = new SelectList(await _lookupService.GetDTaskTypesAsync(), "Id", "Name");

            return View(nameof(Details), new {id = dtask.Id });
        }

        

        // GET: DTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DTask dtask = await _dtaskService.GetDTaskByIdAsync(id.Value);



            if (dtask == null)
            {
                return NotFound();
            }
            
            
            ViewData["DTaskPriorityId"] = new SelectList(await _lookupService.GetDTaskPrioritiesAsync(), "Id", "Name", dtask.DTaskPriorityId);
            ViewData["DTaskStatusId"] = new SelectList(await _lookupService.GetDTaskStatusesAsync(), "Id", "Name", dtask.DTaskStatusId);
            ViewData["DTaskTypeId"] = new SelectList(await _lookupService.GetDTaskTypesAsync(), "Id", "Name", dtask.DTaskTypeId);

            return View(dtask);
        }

        // POST: DTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ProjectId,DTaskTypeId,DTaskPriorityId,DTaskStatusId,OwnerUserId,DeveloperUserId")] DTask dtask)
        {
            if (id != dtask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            { 

                DSUser appUser = await _userManager.GetUserAsync(User);
                DTask oldDTask = await _dtaskService.GetDTasksAsNoTrackingAsync(dtask.Id);

                try
                {
                    dtask.Updated = DateTimeOffset.Now;
                    await _dtaskService.UpdateDTaskAsync(dtask);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DTaskExists(dtask.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                DTask newDTask = await _dtaskService.GetDTasksAsNoTrackingAsync(dtask.Id);
                await _historyService.AddHistoryAsync(oldDTask,newDTask,appUser.Id);

                return RedirectToAction(nameof(Details), new {id = dtask.Id });
            }

            ViewData["DTaskPriorityId"] = new SelectList(await _lookupService.GetDTaskPrioritiesAsync(), "Id", "Name", dtask.DTaskPriorityId);
            ViewData["DTaskStatusId"] = new SelectList(await _lookupService.GetDTaskStatusesAsync(), "Id", "Name", dtask.DTaskStatusId);
            ViewData["DTaskTypeId"] = new SelectList(await _lookupService.GetDTaskTypesAsync(), "Id", "Name", dtask.DTaskTypeId);

            return View(dtask);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDTaskAttachment([Bind("Id,FormFile,Description,DTaskId")] DTaskAttachment dtaskAttachment)
        {
            string statusMessage;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));

                // Breakpoint, Log or examine the list with Exceptions.
            }

            if (ModelState.IsValid && dtaskAttachment.FormFile != null)
            {
                try
                {

                    dtaskAttachment.FileData = await _fileService.ConvertFileToByteArrayAsync(dtaskAttachment.FormFile);
                    dtaskAttachment.FileName = dtaskAttachment.FormFile.FileName;
                    dtaskAttachment.FileContentType = dtaskAttachment.FormFile.ContentType;

                    dtaskAttachment.Created = DateTimeOffset.Now;
                    dtaskAttachment.UserId = _userManager.GetUserId(User);

                    await _dtaskService.AddDTaskAttachmentAsync(dtaskAttachment);

                    await _historyService.AddHistoryAsync(dtaskAttachment.DTaskId, nameof(DTaskAttachment), dtaskAttachment.UserId);
                }
                catch (Exception ex)
                {
                    throw;
                }

                statusMessage = "Success: New attachment added to Task.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";

            }

            return RedirectToAction("Details", new { id = dtaskAttachment.DTaskId, message = statusMessage });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDTaskComment([Bind("Id,DTaskId,Comment")] DTaskComment dtaskComment)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    dtaskComment.UserId = _userManager.GetUserId(User);
                    dtaskComment.Created = DateTimeOffset.Now;

                    await _dtaskService.AddDTaskCommentAsync(dtaskComment);

                    await _historyService.AddHistoryAsync(dtaskComment.DTaskId, nameof(DTaskComment), dtaskComment.UserId);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return RedirectToAction("Details", new { id = dtaskComment.DTaskId });
        }

        // GET: DTasks/Archive/
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var dtask = await _context.DTasks
            //    .Include(t => t.DeveloperUser)
            //    .Include(t => t.OwnerUser)
            //    .Include(t => t.Project)
            //    .Include(t => t.DTaskPriority)
            //    .Include(t => t.DTaskStatus)
            //    .Include(t => t.DTaskType)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            DTask dtask = await _dtaskService.GetDTaskByIdAsync(id.Value);




            if (dtask == null)
            {
                return NotFound();
            }

            return View(dtask);
        }

        // POST: DTasks/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            DTask dtask = await _dtaskService.GetDTaskByIdAsync(id);
            dtask.Archived = true;
            await _dtaskService.UpdateDTaskAsync(dtask);

            return RedirectToAction(nameof(Index));
        }


        // GET: DTasks/Archive/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var dtask = await _context.DTasks
            //    .Include(t => t.DeveloperUser)
            //    .Include(t => t.OwnerUser)
            //    .Include(t => t.Project)
            //    .Include(t => t.DTaskPriority)
            //    .Include(t => t.DTaskStatus)
            //    .Include(t => t.DTaskType)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            DTask dtask = await _dtaskService.GetDTaskByIdAsync(id.Value);




            if (dtask == null)
            {
                return NotFound();
            }

            return View(dtask);
        }

        // POST: DTasks/Delete/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            DTask dtask = await _dtaskService.GetDTaskByIdAsync(id);
            dtask.Archived = false;
            await _dtaskService.UpdateDTaskAsync(dtask);





            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ShowFile(int id)
        {
            DTaskAttachment dtaskAttachment = await _dtaskService.GetDTaskAttachmentByIdAsync(id);
            string fileName = dtaskAttachment.FileName;
            byte[] fileData = dtaskAttachment.FileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }

        private async Task<bool> DTaskExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _dtaskService.GetAllDTasksByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
    }
}
