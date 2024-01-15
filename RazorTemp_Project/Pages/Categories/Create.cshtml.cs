using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorTemp_Project.Data;
using RazorTemp_Project.Models;

namespace RazorTemp_Project.Pages.Categories
{
    [BindProperties] //za vise obelezja
    public class CreateModel : PageModel
    {
        private readonly BulkyWeb_RezorDbContext _dbContext;
        [BindProperty]  //binding property to be available in methods
        public Category Category { get; set; }
        public CreateModel(BulkyWeb_RezorDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            _dbContext.Categories.Add(Category); //mozemo da korisitmo jer smo iskoristili atribut bind
            _dbContext.SaveChanges();
            TempData["success"] = "Category created successfully"; //obavestenje
            return RedirectToPage("Index");
        }
    }
}
