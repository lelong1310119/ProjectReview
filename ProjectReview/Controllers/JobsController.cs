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
    public class JobsController : Controller
    {
        private readonly DataContext _context;

        public JobsController(DataContext context)
        {
            _context = context;
        }

        // GET: Jobs
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Jobs.Include(j => j.CreateUser).Include(j => j.Document).Include(j => j.Host).Include(j => j.Instructor);
            return View(await dataContext.ToListAsync());
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.CreateUser)
                .Include(j => j.Document)
                .Include(j => j.Host)
                .Include(j => j.Instructor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Author");
            ViewData["HostId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["InstructorId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HostId,InstructorId,CreateUserId,CreateDate,Deadline,Request,Status,FileName,Content,DocumentId")] Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", job.CreateUserId);
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Author", job.DocumentId);
            ViewData["HostId"] = new SelectList(_context.Users, "Id", "Email", job.HostId);
            ViewData["InstructorId"] = new SelectList(_context.Users, "Id", "Email", job.InstructorId);
            return View(job);
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", job.CreateUserId);
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Author", job.DocumentId);
            ViewData["HostId"] = new SelectList(_context.Users, "Id", "Email", job.HostId);
            ViewData["InstructorId"] = new SelectList(_context.Users, "Id", "Email", job.InstructorId);
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,HostId,InstructorId,CreateUserId,CreateDate,Deadline,Request,Status,FileName,Content,DocumentId")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", job.CreateUserId);
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Author", job.DocumentId);
            ViewData["HostId"] = new SelectList(_context.Users, "Id", "Email", job.HostId);
            ViewData["InstructorId"] = new SelectList(_context.Users, "Id", "Email", job.InstructorId);
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.CreateUser)
                .Include(j => j.Document)
                .Include(j => j.Host)
                .Include(j => j.Instructor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Jobs == null)
            {
                return Problem("Entity set 'DataContext.Jobs'  is null.");
            }
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(long id)
        {
          return (_context.Jobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
