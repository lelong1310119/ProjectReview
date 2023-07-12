using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.JobProfiles;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Services.JobProfiles;

namespace ProjectReview.Controllers
{
	public class JobProfilesController : BaseController
	{
		private readonly IJobProfileService _jobProfileService;

		public JobProfilesController(IJobProfileService JobProfileService)
		{
			_jobProfileService = JobProfileService;
		}

		// GET: JobProfiles
		public async Task<IActionResult> Index(int? page, int? size)
		{
			JobProfileFilter filter;
			byte[] filterBytes = HttpContext.Session.Get("jobProfileFilter");
			if (filterBytes != null)
			{
				var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
				filter = System.Text.Json.JsonSerializer.Deserialize<JobProfileFilter>(filterJson);
			}
			else
			{
				filter = new JobProfileFilter();
			}
			int pageNumber = (page ?? 1);
			int pageSize = (size ?? 10);
			ViewData["page"] = pageNumber;
			ViewData["pageSize"] = pageSize;
			CustomPaging<JobProfileDTO> result = await _jobProfileService.GetCustomPaging(filter.Name, pageNumber, pageSize);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", pageNumber);
			HttpContext.Session.SetInt32("pageSize", pageSize);
			return View(filter);
		}

		public IActionResult ClearSession()
		{
			HttpContext.Session.Remove("jobProfileFilter");
			HttpContext.Session.Remove("page");
			HttpContext.Session.Remove("pageSize");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> Index([FromForm] JobProfileFilter filter)
		{
			var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
			HttpContext.Session.Set("jobProfileFilter", filterBytes);
			ViewData["page"] = 1;
			ViewData["pageSize"] = 10;
			var result = await _jobProfileService.GetCustomPaging(filter.Name, 1, 10);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", 1);
			HttpContext.Session.SetInt32("pageSize", 10);
			return View(filter);
		}


		// GET: JobProfiles/Create
		public async Task<IActionResult> Create()
		{
			ViewData["CategoryProfileId"] = new SelectList(await _jobProfileService.GetCategoryProfile(), "Id", "Title");
			return View();
		}

		// POST: JobProfiles/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([FromForm] CreateJobProfileDTO createJobProfileDTO)
		{
			try
			{
				await _jobProfileService.Create(createJobProfileDTO);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
                ViewData["CategoryProfileId"] = new SelectList(await _jobProfileService.GetCategoryProfile(), "Id", "Title");
                ModelState.AddModelError("", ex.Message);
				return View(createJobProfileDTO);
			}
		}

		// GET: JobProfiles/Edit/5
		public async Task<IActionResult> Edit(long id)
		{
			ViewData["CategoryProfileId"] = new SelectList(await _jobProfileService.GetCategoryProfile(), "Id", "Title");
			var result = await _jobProfileService.GetById(id);
			if (result == null)
			{
				return NotFound();
			}
			return View(result);
		}

		// POST: JobProfiles/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([FromForm] UpdateJobProfileDTO jobProfile)
		{
			try
			{
				int? page = HttpContext.Session.GetInt32("page");
				int? size = HttpContext.Session.GetInt32("pageSize");
				await _jobProfileService.Update(jobProfile);
				return RedirectToAction(nameof(Index), new { page, size });
			}
			catch (Exception ex)
			{
                ViewData["CategoryProfileId"] = new SelectList(await _jobProfileService.GetCategoryProfile(), "Id", "Title");
                ModelState.AddModelError("", ex.Message);
				return View(jobProfile);
			}
		}

		// GET: JobProfiles/Delete/5
		public async Task<IActionResult> Delete(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _jobProfileService.Delete(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}

		public async Task<IActionResult> Active(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _jobProfileService.Active(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}

		public async Task<IActionResult> Close(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _jobProfileService.Close(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}
	}
}
