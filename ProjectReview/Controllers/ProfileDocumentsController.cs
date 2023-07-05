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
    public class ProfileDocumentsController : Controller
    {
        private readonly DataContext _context;

        public ProfileDocumentsController(DataContext context)
        {
            _context = context;
        }

        // GET: ProfileDocuments
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.ProfileDocuments.Include(p => p.Document).Include(p => p.JobProfile);
            return View(await dataContext.ToListAsync());
        }

        // GET: ProfileDocuments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.ProfileDocuments == null)
            {
                return NotFound();
            }

            var profileDocument = await _context.ProfileDocuments
                .Include(p => p.Document)
                .Include(p => p.JobProfile)
                .FirstOrDefaultAsync(m => m.JobProfileId == id);
            if (profileDocument == null)
            {
                return NotFound();
            }

            return View(profileDocument);
        }

        // GET: ProfileDocuments/Create
        public IActionResult Create()
        {
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Author");
            ViewData["JobProfileId"] = new SelectList(_context.JobProfiles, "Id", "Condition");
            return View();
        }

        // POST: ProfileDocuments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobProfileId,DocumentId")] ProfileDocument profileDocument)
        {
            if (ModelState.IsValid)
            {
                _context.Add(profileDocument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Author", profileDocument.DocumentId);
            ViewData["JobProfileId"] = new SelectList(_context.JobProfiles, "Id", "Condition", profileDocument.JobProfileId);
            return View(profileDocument);
        }

        // GET: ProfileDocuments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.ProfileDocuments == null)
            {
                return NotFound();
            }

            var profileDocument = await _context.ProfileDocuments.FindAsync(id);
            if (profileDocument == null)
            {
                return NotFound();
            }
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Author", profileDocument.DocumentId);
            ViewData["JobProfileId"] = new SelectList(_context.JobProfiles, "Id", "Condition", profileDocument.JobProfileId);
            return View(profileDocument);
        }

        // POST: ProfileDocuments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("JobProfileId,DocumentId")] ProfileDocument profileDocument)
        {
            if (id != profileDocument.JobProfileId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profileDocument);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileDocumentExists(profileDocument.JobProfileId))
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
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Author", profileDocument.DocumentId);
            ViewData["JobProfileId"] = new SelectList(_context.JobProfiles, "Id", "Condition", profileDocument.JobProfileId);
            return View(profileDocument);
        }

        // GET: ProfileDocuments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.ProfileDocuments == null)
            {
                return NotFound();
            }

            var profileDocument = await _context.ProfileDocuments
                .Include(p => p.Document)
                .Include(p => p.JobProfile)
                .FirstOrDefaultAsync(m => m.JobProfileId == id);
            if (profileDocument == null)
            {
                return NotFound();
            }

            return View(profileDocument);
        }

        // POST: ProfileDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.ProfileDocuments == null)
            {
                return Problem("Entity set 'DataContext.ProfileDocuments'  is null.");
            }
            var profileDocument = await _context.ProfileDocuments.FindAsync(id);
            if (profileDocument != null)
            {
                _context.ProfileDocuments.Remove(profileDocument);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfileDocumentExists(long id)
        {
          return (_context.ProfileDocuments?.Any(e => e.JobProfileId == id)).GetValueOrDefault();
        }
    }
}
