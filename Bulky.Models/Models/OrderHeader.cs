using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }  //kom korisniku pripada porudzbina
    
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }

        // when order is shiped
        public string? TrackingNumber { get; set; }
        public string? Carrier {  get; set; }

        public DateTime PaymentDate { get; set; }

        public DateOnly PaymentDueDate { get; set; }  //rok za placanje je 30 dana
    
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }  //jedinstveni identifikator za svaku porudzbinu
        [Required]
        public string? StreetAddress { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        public string? PostalCode { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Name { get; set; }

    }
}
