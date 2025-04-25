using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using LabProject5.Models;

namespace LabProject5.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string InputUsername { get; set; } = "";

        [BindProperty]
        public string InputPassword { get; set; } = "";

        public string ErrorMessage { get; set; } = "";

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
            if (!System.IO.File.Exists(jsonPath))
            {
                ErrorMessage = "User data file not found.";
                return Page();
            }

            var json = await System.IO.File.ReadAllTextAsync(jsonPath);
            var users = JsonSerializer.Deserialize<List<User>>(json);

            var matchedUser = users?.FirstOrDefault(u =>
                u.Username == InputUsername &&
                u.Password == InputPassword &&
                u.IsActive
            );

            if (matchedUser == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            // ✅ Session
            HttpContext.Session.SetString("username", matchedUser.Username);
            var token = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            // ✅ Cookie
            var options = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("username", matchedUser.Username, options);
            Response.Cookies.Append("token", token, options);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, options);

            // ✅ Başarılı giriş → yönlendir
            return RedirectToPage("/Index");
        }
    }
}
