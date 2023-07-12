using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.CategoryProfiles;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Services.CategoryProfiles;

namespace ProjectReview.Controllers
{
	public class CategoryProfilesController : BaseController
	{
		private readonly ICategoryProfileService _categoryProfileService;

		public CategoryProfilesController(ICategoryProfileService CategoryProfileService)
		{
			_categoryProfileService = CategoryProfileService;
		}

		// GET: CategoryProfiles
		public async Task<IActionResult> Index(int? page, int? size)
		{
			CategoryProfileFilter filter;
			byte[] filterBytes = HttpContext.Session.Get("categoryProfileFilter");
			if (filterBytes != null)
			{
				var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
				filter = System.Text.Json.JsonSerializer.Deserialize<CategoryProfileFilter>(filterJson);
			}
			else
			{
				filter = new CategoryProfileFilter();
			}
			int pageNumber = (page ?? 1);
			int pageSize = (size ?? 10);
			ViewData["page"] = pageNumber;
			ViewData["pageSize"] = pageSize;
			CustomPaging<CategoryProfileDTO> result = await _categoryProfileService.GetCustomPaging(filter.Title, pageNumber, pageSize);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", pageNumber);
			HttpContext.Session.SetInt32("pageSize", pageSize);
			return View(filter);
		}

		public IActionResult ClearSession()
		{
			HttpContext.Session.Remove("categoryProfileFilter");
			HttpContext.Session.Remove("page");
			HttpContext.Session.Remove("pageSize");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> Index([FromForm] CategoryProfileFilter filter)
		{
			var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
			HttpContext.Session.Set("categoryProfileFilter", filterBytes);
			ViewData["page"] = 1;
			ViewData["pageSize"] = 10;
			var result = await _categoryProfileService.GetCustomPaging(filter.Title, 1, 10);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", 1);
			HttpContext.Session.SetInt32("pageSize", 10);
			return View(filter);
		}


		// GET: CategoryProfiles/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: CategoryProfiles/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([FromForm] CreateCategoryProfileDTO createCategoryProfileDTO)
		{
			try
			{
				await _categoryProfileService.Create(createCategoryProfileDTO);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(createCategoryProfileDTO);
			}
		}

		// GET: CategoryProfiles/Edit/5
		public async Task<IActionResult> Edit(long id)
		{
			var result = await _categoryProfileService.GetById(id);
			if (result == null)
			{
				return NotFound();
			}
			return View(result);
		}

		// POST: CategoryProfiles/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([FromForm] UpdateCategoryProfileDTO categoryProfile)
		{
			try
			{
				int? page = HttpContext.Session.GetInt32("page");
				int? size = HttpContext.Session.GetInt32("pageSize");
				await _categoryProfileService.Update(categoryProfile);
				return RedirectToAction(nameof(Index), new { page, size });
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(categoryProfile);
			}
		}

		// GET: CategoryProfiles/Delete/5
		public async Task<IActionResult> Delete(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _categoryProfileService.Delete(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}

		public async Task<IActionResult> Active(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _categoryProfileService.Active(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}

        public async Task<IActionResult> Close(long id)
        {
            int? page = HttpContext.Session.GetInt32("page");
            int? size = HttpContext.Session.GetInt32("pageSize");
            await _categoryProfileService.Close(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }
    }
}
