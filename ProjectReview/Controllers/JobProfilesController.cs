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
    public class JobProfilesController : BaseController
    {
        private readonly DataContext _context;

        public JobProfilesController(DataContext context)
        {
            _context = context;
        }

        // GET: JobProfiles
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.JobProfiles.Include(j => j.CreateUser).Include(j => j.Profile);
            return View(await dataContext.ToListAsync());
        }

        // GET: JobProfiles/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.JobProfiles == null)
            {
                return NotFound();
            }

            var jobProfile = await _context.JobProfiles
                .Include(j => j.CreateUser)
                .Include(j => j.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobProfile == null)
            {
                return NotFound();
            }

            return View(jobProfile);
        }

        // GET: JobProfiles/Create
        public IActionResult Create()
        {
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["ProfileId"] = new SelectList(_context.Profiles, "Id", "Expiry");
            return View();
        }

        // POST: JobProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProfileId,StartDate,EndDate,Condition,NumberPaper,Status,CreateDate,CreateUserId")] JobProfile jobProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", jobProfile.CreateUserId);
            ViewData["ProfileId"] = new SelectList(_context.Profiles, "Id", "Expiry", jobProfile.ProfileId);
            return View(jobProfile);
        }

        // GET: JobProfiles/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.JobProfiles == null)
            {
                return NotFound();
            }

            var jobProfile = await _context.JobProfiles.FindAsync(id);
            if (jobProfile == null)
            {
                return NotFound();
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", jobProfile.CreateUserId);
            ViewData["ProfileId"] = new SelectList(_context.Profiles, "Id", "Expiry", jobProfile.ProfileId);
            return View(jobProfile);
        }

        // POST: JobProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,ProfileId,StartDate,EndDate,Condition,NumberPaper,Status,CreateDate,CreateUserId")] JobProfile jobProfile)
        {
            if (id != jobProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobProfileExists(jobProfile.Id))
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
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", jobProfile.CreateUserId);
            ViewData["ProfileId"] = new SelectList(_context.Profiles, "Id", "Expiry", jobProfile.ProfileId);
            return View(jobProfile);
        }

        // GET: JobProfiles/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.JobProfiles == null)
            {
                return NotFound();
            }

            var jobProfile = await _context.JobProfiles
                .Include(j => j.CreateUser)
                .Include(j => j.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobProfile == null)
            {
                return NotFound();
            }

            return View(jobProfile);
        }

        // POST: JobProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.JobProfiles == null)
            {
                return Problem("Entity set 'DataContext.JobProfiles'  is null.");
            }
            var jobProfile = await _context.JobProfiles.FindAsync(id);
            if (jobProfile != null)
            {
                _context.JobProfiles.Remove(jobProfile);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobProfileExists(long id)
        {
          return (_context.JobProfiles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
