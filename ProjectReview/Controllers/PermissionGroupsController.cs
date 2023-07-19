using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ProjectReview.DTO.Departments;
using ProjectReview.DTO.PermissionGroups;
using ProjectReview.Enums;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Services.PermissionGroups;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectReview.Controllers
{
	public class PermissionGroupsController : BaseController
	{
		private readonly IPermissionGroupService _permissionGroupService;

		public PermissionGroupsController(IPermissionGroupService PermissionGroupService)
		{
			_permissionGroupService = PermissionGroupService;
		}

		// GET: PermissionGroups
		public async Task<IActionResult> Index(int? page, int? size)
		{
			int pageNumber = (page ?? 1);
			int pageSize = (size ?? 10);
			ViewData["page"] = pageNumber;
			ViewData["pageSize"] = pageSize;
			CustomPaging<PermissionGroupDTO> result = await _permissionGroupService.GetCustomPaging(pageNumber, pageSize);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", pageNumber);
			HttpContext.Session.SetInt32("pageSize", pageSize);
			return View();
		}

		public IActionResult ClearSession()
		{
			HttpContext.Session.Remove("page");
			HttpContext.Session.Remove("pageSize");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> Index([FromForm] CreatePermissionGroupDTO createPermissionGroupDTO)
		{
			try
			{
				int? page = HttpContext.Session.GetInt32("page");
				int? size = HttpContext.Session.GetInt32("pageSize");
				var result = await _permissionGroupService.Create(createPermissionGroupDTO);
				return RedirectToAction(nameof(Index), new { page, size });
			} catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(createPermissionGroupDTO);
			}
		}


        public IActionResult Create()
        {
			ViewData["RoleId"] = new SelectList(RoleEnum.RoleEnumList, "Id", "Detail");
            return View();
        }


        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([FromForm] CreatePermissionGroupDTO createPermissionGroupDTO)
		{
			try
			{
				await _permissionGroupService.Create(createPermissionGroupDTO);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
                ViewData["RoleId"] = new SelectList(RoleEnum.RoleEnumList, "Id", "Detail");
                ModelState.AddModelError("", ex.Message);
				return View(createPermissionGroupDTO);
			}
		}

		// GET: PermissionGroups/Edit/5
		public async Task<IActionResult> Edit(long id)
		{
            ViewData["RoleId"] = new SelectList(RoleEnum.RoleEnumList, "Id", "Detail");
            var result = await _permissionGroupService.GetById(id);
			if (result == null)
			{
				return NotFound();
			}
            return View(result);
		}

		// POST: PermissionGroups/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([FromForm] UpdatePermissionGroupDTO permissionGroup)
		{
			try
			{
				int? page = HttpContext.Session.GetInt32("page");
				int? size = HttpContext.Session.GetInt32("pageSize");
				await _permissionGroupService.Update(permissionGroup);
				return RedirectToAction(nameof(Index), new { page, size });
			}
			catch (Exception ex)
			{
                ViewData["RoleId"] = new SelectList(RoleEnum.RoleEnumList, "Id", "Detail");
                ModelState.AddModelError("", ex.Message);
				return View(permissionGroup);
			}
		}

		// GET: PermissionGroups/Delete/5
		public async Task<IActionResult> Delete(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _permissionGroupService.Delete(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}

		public async Task<IActionResult> Active(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _permissionGroupService.Active(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}
	}
}
