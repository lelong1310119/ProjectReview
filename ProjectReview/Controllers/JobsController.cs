using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.Histories;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Users;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Services.Jobs;

namespace ProjectReview.Controllers
{
    public class JobsController : BaseController
    {
        private readonly IJobService _jobService;
        private readonly DataContext _context;
        private readonly ICurrentUser _currentUser;

        public JobsController(DataContext context, IJobService jobService, ICurrentUser currentUser)
        {
            _context = context;
            _jobService = jobService;
            _currentUser = currentUser;
        }

		// GET: Jobs
		public async Task<IActionResult> Index(int? page, int? size)
		{
			JobFilter filter;
			byte[] filterBytes = HttpContext.Session.Get("jobFilter");
			if (filterBytes != null)
			{
				var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
				filter = System.Text.Json.JsonSerializer.Deserialize<JobFilter>(filterJson);
			}
			else
			{
				filter = new JobFilter();
			}
			int pageNumber = (page ?? 1);
			int pageSize = (size ?? 10);
			ViewData["page"] = pageNumber;
			ViewData["pageSize"] = pageSize;
            ViewData["Id"] = _currentUser.UserId;
			CustomPaging<JobDTO> result = await _jobService.GetListJob(filter.Content, pageNumber, pageSize);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", pageNumber);
			HttpContext.Session.SetInt32("pageSize", pageSize);
			return View(filter);
		}

		public IActionResult ClearSession()
		{
			HttpContext.Session.Remove("jobFilter");
			HttpContext.Session.Remove("page");
			HttpContext.Session.Remove("pageSize");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> Index([FromForm] JobFilter filter)
		{
			var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
			HttpContext.Session.Set("jobFilter", filterBytes);
			ViewData["page"] = 1;
			ViewData["pageSize"] = 10;
			ViewData["Id"] = _currentUser.UserId;
			var result = await _jobService.GetListJob(filter.Content, 1, 10);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", 1);
			HttpContext.Session.SetInt32("pageSize", 10);
			return View(filter);
		}

        public async Task<IActionResult> Create()
        {
            ViewData["HostId"] = new SelectList( await _jobService.GetHostUser(), "Id", "FullName");
            ViewData["InstructorId"] = new SelectList(await _jobService.GetManager(), "Id", "FullName");
            ViewData["UserId"] =  new SelectList(await _jobService.GetListUser(), "Id", "FullName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateJobDTO createJobDTO)
        {
            try
            {
                await _jobService.Create(createJobDTO);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["HostId"] = new SelectList(await _jobService.GetHostUser(), "Id", "FullName");
                ViewData["InstructorId"] = new SelectList(await _jobService.GetManager(), "Id", "FullName");
                ViewData["UserId"] = new SelectList(await _jobService.GetListUser(), "Id", "FullName");
                ModelState.AddModelError("", ex.Message);
                return View(createJobDTO);
            }
        }

        public async Task<IActionResult> Edit(long id)
        {
            ViewData["HostId"] = new SelectList(await _jobService.GetHostUser(), "Id", "FullName");
            ViewData["InstructorId"] = new SelectList(await _jobService.GetHostUser(), "Id", "FullName");
            ViewData["UserId"] = new SelectList(await _jobService.GetListUser(), "Id", "FullName");
            var result = await _jobService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] UpdateJobDTO job)
        {
            try
            {
                int? page = HttpContext.Session.GetInt32("page");
                int? size = HttpContext.Session.GetInt32("pageSize");
                await _jobService.Update(job);
                return RedirectToAction(nameof(Index), new { page, size });
            }
            catch (Exception ex)
            {
                ViewData["HostId"] = new SelectList(await _jobService.GetHostUser(), "Id", "FullName");
                ViewData["InstructorId"] = new SelectList(await _jobService.GetHostUser(), "Id", "FullName");
                ViewData["UserId"] = new SelectList(await _jobService.GetListUser(), "Id", "FullName");
                ModelState.AddModelError("", ex.Message);
                return View(job);
            }
        }

		public async Task<IActionResult> Detail(long id)
		{
			ViewData["Id"] = _currentUser.UserId;
			var result = await _jobService.GetJob(id);
			ViewData["Job"] = result;
			CreateHistoryDTO create = new CreateHistoryDTO { ProcessId = result.Process.Id, JobId = id}; 
			return View(create);
		}

		// POST: Users/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Detail([FromForm] CreateHistoryDTO create)
		{
			try
			{
				await _jobService.AddHistory(create);
				ViewData["Id"] = _currentUser.UserId;
				ViewData["Job"] = await _jobService.GetJob(create.JobId);
				return RedirectToAction(nameof(Detail));
			}
			catch (Exception ex)
			{
				ViewData["Id"] = _currentUser.UserId;
				ViewData["Job"] = await _jobService.GetJob(create.JobId);
				ModelState.AddModelError("", ex.Message);
				return View(create);
			}
		}

		public async Task<IActionResult> Active(long id)
        {
            int? page = HttpContext.Session.GetInt32("page");
            int? size = HttpContext.Session.GetInt32("pageSize");
            await _jobService.Active(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }

		public async Task<IActionResult> Finish(long id)
		{
			await _jobService.Finish(id);
			ViewData["Id"] = _currentUser.UserId;
			ViewData["Job"] = await _jobService.GetJob(id);
			return RedirectToAction(nameof(Detail), new {id});
		}

		public async Task<IActionResult> Open(long id)
		{
			await _jobService.Open(id);
			ViewData["Id"] = _currentUser.UserId;
			ViewData["Job"] = await _jobService.GetJob(id);
			return RedirectToAction(nameof(Detail), new { id });
		}

		public async Task<IActionResult> CancleAssign(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _jobService.CancleAssign(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}

		public async Task<IActionResult> Delete(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _jobService.Delete(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}

        public async Task<IActionResult> Forward(long id)
        {
            ViewData["InstructorId"] = new SelectList(await _jobService.GetMangerForward(id), "Id", "FullName");
            ViewData["UserId"] = new SelectList(await _jobService.GetListForward(id), "Id", "FullName");
            var result = await _jobService.GetForward(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forward([FromForm] ForwardDTO forward)
        {
            try
            {
                int? page = HttpContext.Session.GetInt32("page");
                int? size = HttpContext.Session.GetInt32("pageSize");
                await _jobService.Forward(forward);
                return RedirectToAction(nameof(Index), new { page, size });
            }
            catch (Exception ex)
            {
                ViewData["InstructorId"] = new SelectList(await _jobService.GetMangerForward(forward.Id), "Id", "FullName");
                ViewData["UserId"] = new SelectList(await _jobService.GetListForward(forward.Id), "Id", "FullName");
                ModelState.AddModelError("", ex.Message);
                return View(forward);
            }
        }
    }
}
