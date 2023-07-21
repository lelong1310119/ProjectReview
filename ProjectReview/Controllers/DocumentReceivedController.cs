using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectReview.DTO.CategoryProfiles;
using ProjectReview.DTO.Departments;
using ProjectReview.DTO.Documents;
using ProjectReview.DTO.Users;
using ProjectReview.Paging;
using ProjectReview.Services.Documents;
using ProjectReview.Services.Jobs;

namespace ProjectReview.Controllers
{
	public class DocumentReceivedController : BaseController
	{
		private readonly IDocumentService _documentService;
		private readonly IJobService _jobService;

		public DocumentReceivedController(IDocumentService documentService, IJobService jobService)
		{
			_documentService = documentService;
			_jobService = jobService;
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

		public async Task<IActionResult> Recall(long id)
		{
			int? page = HttpContext.Session.GetInt32("page");
			int? size = HttpContext.Session.GetInt32("pageSize");
			await _documentService.Recall(id);
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
			ViewData["JobProfileId"] = new SelectList(await _documentService.GetListProfile(), "Id", "Name");
            return View();
        }

		public async Task<IActionResult> Assign(long id)
		{
			var result = await _documentService.GetById(id);
			if (result == null)
			{
				return NotFound();
			}
			ViewData["HostId"] = new SelectList(await _jobService.GetHostUser(), "Id", "FullName");
			ViewData["InstructorId"] = new SelectList(await _jobService.GetManager(), "Id", "FullName");
			ViewData["UserId"] = new SelectList(await _jobService.GetListUser(), "Id", "FullName");
			AssignDocumentDTO assignDocumentDTO = new AssignDocumentDTO { DocumentId = result.Id, Content = result.Content, FileName = result.FileName, FilePath = result.FilePath , Deadline = DateTime.Now};
			return View(assignDocumentDTO);
		}

		// POST: CategoryProfiles/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Assign([FromForm] AssignDocumentDTO assignDocumentDTO)
		{
			try
			{
				int? page = HttpContext.Session.GetInt32("page");
				int? size = HttpContext.Session.GetInt32("pageSize");
				await _documentService.Assign(assignDocumentDTO);
				return RedirectToAction(nameof(Index), new { page, size });
			}
			catch (Exception ex)
			{
				ViewData["HostId"] = new SelectList(await _jobService.GetHostUser(), "Id", "FullName");
				ViewData["InstructorId"] = new SelectList(await _jobService.GetManager(), "Id", "FullName");
				ViewData["UserId"] = new SelectList(await _jobService.GetListUser(), "Id", "FullName");
                ViewData["JobProfileId"] = new SelectList(await _documentService.GetListProfile(), "Id", "Name");

                ModelState.AddModelError("", ex.Message);
				return View(assignDocumentDTO);
			}
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
                int? page = HttpContext.Session.GetInt32("page");
                int? size = HttpContext.Session.GetInt32("pageSize");
                await _documentService.CreateDocumentReceived(createDocumentDTO);
                return RedirectToAction(nameof(Index), new { page, size });
            }
            catch (Exception ex)
            {
                ViewData["DocumentTypeId"] = new SelectList(await _documentService.GetListDocument(), "Id", "Name");
                ViewData["DensityId"] = new SelectList(await _documentService.GetListDensity(), "Id", "Detail");
				ViewData["JobProfileId"] = new SelectList(await _documentService.GetListProfile(), "Id", "Name");
				ViewData["UrgencyId"] = new SelectList(await _documentService.GetListUrgency(), "Id", "Detail");
                ModelState.AddModelError("", ex.Message);
                return View(createDocumentDTO);
            }
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
			ViewData["DocumentTypeId"] = new SelectList(await _documentService.GetListDocument(), "Id", "Name");
			ViewData["DensityId"] = new SelectList(await _documentService.GetListDensity(), "Id", "Detail");
            ViewData["JobProfileId"] = new SelectList(await _documentService.GetListProfile(), "Id", "Name");
            ViewData["UrgencyId"] = new SelectList(await _documentService.GetListUrgency(), "Id", "Detail");
			var result = await _documentService.GetUpdate(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

		// POST: Departments/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([FromForm] UpdateDocumentDTO documentDTO)
		{
			try
			{
				int? page = HttpContext.Session.GetInt32("page");
				int? size = HttpContext.Session.GetInt32("pageSize");
				await _documentService.Update(documentDTO);
				return RedirectToAction(nameof(Index), new { page, size });
			}
			catch (Exception ex)
			{
                ViewData["DocumentTypeId"] = new SelectList(await _documentService.GetListDocument(), "Id", "Name");
                ViewData["DensityId"] = new SelectList(await _documentService.GetListDensity(), "Id", "Detail");
                ViewData["JobProfileId"] = new SelectList(await _documentService.GetListProfile(), "Id", "Name");
                ViewData["UrgencyId"] = new SelectList(await _documentService.GetListUrgency(), "Id", "Detail");
                ModelState.AddModelError("", ex.Message);
				return View(documentDTO);
			}
		}

        public async Task<IActionResult> AddProfile(long id)
        {
            ViewData["JobProfileId"] = new SelectList(await _documentService.GetListProfile(), "Id", "Name");
            var result = await _documentService.GetToMove(id);
            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProfile([FromForm] AddProfileDTO documentDTO)
        {
            try
            {
                int? page = HttpContext.Session.GetInt32("page");
                int? size = HttpContext.Session.GetInt32("pageSize");
                await _documentService.UpdateProfile(documentDTO);
                return RedirectToAction(nameof(Index), new { page, size });
            }
            catch (Exception ex)
            {
                ViewData["JobProfileId"] = new SelectList(await _documentService.GetListProfile(), "Id", "Name");
                ModelState.AddModelError("", ex.Message);
                return View(documentDTO);
            }
        }

		public async Task<IActionResult> Detail(long id)
		{
			var result = await _documentService.GetById(id);
			if (result == null)
			{
				return NotFound();
			}
			return View(result);
		}
	}
}
