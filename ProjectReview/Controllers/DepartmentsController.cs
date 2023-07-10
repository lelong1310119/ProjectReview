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
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Services.Departments;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectReview.Controllers
{
    public class DepartmentsController : Controller
    {
		private readonly DataContext _context;
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(DataContext context, IDepartmentService departmentService)
        {
            _context = context;
            _departmentService = departmentService;
        }

        // GET: Departments
        public async Task<IActionResult> Index(int? page, int? size)
        {
            DepartmentFilter filter;
			byte[] filterBytes = HttpContext.Session.Get("departmentFilter");
			if (filterBytes != null)
			{
				var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
				filter = System.Text.Json.JsonSerializer.Deserialize<DepartmentFilter>(filterJson);
			}
			else
			{
				filter = new DepartmentFilter();
			}
			int pageNumber = (page ?? 1);
            int pageSize = (size ?? 10);
			ViewData["page"] = pageNumber;
			ViewData["pageSize"] = pageSize;
			CustomPaging<DepartmentDTO> result = await _departmentService.GetCustomPaging(filter.Name, pageNumber, pageSize);
            int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
            ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", pageNumber);
			HttpContext.Session.SetInt32("pageSize", pageSize);
			return View(filter);
		}

		public IActionResult ClearSession()
		{
			HttpContext.Session.Remove("departmentFilter");
			HttpContext.Session.Remove("page");
			HttpContext.Session.Remove("pageSize");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> Index([FromForm] DepartmentFilter filter)
		{
			var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
			HttpContext.Session.Set("departmentFilter", filterBytes);
			ViewData["page"] = 1;
            ViewData["pageSize"] = 10;
			var result = await _departmentService.GetCustomPaging(filter.Name, 1, 10);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", 1);
			HttpContext.Session.SetInt32("pageSize", 10);
			return View(filter);
		}

		// GET: Departments/Details/5
		public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Test(Test test)
        {
            if (ModelState.IsValid)
            {
                Department department = new Department();
                department.Name = test.Name;
                department.Address = test.Address;
                department.Phone = test.Phone;
                department.CreateDate = DateTime.Now;
		        Console.WriteLine("Department Name: " + department.Name);
		        Console.WriteLine("Department Description: " + department.Address);
		        Console.WriteLine("Department CreateDate: " + department.CreateDate);
		        await _context.Departments.AddAsync(department);
                await _context.SaveChangesAsync();
                return Ok(department);
            }
            else
            {
                return BadRequest();
            }
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateDepartmentDTO createDepartmentDTO)
        {
			try
			{
				await _departmentService.Create(createDepartmentDTO);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(createDepartmentDTO);
			}
		}

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var result = await _departmentService.GetById(id);
            if (result == null) {
                return NotFound();
            }   
            return View(result);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] UpdateDepartmentDTO department)
        {
            try
            {
                int? page = HttpContext.Session.GetInt32("page");
                int? size = HttpContext.Session.GetInt32("pageSize");
                await _departmentService.Update(department);
                return RedirectToAction(nameof(Index), new { page, size });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(department);
            }
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _departmentService.Delete(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }

        public async Task<IActionResult> Active(long id)
        {
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _departmentService.Active(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}
    }
}
