using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorTemp_Project.Data;
using RazorTemp_Project.Models;

namespace RazorTemp_Project.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly BulkyWeb_RezorDbContext _dbContext;
        public Category Category { get; set; }
        public DeleteModel(BulkyWeb_RezorDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void OnGet(int? id)
        {
            Category = _dbContext.Categories.Find(id);
        }
        public IActionResult OnPost(Category obj) 
        {
            if (obj == null)
            {
                return NotFound();
            }
            _dbContext.Categories.Remove(obj);
            _dbContext.SaveChanges();
            TempData["success"] = "Category deleted successfully"; //obavestenje
            return RedirectToPage("Index");
        }
    }
}
