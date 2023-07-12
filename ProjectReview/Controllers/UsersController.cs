using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectReview.DTO.Positions;
using ProjectReview.DTO.Users;
using ProjectReview.Paging;
using ProjectReview.Services.Users;

namespace ProjectReview.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Users
        public async Task<IActionResult> Index(int? page, int? size)
        {
            UserFilter filter;
            byte[] filterBytes = HttpContext.Session.Get("userFilter");
            if (filterBytes != null)
            {
                var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
                filter = System.Text.Json.JsonSerializer.Deserialize<UserFilter>(filterJson);
            }
            else
            {
                filter = new UserFilter();
            }
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 10);
            ViewData["page"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            CustomPaging<UserDTO> result = await _userService.GetCustomPaging(filter.FullName, pageNumber, pageSize);
            int totalPage = result.TotalPage;
            ViewData["totalPage"] = totalPage;
            ViewData["items"] = result.Data;
            HttpContext.Session.SetInt32("page", pageNumber);
            HttpContext.Session.SetInt32("pageSize", pageSize);
            return View(filter);
        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Remove("userFilter");
            HttpContext.Session.Remove("page");
            HttpContext.Session.Remove("pageSize");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Index([FromForm] UserFilter filter)
        {
            var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
            HttpContext.Session.Set("userFilter", filterBytes);
            ViewData["page"] = 1;
            ViewData["pageSize"] = 10;
            var result = await _userService.GetCustomPaging(filter.FullName, 1, 10);
            int totalPage = result.TotalPage;
            ViewData["totalPage"] = totalPage;
            ViewData["items"] = result.Data;
            HttpContext.Session.SetInt32("page", 1);
            HttpContext.Session.SetInt32("pageSize", 10);
            return View(filter);
        }


        // GET: Users/Create
        public async Task<IActionResult> Create()
        {
            ViewData["PositionId"] = new SelectList(await _userService.GetPosition(), "Id", "Name");
            ViewData["PermissionGroupId"] = new SelectList(await _userService.GetPermissionGroup(), "Id", "Name");
            ViewData["RankId"] = new SelectList(await _userService.GetRank(), "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(await _userService.GetDepartment(), "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateUserDTO createUserDTO)
        {
            try
            {
                await _userService.Create(createUserDTO);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(createUserDTO);
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            ViewData["PositionId"] = new SelectList(await _userService.GetPosition(), "Id", "Name");
            ViewData["PermissionGroupId"] = new SelectList(await _userService.GetPermissionGroup(), "Id", "Name");
            ViewData["RankId"] = new SelectList(await _userService.GetRank(), "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(await _userService.GetDepartment(), "Id", "Name");
            var result = await _userService.GetById(id);
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
        public async Task<IActionResult> Edit([FromForm] UpdateUserDTO user)
        {
            try
            {
                int? page = HttpContext.Session.GetInt32("page");
                int? size = HttpContext.Session.GetInt32("pageSize");
                await _userService.Update(user);
                return RedirectToAction(nameof(Index), new { page, size });
            }
            catch (Exception ex)
            {
                //ViewData["PositionId"] = new SelectList(await _userService.GetPosition(), "Id", "Name");
                //ViewData["PermissionGroupId"] = new SelectList(await _userService.GetPermissionGroup(), "Id", "Name");
                //ViewData["RankId"] = new SelectList(await _userService.GetRank(), "Id", "Name");
                //ViewData["DepartmentId"] = new SelectList(await _userService.GetDepartment(), "Id", "Name");
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            int? page = HttpContext.Session.GetInt32("page");
            int? size = HttpContext.Session.GetInt32("pageSize");
            await _userService.Delete(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }

        public async Task<IActionResult> Active(long id)
        {
            int? page = HttpContext.Session.GetInt32("page");
            int? size = HttpContext.Session.GetInt32("pageSize");
            await _userService.Active(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }
    }
}
