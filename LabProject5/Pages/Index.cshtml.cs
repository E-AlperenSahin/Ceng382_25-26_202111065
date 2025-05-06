using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject5.Models;
using LabProject5.Helpers;
using LabProject5.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LabProject5.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SchoolDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, SchoolDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Class> FilteredClassList { get; set; } = new List<Class>();

        [BindProperty]
        public Class ClassInput { get; set; } = new();

        public bool IsEditMode { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Ai prompt: Giriş Kontrolü Nasıl Yapılır
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");
            var cookieUsername = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];
            var cookieSessionId = Request.Cookies["session_id"];

            if (string.IsNullOrEmpty(sessionUsername) ||
                string.IsNullOrEmpty(sessionToken) ||
                string.IsNullOrEmpty(sessionId) ||
                string.IsNullOrEmpty(cookieUsername) ||
                string.IsNullOrEmpty(cookieToken) ||
                string.IsNullOrEmpty(cookieSessionId) ||
                sessionUsername != cookieUsername ||
                sessionToken != cookieToken ||
                sessionId != cookieSessionId)
            {
                HttpContext.Session.Clear();
                Response.Cookies.Delete("username");
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("session_id");
                return RedirectToPage("/Login", new { error = "notauthorized" });
            }

            var query = _context.Classes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(c =>
                    c.Name.Contains(SearchKeyword) ||
                    c.Description.Contains(SearchKeyword));
            }

            int totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            FilteredClassList = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Classes.Add(ClassInput);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { PageNumber, SearchKeyword });
        }

        public async Task<IActionResult> OnPostEditSelectAsync(int id)
        {
            var existing = await _context.Classes.FindAsync(id);
            if (existing == null)
                return RedirectToPage();

            ClassInput = existing;
            IsEditMode = true;
            return await OnGetAsync();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var existing = await _context.Classes.FindAsync(ClassInput.Id);
            if (existing != null)
            {
                existing.Name = ClassInput.Name;
                existing.PersonCount = ClassInput.PersonCount;
                existing.Description = ClassInput.Description;
                existing.IsActive = ClassInput.IsActive;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { PageNumber, SearchKeyword });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item != null)
            {
                _context.Classes.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { PageNumber, SearchKeyword });
        }

        // Ai prompt: Kolonları Aktif Olarak Nasıl Kullanabilirim
        public async Task<IActionResult> OnPostExportJsonAsync(string SelectedColumns)
        {
            var columns = string.IsNullOrWhiteSpace(SelectedColumns)
                ? new List<string>()
                : SelectedColumns.Split(',').ToList();

            var query = _context.Classes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(c =>
                    c.Name.Contains(SearchKeyword) ||
                    c.Description.Contains(SearchKeyword));
            }

            var dataToExport = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            string json = Utils.Instance.ExportToJson(dataToExport, columns);
            var bytes = Encoding.UTF8.GetBytes(json);

            return File(bytes, "application/json", $"current-page-p{PageNumber}.json");
        }

        // Ai prompt: Logout Bölümü nasıl yaparım
        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");
            return RedirectToPage("/Login");
        }
    }
}
