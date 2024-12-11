using BlogApplication.Data;
using BlogApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApplication.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Create Blog Post - GET
        public IActionResult Create()
        {
            return View();
        }

        // Create Blog Post - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost model, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.UserId = _userManager.GetUserId(User);

            var user = await _userManager.GetUserAsync(User);
            model.AuthorName = user?.FullName;

            model.CreatedAt = DateTime.Now;

            if (image != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = Path.GetFileName(image.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                model.ImagePath = $"/uploads/{fileName}";
            }

            model.Status = "Draft";

            _context.BlogPosts.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("List");
        }


        // List Blog Posts
        public async Task<IActionResult> List()
        {
            string userId = _userManager.GetUserId(User);
            var posts = await _context.BlogPosts
                                        .Where(bp => bp.UserId == userId)
                                        .OrderByDescending(bp => bp.CreatedAt)
                                        .ToListAsync();
            return View(posts);
        }


        // View Blog Post
        public async Task<IActionResult> View(int id)
        {
            var post = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }


        // Upload Image (AJAX)
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Invalid file." });
            }

            try
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string relativePath = $"/uploads/{fileName}";
                return Json(new { success = true, imagePath = relativePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Edit Blog Post - GET
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
            if (post == null || post.Status != "Draft")
            {
                return NotFound();
            }
            return View(post);
        }

        // Edit Blog Post - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BlogPost model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var post = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == model.Id);
            if (post == null || post.Status != "Draft")
            {
                return NotFound();
            }

            post.Title = model.Title;
            post.Description = model.Description;
            post.ImagePath = model.ImagePath;

            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }

        // Delete Blog Post
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("List");
        }

        // Publish Blog Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            var post = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
            if (post == null || post.Status != "Draft")
            {
                return BadRequest("Post not found or already published/rejected");
            }

            post.Status = "Published";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Post published successfully!" });
        }

        // Reject Blog Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var post = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
            if (post == null || post.Status != "Draft")
            {
                return BadRequest("Post not found or already published/rejected");
            }

            post.Status = "Rejected";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Post rejected successfully!" });
        }

        // List Draft Blog Posts
        public async Task<IActionResult> Drafts()
        {
            var drafts = await _context.BlogPosts
                                        .Where(bp => bp.Status == "Draft")
                                        .OrderByDescending(bp => bp.CreatedAt)
                                        .ToListAsync();
            return View(drafts);
        }


        // List Published Blog Posts
        public async Task<IActionResult> Published()
        {
            var publishedPosts = await _context.BlogPosts
                                                .Where(bp => bp.Status == "Published")
                                                .OrderByDescending(bp => bp.CreatedAt)
                                                .ToListAsync();
            return View(publishedPosts);
        }


        // List Rejected Blog Posts
        public async Task<IActionResult> Rejected()
        {
            var rejectedPosts = await _context.BlogPosts
                                               .Where(bp => bp.Status == "Rejected")
                                               .OrderByDescending(bp => bp.CreatedAt)
                                               .ToListAsync();
            return View(rejectedPosts);
        }


        // Revert Blog Post to Draft
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevertToDraft(int id)
        {
            try
            {
                var post = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
                if (post == null)
                {
                    return BadRequest($"No post found with ID {id}.");
                }

                if (post.Status != "Published" && post.Status != "Rejected")
                {
                    return BadRequest("Post status must be Published or Rejected to revert to draft.");
                }

                post.Status = "Draft";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Post Reverted to Draft!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> PublishedFeed()
        {
            var publishedPosts = await _context.BlogPosts.Where(bp => bp.Status == "Published")
                                                         .OrderByDescending(bp => bp.CreatedAt)
                                                         .ToListAsync();
            return View(publishedPosts);
        }

    }


}
