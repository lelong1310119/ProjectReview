using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.DocumentTypes;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Services.DocumentTypes;

namespace ProjectReview.Controllers
{
    public class DocumentTypesController : BaseController
    {
        private readonly IDocumentTypeService _documentTypeService;

        public DocumentTypesController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

        // GET: DocumentTypes
        public async Task<IActionResult> Index(int? page, int? size)
        {
            DocumentTypeFilter filter;
            byte[] filterBytes = HttpContext.Session.Get("documentTypeFilter");
            if (filterBytes != null)
            {
                var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
                filter = System.Text.Json.JsonSerializer.Deserialize<DocumentTypeFilter>(filterJson);
            }
            else
            {
                filter = new DocumentTypeFilter();
            }
            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 10);
            ViewData["page"] = pageNumber;
            ViewData["pageSize"] = pageSize;
            CustomPaging<DocumentTypeDTO> result = await _documentTypeService.GetCustomPaging(filter.Name, pageNumber, pageSize);
            int totalPage = result.TotalPage;
            ViewData["totalPage"] = totalPage;
            ViewData["items"] = result.Data;
            HttpContext.Session.SetInt32("page", pageNumber);
            HttpContext.Session.SetInt32("pageSize", pageSize);
            return View(filter);
        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Remove("documentTypeFilter");
            HttpContext.Session.Remove("page");
            HttpContext.Session.Remove("pageSize");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Index([FromForm] DocumentTypeFilter filter)
        {
            var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
            HttpContext.Session.Set("documentTypeFilter", filterBytes);
            ViewData["page"] = 1;
            ViewData["pageSize"] = 10;
            var result = await _documentTypeService.GetCustomPaging(filter.Name, 1, 10);
            int totalPage = result.TotalPage;
            ViewData["totalPage"] = totalPage;
            ViewData["items"] = result.Data;
            HttpContext.Session.SetInt32("page", 1);
            HttpContext.Session.SetInt32("pageSize", 10);
            return View(filter);
        }


        // GET: DocumentTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DocumentTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateDocumentTypeDTO createDocumentTypeDTO)
        {
            try
            {
                await _documentTypeService.Create(createDocumentTypeDTO);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(createDocumentTypeDTO);
            }
        }

        // GET: DocumentTypes/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var result = await _documentTypeService.GetById(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        // POST: DocumentTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] UpdateDocumentTypeDTO DocumentType)
        {
            try
            {
                int? page = HttpContext.Session.GetInt32("page");
                int? size = HttpContext.Session.GetInt32("pageSize");
                await _documentTypeService.Update(DocumentType);
                return RedirectToAction(nameof(Index), new { page, size });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(DocumentType);
            }
        }

        // GET: DocumentTypes/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            int? page = HttpContext.Session.GetInt32("page");
            int? size = HttpContext.Session.GetInt32("pageSize");
            await _documentTypeService.Delete(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }

        public async Task<IActionResult> Active(long id)
        {
            int? page = HttpContext.Session.GetInt32("page");
            int? size = HttpContext.Session.GetInt32("pageSize");
            await _documentTypeService.Active(id);
            return RedirectToAction(nameof(Index), new { page, size });
        }
    }
}
