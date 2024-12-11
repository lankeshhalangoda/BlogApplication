using BlogApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: Login Form
    [HttpGet]
    public IActionResult Login()
    {
        TempData.Clear();

        return View();
    }


    // POST: Process Login
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string role)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains(role))
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["SuccessMessage"] = "Login successful!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The selected role does not match the user's assigned role.");
                    TempData["ErrorMessage"] = "Role mismatch!";
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                TempData["ErrorMessage"] = "Invalid credentials!";
            }
        }

        return View();
    }

    // GET: Registration Form
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: Process Registration

    [HttpPost]
    public async Task<IActionResult> Register(string fullName, string email, string password, string confirmPassword, string role)
    {
        if (ModelState.IsValid)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                TempData["ErrorMessage"] = "Passwords do not match!";
                return View();
            }

            string roleToAssign = string.IsNullOrEmpty(role) ? "Editor" : role;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                Role = roleToAssign
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, roleToAssign);
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["SuccessMessage"] = "Registration successful! Welcome!";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View();
    }


    // Logout
    public async Task<IActionResult> Logout(bool? confirmed)
    {
        if (confirmed.HasValue && confirmed.Value)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        TempData["LogoutConfirmation"] = true;
        return RedirectToAction("Index", "Home");
    }

}
