using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Models;
using ProjectReview.Models.Entities;

namespace ProjectReview.Controllers
{
    public class UsersController : Controller
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Users.Include(u => u.CreateUser).Include(u => u.Department).Include(u => u.PermissionGroup).Include(u => u.Position).Include(u => u.Rank);
            return View(await dataContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.CreateUser)
                .Include(u => u.Department)
                .Include(u => u.PermissionGroup)
                .Include(u => u.Position)
                .Include(u => u.Rank)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["PermissionGroupId"] = new SelectList(_context.PermissionGroups, "Id", "Name");
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name");
            ViewData["RankId"] = new SelectList(_context.Ranks, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Birthday,Gender,PositionId,DepartmentId,PermissionGroupId,RankId,UserName,Password,Email,Note,CreateDate,CreateUserId")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", user.CreateUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", user.DepartmentId);
            ViewData["PermissionGroupId"] = new SelectList(_context.PermissionGroups, "Id", "Name", user.PermissionGroupId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name", user.PositionId);
            ViewData["RankId"] = new SelectList(_context.Ranks, "Id", "Name", user.RankId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", user.CreateUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", user.DepartmentId);
            ViewData["PermissionGroupId"] = new SelectList(_context.PermissionGroups, "Id", "Name", user.PermissionGroupId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name", user.PositionId);
            ViewData["RankId"] = new SelectList(_context.Ranks, "Id", "Name", user.RankId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FullName,Birthday,Gender,PositionId,DepartmentId,PermissionGroupId,RankId,UserName,Password,Email,Note,CreateDate,CreateUserId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", user.CreateUserId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", user.DepartmentId);
            ViewData["PermissionGroupId"] = new SelectList(_context.PermissionGroups, "Id", "Name", user.PermissionGroupId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "Id", "Name", user.PositionId);
            ViewData["RankId"] = new SelectList(_context.Ranks, "Id", "Name", user.RankId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.CreateUser)
                .Include(u => u.Department)
                .Include(u => u.PermissionGroup)
                .Include(u => u.Position)
                .Include(u => u.Rank)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DataContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(long id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
