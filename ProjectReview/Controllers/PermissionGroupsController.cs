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
    public class PermissionGroupsController : Controller
    {
        private readonly DataContext _context;

        public PermissionGroupsController(DataContext context)
        {
            _context = context;
        }

        // GET: PermissionGroups
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.PermissionGroups.Include(p => p.CreateUser);
            return View(await dataContext.ToListAsync());
        }

        // GET: PermissionGroups/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.PermissionGroups == null)
            {
                return NotFound();
            }

            var permissionGroup = await _context.PermissionGroups
                .Include(p => p.CreateUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permissionGroup == null)
            {
                return NotFound();
            }

            return View(permissionGroup);
        }

        // GET: PermissionGroups/Create
        public IActionResult Create()
        {
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: PermissionGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Status,CreateDate,CreateUserId")] PermissionGroup permissionGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permissionGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", permissionGroup.CreateUserId);
            return View(permissionGroup);
        }

        // GET: PermissionGroups/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.PermissionGroups == null)
            {
                return NotFound();
            }

            var permissionGroup = await _context.PermissionGroups.FindAsync(id);
            if (permissionGroup == null)
            {
                return NotFound();
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", permissionGroup.CreateUserId);
            return View(permissionGroup);
        }

        // POST: PermissionGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Status,CreateDate,CreateUserId")] PermissionGroup permissionGroup)
        {
            if (id != permissionGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permissionGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermissionGroupExists(permissionGroup.Id))
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
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", permissionGroup.CreateUserId);
            return View(permissionGroup);
        }

        // GET: PermissionGroups/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.PermissionGroups == null)
            {
                return NotFound();
            }

            var permissionGroup = await _context.PermissionGroups
                .Include(p => p.CreateUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (permissionGroup == null)
            {
                return NotFound();
            }

            return View(permissionGroup);
        }

        // POST: PermissionGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.PermissionGroups == null)
            {
                return Problem("Entity set 'DataContext.PermissionGroups'  is null.");
            }
            var permissionGroup = await _context.PermissionGroups.FindAsync(id);
            if (permissionGroup != null)
            {
                _context.PermissionGroups.Remove(permissionGroup);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermissionGroupExists(long id)
        {
          return (_context.PermissionGroups?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
