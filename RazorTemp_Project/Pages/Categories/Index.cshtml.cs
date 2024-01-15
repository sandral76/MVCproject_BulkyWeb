using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorTemp_Project.Data;
using RazorTemp_Project.Models;

namespace RazorTemp_Project.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly BulkyWeb_RezorDbContext _dbContext;
        public List<Category> CategoryList { get; set; }
        public IndexModel(BulkyWeb_RezorDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void OnGet()
        {
            CategoryList = _dbContext.Categories.ToList();
        }
    }
}
