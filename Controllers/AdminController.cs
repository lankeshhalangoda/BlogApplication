using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlogApplication.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Add User Form
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        // POST: Process Add User Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(string fullName, string email, string password, string confirmPassword, string role)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
            }

            if (password.Length < 6)
            {
                ModelState.AddModelError("", "Password must be at least 6 characters long.");
            }

            if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter))
            {
                ModelState.AddModelError("", "Password must contain both letters and digits.");
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with this email already exists.");
            }

            if (ModelState.IsValid)
            {
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
                    if (!await _roleManager.RoleExistsAsync(roleToAssign))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleToAssign));
                    }

                    await _userManager.AddToRoleAsync(user, roleToAssign);
                    TempData["SuccessMessage"] = "User added successfully!";
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }


        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // GET: Edit User Form
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList(); // Get all available roles

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = roles.FirstOrDefault() // Default role from user if it exists
            };

            return View(model);
        }

        // POST: Update User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.Email;

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (!string.IsNullOrEmpty(model.Role) && !currentRoles.Contains(model.Role))
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        // Delete User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { success = false, message = "Invalid user ID." });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest(new { success = false, message = "User not found." });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            var errorMessages = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { success = false, message = string.Join(", ", errorMessages) });
        }

    }
}
