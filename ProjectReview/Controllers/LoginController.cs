using Microsoft.AspNetCore.Mvc;
using ProjectReview.DTO.Users;
using ProjectReview.Services.Users;

namespace ProjectReview.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("username") is null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] LoginDTO login)
        {
            try
            {
                var user = await _userService.Login(login.UserName, login.Password);
                HttpContext.Session.SetString("username", user.UserName);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(login);
            }
        }
    }
}
