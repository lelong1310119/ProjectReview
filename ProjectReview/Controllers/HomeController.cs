using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.Users;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Services.Users;
using System.Diagnostics;

namespace ProjectReview.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("username") is null)
            {
                return RedirectToAction("Index","Login");
            }
            else
            {
                return View();
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