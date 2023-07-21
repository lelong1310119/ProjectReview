using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectReview.DTO.CategoryProfiles;
using ProjectReview.DTO.Departments;
using ProjectReview.DTO.Documents;
using ProjectReview.DTO.Users;
using ProjectReview.Paging;
using ProjectReview.Services.Documents;
using ProjectReview.Services.Jobs;
using OfficeOpenXml;

namespace ProjectReview.Controllers
{
    public class DocumentSentController : BaseController
    {
        private readonly IDocumentService _documentService;
        private readonly IJobService _jobService;

        public DocumentSentController(IDocumentService documentService, IJobService jobService)
        {
            _documentService = documentService;
            _jobService = jobService;
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
            AssignDocumentDTO assignDocumentDTO = new AssignDocumentDTO { DocumentId = result.Id, Content = result.Content, FileName = result.FileName, FilePath = result.FilePath, Deadline = DateTime.Now };
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
                await _documentService.CreateDocumentSent(createDocumentDTO);
                return RedirectToAction(nameof(Index));
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

		public async Task<IActionResult> Print(long id)
		{
			var result = await _documentService.GetById(id);
			if (result == null)
			{
				return NotFound();
			}
			return View(result);
		}

		public async Task<IActionResult> Export()
		{
			var documents = await _documentService.GetAllDocumentSent();
			using (ExcelPackage p = new ExcelPackage())
			{
				p.Workbook.Properties.Author = "Long Lê Đăng";
				p.Workbook.Properties.Title = "Danh sách văn bản";
				p.Workbook.Worksheets.Add("Văn bản đi");
				ExcelWorksheet ws = p.Workbook.Worksheets[1];
				ws.Name = "Văn bản đi";

				string[] arrColumnHeader = {
												"Số đi",
												"Số ký hiệu",
												"Nội dung",
												"Tác giả",
												"Ngày phát hành",
												"Ngày đi",
												"Người ký",
												"Chức vụ",
												"Loại văn bản",
												"Độ mật",
												"Độ khẩn",
												"Số tờ",
												"Ngôn ngữ",
												"Ghi chú",
						};

				var countColHeader = arrColumnHeader.Count();

				ws.Cells[1, 1].Value = "Danh sách văn bản đi";
				ws.Cells[1, 1, 1, countColHeader].Merge = true;
				ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;

				int colIndex = 1;
				int rowIndex = 2;

				foreach (var item in arrColumnHeader)
				{
					var cell = ws.Cells[rowIndex, colIndex];
					cell.Value = item;
					colIndex++;
				}

				foreach (var item in documents)
				{
					colIndex = 1;
					rowIndex++;
					ws.Cells[rowIndex, colIndex++].Value = item.Number;
					ws.Cells[rowIndex, colIndex++].Value = item.Symbol;
					ws.Cells[rowIndex, colIndex++].Value = item.Content;
					ws.Cells[rowIndex, colIndex++].Value = item.Author;
					ws.Cells[rowIndex, colIndex++].Value = item.DateIssued.ToShortDateString();
					ws.Cells[rowIndex, colIndex++].Value = item.CreateDate.ToShortDateString();
					ws.Cells[rowIndex, colIndex++].Value = item.Signer;
					ws.Cells[rowIndex, colIndex++].Value = item.Position;
					ws.Cells[rowIndex, colIndex++].Value = item.DocumentType.Name;
					ws.Cells[rowIndex, colIndex++].Value = item.Density.Detail;
					ws.Cells[rowIndex, colIndex++].Value = item.Urgency.Detail;
					ws.Cells[rowIndex, colIndex++].Value = item.NumberPaper;
					ws.Cells[rowIndex, colIndex++].Value = item.Language;
					ws.Cells[rowIndex, colIndex++].Value = item.Note;
				}

				var memoryStream = new MemoryStream();
				p.SaveAs(memoryStream);
				memoryStream.Position = 0;
				var fileDownloadName = "DocumentSent.xlsx";
				return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileDownloadName);
			}
		}
	}
}
