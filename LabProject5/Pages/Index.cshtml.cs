using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject5.Models;
using System.Collections.Generic;
using System.Linq;

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

        public void OnGet()
        {

            GenerateDummyData();


            var query = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(c =>
                    c.ClassName.Contains(SearchKeyword, System.StringComparison.OrdinalIgnoreCase) ||
                    c.Description.Contains(SearchKeyword, System.StringComparison.OrdinalIgnoreCase));
            }


            int totalItems = query.Count();
            TotalPages = (int)System.Math.Ceiling(totalItems / (double)PageSize);


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

            return RedirectToPage(new { PageNumber = PageNumber, SearchKeyword = SearchKeyword });
        }

        // Satırdan seçmek için
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

            return RedirectToPage(new { PageNumber = PageNumber, SearchKeyword = SearchKeyword });
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
                ClassList.Remove(item);

            return RedirectToPage(new { PageNumber = PageNumber, SearchKeyword = SearchKeyword });
        }
    }
}
