using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject5.Models;
using LabProject5.Data; // DbContext için
using Microsoft.EntityFrameworkCore;

namespace LabProject5.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public LoginModel(SchoolDbContext context)
        {
            _context = context;
        }

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
            if (string.IsNullOrWhiteSpace(InputUsername) || string.IsNullOrWhiteSpace(InputPassword))
            {
                ErrorMessage = "Please enter username and password.";
                return Page();
            }

            var matchedUser = await _context.Login
                .FirstOrDefaultAsync(u =>
                    u.Username == InputUsername &&
                    u.Password == InputPassword);

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

            return RedirectToPage("/Index");
        }
    }
}
