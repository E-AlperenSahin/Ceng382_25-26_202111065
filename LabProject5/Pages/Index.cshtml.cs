using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject5.Models;
using LabProject5.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabProject5.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public static List<ClassInformationModel> ClassList { get; set; } = new();

        [BindProperty]
        public ClassInformationModel ClassInput { get; set; } = new();

        public List<ClassInformationTable> FilteredClassList { get; set; } = new();

        public bool IsEditMode { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        private static bool _isDataGenerated = false;

        private void GenerateDummyData()
        {
            if (_isDataGenerated) return;

            var random = new Random();
            for (int i = 1; i <= 100; i++)
            {
                ClassList.Add(new ClassInformationModel
                {
                    ClassName = $"Class {i}",
                    StudentCount = random.Next(10, 100),
                    Description = $"Sample description {i}"
                });
            }

            _isDataGenerated = true;
        }

        public IActionResult OnGet()
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

            GenerateDummyData();

            var query = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(c =>
                    c.ClassName.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                    c.Description.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            FilteredClassList = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();

            return Page();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            ClassList.Add(new ClassInformationModel
            {
                ClassName = ClassInput.ClassName,
                StudentCount = ClassInput.StudentCount,
                Description = ClassInput.Description
            });

            return RedirectToPage(new { PageNumber, SearchKeyword });
        }

        public IActionResult OnPostEditSelect(int id)
        {
            var existing = ClassList.FirstOrDefault(c => c.Id == id);
            if (existing == null)
                return RedirectToPage();

            ClassInput = existing;
            IsEditMode = true;
            return Page();
        }

        public IActionResult OnPostEdit()
        {
            var existing = ClassList.FirstOrDefault(c => c.Id == ClassInput.Id);
            if (existing != null)
            {
                existing.ClassName = ClassInput.ClassName;
                existing.StudentCount = ClassInput.StudentCount;
                existing.Description = ClassInput.Description;
            }

            return RedirectToPage(new { PageNumber, SearchKeyword });
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
                ClassList.Remove(item);

            return RedirectToPage(new { PageNumber, SearchKeyword });
        }


        public IActionResult OnPostExportJson(string SelectedColumns)
        {
            // Ai prompt: Kolonları Aktif Olarak Nasıl Kullanıbilirim
            var columns = string.IsNullOrWhiteSpace(SelectedColumns)
                ? new List<string>()
                : SelectedColumns.Split(',').ToList();

            var query = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(c =>
                    c.ClassName.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                    c.Description.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            var dataToExport = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

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
