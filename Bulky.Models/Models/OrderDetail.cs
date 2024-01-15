using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required] 
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }
        [Required]
        public int ProductId {  get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
        public int Count { get; set; }  //kolicina za svaki od proizvoda koji su u korpi

        public double Price { get; set; }  //cena se moze promeniti na sajtu npr, ali cena bi trebala da ostane ista za korisnika kada je izvrsio porudzbinu

    }
}
