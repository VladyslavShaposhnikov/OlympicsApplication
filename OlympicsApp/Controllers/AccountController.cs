using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OlympicsApp.Models;
namespace OlympicsApp.Controllers;
using Microsoft.AspNetCore.Mvc;
public class AccountController : Controller
{
    private readonly FileUserStore _userStore;
    public AccountController(FileUserStore userStore)
    {
        _userStore = userStore;
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError(string.Empty, "Email and Password are required.");
            return View();
        }
        if (_userStore.ValidateUser(email, password))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index", "Home"); 
        }
        // Если данные неверны
        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View();
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError(string.Empty, "Email and Password are required.");
            return View();
        }
        if (_userStore.AddUser(email, password))
        {
            TempData["Message"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }
        // Если пользователь уже существует
        ModelState.AddModelError(string.Empty, "User with this email already exists.");
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Login", "Account");
    }
    
}