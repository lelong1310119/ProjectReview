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
    public class DocumentsController : Controller
    {
        private readonly DataContext _context;

        public DocumentsController(DataContext context)
        {
            _context = context;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Documents.Include(d => d.CreateUser).Include(d => d.DocumentType);
            return View(await dataContext.ToListAsync());
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.CreateUser)
                .Include(d => d.DocumentType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentTypes, "Id", "Name");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreateDate,CreateUserId,Number,Author,Symbol,DateIssued,Content,DocumentTypeId,Receiver,FileName,Density,Urgency,NumberPaper,Language,Signer,Position,Note,IsAssign,Type")] Document document)
        {
            if (ModelState.IsValid)
            {
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", document.CreateUserId);
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentTypes, "Id", "Name", document.DocumentTypeId);
            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", document.CreateUserId);
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentTypes, "Id", "Name", document.DocumentTypeId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,CreateDate,CreateUserId,Number,Author,Symbol,DateIssued,Content,DocumentTypeId,Receiver,FileName,Density,Urgency,NumberPaper,Language,Signer,Position,Note,IsAssign,Type")] Document document)
        {
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
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
            ViewData["CreateUserId"] = new SelectList(_context.Users, "Id", "Email", document.CreateUserId);
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentTypes, "Id", "Name", document.DocumentTypeId);
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Documents == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.CreateUser)
                .Include(d => d.DocumentType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Documents == null)
            {
                return Problem("Entity set 'DataContext.Documents'  is null.");
            }
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(long id)
        {
          return (_context.Documents?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
