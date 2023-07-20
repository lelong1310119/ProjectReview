using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Users;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Services;
using ProjectReview.Services.Users;
using System.Diagnostics;

namespace ProjectReview.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
		private readonly IGenericService _genericService;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IGenericService genericService)
        {
            _logger = logger;
            _userService = userService;
			_genericService = genericService;
        }

        public async Task<IActionResult> Index(int? filter)
        {
            if (HttpContext.Session.GetString("username") is null)
            {
                return RedirectToAction("Index","Login");
            }
            else
            {
				int check = (filter) ?? 1;
				DateTime date = DateTime.Now;	
				ListJobDTO listJobDTO = await _genericService.ListJob(date);
				ViewData["Birthday"] = await _genericService.GetListByBirthday(date);
				ViewData["Profile"] = await _genericService.QuantityProfile();
				ViewData["DocumentSent"] = await _genericService.QuantityDocumentSent(date);
				ViewData["DocumentReceived"] = await _genericService.QuantityDocumentReceived(date);
				ViewData["Year"] = date.Year;
				ViewData["Notification"] = listJobDTO.Pending.Count + listJobDTO.Processing.Count;
				ViewData["filter"] = filter;
				ViewData["ListJob"] = await _genericService.GetList(date);
				if(check == 3)
				{
					return View(listJobDTO.Processed);
				}
				if(check == 1)
				{
					return View(listJobDTO.Pending);
				}
				if(check == 4)
				{
					return View(listJobDTO.OutOfDate);
				}
                return View(listJobDTO.Processing);
            }
        }

        public IActionResult Login()
        {
			if (HttpContext.Session.GetString("username") is null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Index");
			}
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Remove("username");
			return RedirectToAction("Login");
		}

		[HttpPost]
		public async Task<IActionResult> Login([FromForm] LoginDTO login)
		{
			try
			{
				var user = await _userService.Login(login.UserName, login.Password);
				HttpContext.Session.SetString("username", user.UserName);
				return RedirectToAction("Index");
			} catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(login);
			}
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}