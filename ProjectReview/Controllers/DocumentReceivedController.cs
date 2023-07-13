using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectReview.DTO.Documents;
using ProjectReview.DTO.Users;
using ProjectReview.Paging;
using ProjectReview.Services.Documents;

namespace ProjectReview.Controllers
{
	public class DocumentReceivedController : BaseController
	{
		private readonly IDocumentService _documentService;
		public DocumentReceivedController(IDocumentService documentService)
		{
			_documentService = documentService;
		}

		public async Task<IActionResult> Index(int? page, int? size)
		{
			DocumentFilter filter;
			byte[] filterBytes = HttpContext.Session.Get("documentReceivedFilter");
			if (filterBytes != null)
			{
				var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
				filter = System.Text.Json.JsonSerializer.Deserialize<DocumentFilter>(filterJson);
			}
			else
			{
				filter = new DocumentFilter();
			}
			int pageNumber = (page ?? 1);
			int pageSize = (size ?? 10);
			ViewData["page"] = pageNumber;
			ViewData["pageSize"] = pageSize;
			CustomPaging<DocumentDTO> result = await _documentService.GetListDocumentReceived(filter.Content, pageNumber, pageSize);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", pageNumber);
			HttpContext.Session.SetInt32("pageSize", pageSize);
			return View(filter);
		}

		public IActionResult ClearSession()
		{
			HttpContext.Session.Remove("documentReceivedFilter");
			HttpContext.Session.Remove("page");
			HttpContext.Session.Remove("pageSize");
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Delete(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _documentService.Delete(id);
			return RedirectToAction(nameof(Index), new { page, size });
		}

		[HttpPost]
		public async Task<ActionResult> Index([FromForm] DocumentFilter filter)
		{
			var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
			HttpContext.Session.Set("documentReceivedFilter", filterBytes);
			ViewData["page"] = 1;
			ViewData["pageSize"] = 10;
			var result = await _documentService.GetListDocumentReceived(filter.Content, 1, 10);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", 1);
			HttpContext.Session.SetInt32("pageSize", 10);
			return View(filter);
		}

        public async Task<IActionResult> Create()
        {
            ViewData["DocumentTypeId"] = new SelectList(await _documentService.GetListDocument(), "Id", "Name");
            ViewData["DensityId"] = new SelectList(await _documentService.GetListDensity(), "Id", "Detail");
            ViewData["UrgencyId"] = new SelectList(await _documentService.GetListUrgency(), "Id", "Detail");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateDocumentDTO createDocumentDTO)
        {
            try
            {
				await _documentService.Create(createDocumentDTO);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["DocumentTypeId"] = new SelectList(await _documentService.GetListDocument(), "Id", "Name");
                ViewData["DensityId"] = new SelectList(await _documentService.GetListDensity(), "Id", "Detail");
                ViewData["UrgencyId"] = new SelectList(await _documentService.GetListUrgency(), "Id", "Detail");
                ModelState.AddModelError("", ex.Message);
                return View(createDocumentDTO);
            }
        }
	}
}
