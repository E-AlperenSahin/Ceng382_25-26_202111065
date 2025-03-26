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
        public ClassInformationModel ClassInput { get; set; }

        public bool IsEditMode { get; set; } = false;

        public void OnGet()
        {
            
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

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
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

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
                ClassList.Remove(item);

            return RedirectToPage();
        }
    }
}
