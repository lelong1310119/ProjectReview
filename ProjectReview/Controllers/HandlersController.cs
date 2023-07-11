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
    public class HandlersController : BaseController
    {
        private readonly DataContext _context;

        public HandlersController(DataContext context)
        {
            _context = context;
        }

        // GET: Handlers
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Handlers.Include(h => h.Job).Include(h => h.User);
            return View(await dataContext.ToListAsync());
        }

        // GET: Handlers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Handlers == null)
            {
                return NotFound();
            }

            var handler = await _context.Handlers
                .Include(h => h.Job)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (handler == null)
            {
                return NotFound();
            }

            return View(handler);
        }

        // GET: Handlers/Create
        public IActionResult Create()
        {
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Content");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Handlers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,JobId")] Handler handler)
        {
            if (ModelState.IsValid)
            {
                _context.Add(handler);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Content", handler.JobId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", handler.UserId);
            return View(handler);
        }

        // GET: Handlers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Handlers == null)
            {
                return NotFound();
            }

            var handler = await _context.Handlers.FindAsync(id);
            if (handler == null)
            {
                return NotFound();
            }
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Content", handler.JobId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", handler.UserId);
            return View(handler);
        }

        // POST: Handlers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("UserId,JobId")] Handler handler)
        {
            if (id != handler.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(handler);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HandlerExists(handler.UserId))
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
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Content", handler.JobId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", handler.UserId);
            return View(handler);
        }

        // GET: Handlers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Handlers == null)
            {
                return NotFound();
            }

            var handler = await _context.Handlers
                .Include(h => h.Job)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (handler == null)
            {
                return NotFound();
            }

            return View(handler);
        }

        // POST: Handlers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Handlers == null)
            {
                return Problem("Entity set 'DataContext.Handlers'  is null.");
            }
            var handler = await _context.Handlers.FindAsync(id);
            if (handler != null)
            {
                _context.Handlers.Remove(handler);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HandlerExists(long id)
        {
          return (_context.Handlers?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
