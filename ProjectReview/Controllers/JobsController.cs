using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
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
            ViewData["InstructorId"] = new SelectList(await _jobService.GetHostUser(), "Id", "FullName");
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
                ViewData["InstructorId"] = new SelectList(await _jobService.GetHostUser(), "Id", "FullName");
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

        private bool JobExists(long id)
        {
          return (_context.Jobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Active(long id)
        {
            int? page = HttpContext.Session.GetInt32("page");
            int? size = HttpContext.Session.GetInt32("pageSize");
            await _jobService.Active(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }

		public async Task<IActionResult> Delete(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _jobService.Delete(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}
	}
}
