using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorTemp_Project.Data;
using RazorTemp_Project.Models;

namespace RazorTemp_Project.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly BulkyWeb_RezorDbContext _dbContext;
        public Category Category { get; set; }
        public EditModel(BulkyWeb_RezorDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void OnGet(int? id)
        {
            if(id != null && id != 0)
            {
                Category = _dbContext.Categories.Find(id);
            }
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Update(Category);
                _dbContext.SaveChanges();
                TempData["success"] = "Category updated successfully"; //obavestenje
                return RedirectToPage("Index");
            }
            return Page();


        }
    }
}
