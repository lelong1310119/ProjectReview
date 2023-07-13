using Microsoft.AspNetCore.Mvc;
using ProjectReview.DTO.Documents;
using ProjectReview.Paging;
using ProjectReview.Services.Documents;

namespace ProjectReview.Controllers
{
    public class DocumentSentController : BaseController
    {
		private readonly IDocumentService _documentService;
		public DocumentSentController(IDocumentService documentService)
		{
			_documentService = documentService;
		}

		public async Task<IActionResult> Index(int? page, int? size)
		{
			DocumentFilter filter;
			byte[] filterBytes = HttpContext.Session.Get("documentSentFilter");
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
			CustomPaging<DocumentDTO> result = await _documentService.GetListDocumentSent(filter.Content, pageNumber, pageSize);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", pageNumber);
			HttpContext.Session.SetInt32("pageSize", pageSize);
			return View(filter);
		}

		public IActionResult ClearSession()
		{
			HttpContext.Session.Remove("documentSentFilter");
			HttpContext.Session.Remove("page");
			HttpContext.Session.Remove("pageSize");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> Index([FromForm] DocumentFilter filter)
		{
			var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
			HttpContext.Session.Set("documentSentFilter", filterBytes);
			ViewData["page"] = 1;
			ViewData["pageSize"] = 10;
			var result = await _documentService.GetListDocumentSent(filter.Content, 1, 10);
			int totalPage = result.TotalPage;
			ViewData["totalPage"] = totalPage;
			ViewData["items"] = result.Data;
			HttpContext.Session.SetInt32("page", 1);
			HttpContext.Session.SetInt32("pageSize", 10);
			return View(filter);
		}
	}
}
