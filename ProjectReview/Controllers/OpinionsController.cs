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
    public class OpinionsController : BaseController
    {
        private readonly DataContext _context;

        public OpinionsController(DataContext context)
        {
            _context = context;
        }

        // GET: Opinions
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Opinions.Include(o => o.CreateUser).Include(o => o.Job);
            return View(await dataContext.ToListAsync());
        }

        // GET: Opinions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Opinions == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions
                .Include(o => o.CreateUser)
                .Include(o => o.Job)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
        }

        // GET: Opinions/Create
        public IActionResult Create()
        {
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Content");
            return View();
        }

        // POST: Opinions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,FileName,CreateDate,CreateUserId,JobId")] Opinion opinion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(opinion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", opinion.CreateUserId);
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Content", opinion.JobId);
            return View(opinion);
        }

        // GET: Opinions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Opinions == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions.FindAsync(id);
            if (opinion == null)
            {
                return NotFound();
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", opinion.CreateUserId);
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Content", opinion.JobId);
            return View(opinion);
        }

        // POST: Opinions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Content,FileName,CreateDate,CreateUserId,JobId")] Opinion opinion)
        {
            if (id != opinion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opinion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpinionExists(opinion.Id))
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
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", opinion.CreateUserId);
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Content", opinion.JobId);
            return View(opinion);
        }

        // GET: Opinions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Opinions == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions
                .Include(o => o.CreateUser)
                .Include(o => o.Job)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
        }

        // POST: Opinions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Opinions == null)
            {
                return Problem("Entity set 'DataContext.Opinions'  is null.");
            }
            var opinion = await _context.Opinions.FindAsync(id);
            if (opinion != null)
            {
                _context.Opinions.Remove(opinion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OpinionExists(long id)
        {
          return (_context.Opinions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
