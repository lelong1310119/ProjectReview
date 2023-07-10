using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ProjectReview.DTO.Positions;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Services.Positions;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectReview.Controllers
{
	public class PositionsController : Controller
	{
		private readonly DataContext _context;
		private readonly IPositionService _positionService;

		public PositionsController(DataContext context, IPositionService positionService)
		{
			_context = context;
			_positionService = positionService;
		}

		// GET: Positions
		public async Task<IActionResult> Index(int? page, int? size)
		{
			PositionFilter filter;
			byte[] filterBytes = HttpContext.Session.Get("positionFilter");
			if (filterBytes != null)
			{
				var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
				filter = System.Text.Json.JsonSerializer.Deserialize<PositionFilter>(filterJson);
			}
			else
			{
				filter = new PositionFilter();
			}
			int pageNumber = (page ?? 1);
			int pageSize = (size ?? 10);
			ViewData["page"] = pageNumber;
			ViewData["pageSize"] = pageSize;
			CustomPaging<PositionDTO> result = await _positionService.GetCustomPaging(filter.Name, pageNumber, pageSize);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", pageNumber);
			HttpContext.Session.SetInt32("pageSize", pageSize);
			return View(filter);
		}

		public IActionResult ClearSession()
		{
			HttpContext.Session.Remove("positionFilter");
			HttpContext.Session.Remove("page");
			HttpContext.Session.Remove("pageSize");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> Index([FromForm] PositionFilter filter)
		{
			var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
			HttpContext.Session.Set("positionFilter", filterBytes);
			ViewData["page"] = 1;
			ViewData["pageSize"] = 10;
			var result = await _positionService.GetCustomPaging(filter.Name, 1, 10);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", 1);
			HttpContext.Session.SetInt32("pageSize", 10);
			return View(filter);
		}

		// GET: Positions/Details/5
		public async Task<IActionResult> Details(long? id)
		{
			if (id == null || _context.Positions == null)
			{
				return NotFound();
			}

			var position = await _context.Positions
				.FirstOrDefaultAsync(m => m.Id == id);
			if (position == null)
			{
				return NotFound();
			}

			return View(position);
		}

		// GET: Positions/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Positions/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([FromForm] CreatePositionDTO createPositionDTO)
		{
			try
			{
				await _positionService.Create(createPositionDTO);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(createPositionDTO);
			}
		}

		// GET: Positions/Edit/5
		public async Task<IActionResult> Edit(long id)
		{
			var result = await _positionService.GetById(id);
			if (result == null)
			{
				return NotFound();
			}
			return View(result);
		}

		// POST: Positions/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([FromForm] UpdatePositionDTO position)
		{
			try
			{
				int? page = HttpContext.Session.GetInt32("page");
				int? size = HttpContext.Session.GetInt32("pageSize");
				await _positionService.Update(position);
				return RedirectToAction(nameof(Index), new { page, size });
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(position);
			}
		}

		// GET: Positions/Delete/5
		public async Task<IActionResult> Delete(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _positionService.Delete(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}

		public async Task<IActionResult> Active(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _positionService.Active(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}
	}
}
