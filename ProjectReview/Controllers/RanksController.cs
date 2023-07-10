using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ProjectReview.DTO.Ranks;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Services.Ranks;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectReview.Controllers
{
    public class RanksController : Controller
    {
        private readonly DataContext _context;
        private readonly IRankService _rankService;

        public RanksController(DataContext context, IRankService RankService)
        {
            _context = context;
            _rankService = RankService;
        }

        // GET: Ranks
        public async Task<IActionResult> Index(int? page, int? size)
        {
            RankFilter filter;
            byte[] filterBytes = HttpContext.Session.Get("rankFilter");
            if (filterBytes != null)
            {
                var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
                filter = System.Text.Json.JsonSerializer.Deserialize<RankFilter>(filterJson);
            }
            else
            {
                filter = new RankFilter();
            }
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 10);
            ViewData["page"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            CustomPaging<RankDTO> result = await _rankService.GetCustomPaging(filter.Name, pageNumber, pageSize);
            int totalPage = result.TotalPage;
            ViewData["totalPage"] = totalPage;
            ViewData["items"] = result.Data;
            HttpContext.Session.SetInt32("page", pageNumber);
            HttpContext.Session.SetInt32("pageSize", pageSize);
            return View(filter);
        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Remove("rankFilter");
            HttpContext.Session.Remove("page");
            HttpContext.Session.Remove("pageSize");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Index([FromForm] RankFilter filter)
        {
            var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
            HttpContext.Session.Set("rankFilter", filterBytes);
            ViewData["page"] = 1;
            ViewData["pageSize"] = 10;
            var result = await _rankService.GetCustomPaging(filter.Name, 1, 10);
            int totalPage = result.TotalPage;
            ViewData["totalPage"] = totalPage;
            ViewData["items"] = result.Data;
            HttpContext.Session.SetInt32("page", 1);
            HttpContext.Session.SetInt32("pageSize", 10);
            return View(filter);
        }

        // GET: Ranks/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Ranks == null)
            {
                return NotFound();
            }

            var Rank = await _context.Ranks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Rank == null)
            {
                return NotFound();
            }

            return View(Rank);
        }

        // GET: Ranks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ranks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateRankDTO createRankDTO)
        {
            try
            {
                await _rankService.Create(createRankDTO);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(createRankDTO);
            }
        }

        // GET: Ranks/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var result = await _rankService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        // POST: Ranks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] UpdateRankDTO rank)
        {
            try
            {
                int? page = HttpContext.Session.GetInt32("page");
                int? size = HttpContext.Session.GetInt32("pageSize");
                await _rankService.Update(rank);
                return RedirectToAction(nameof(Index), new { page, size });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(rank);
            }
        }

        // GET: Ranks/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            int? page = HttpContext.Session.GetInt32("page");
            int? size = HttpContext.Session.GetInt32("pageSize");
            await _rankService.Delete(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }

        public async Task<IActionResult> Active(long id)
        {
            int? page = HttpContext.Session.GetInt32("page");
            int? size = HttpContext.Session.GetInt32("pageSize");
            await _rankService.Active(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }
    }
}
