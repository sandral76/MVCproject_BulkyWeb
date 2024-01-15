using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RazorTemp_Project.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }  //primary key for Category(ako stavimo NazivTabeleId ili Id samo ne mora anotacija za pk)
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}
